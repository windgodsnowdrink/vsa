#:sdk Microsoft.NET.Sdk.Web
#:package ClosedXML@0.102.1
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using System.Threading.Channels;
using ClosedXML.Excel;
using Microsoft.Extensions.ObjectPool;

[SkipLocalsInit]
public sealed class ExcelDocumentService : IAsyncDisposable
{
    private readonly Channel<ExcelRequest> _requestChannel;
    private readonly Channel<ExcelOperation> _operationChannel;
    private readonly ObjectPool<XLWorkbook> _workbookPool;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private readonly CancellationTokenSource _cts;

    public ExcelDocumentService()
    {
        _latencyOptimizer = new TailLatencyOptimizer();
        _cts = new CancellationTokenSource();
        
        _requestChannel = Channel.CreateBounded<ExcelRequest>(
            new BoundedChannelOptions(10_000)
            {
                SingleReader = true,
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.DropOldest
            });
            
        _operationChannel = Channel.CreateBounded<ExcelOperation>(
            new BoundedChannelOptions(10_000)
            {
                SingleReader = true,
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.DropOldest
            });
        
        _workbookPool = new DefaultObjectPool<XLWorkbook>(
            new WorkbookPooledPolicy(), 
            Environment.ProcessorCount * 2);
        
        _ = Task.Run(ProcessRequestsAsync);
        _ = Task.Run(ProcessOperationsAsync);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async ValueTask GenerateDocumentAsync<T>(IEnumerable<T> data, string filePath)
    {
        var request = new ExcelRequest(data, filePath);
        await _requestChannel.Writer.WriteAsync(request, _cts.Token);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async IAsyncEnumerable<T> ReadExcelAsync<T>(string filePath) where T : class, new()
    {
        using var stream = new FileStream(filePath, FileMode.Open);
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet(1);
        
        var rows = worksheet.RowsUsed().Skip(1);
        foreach (var row in rows)
        {
            yield return MapRowToModel<T>(row);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task ProcessRequestsAsync()
    {
        await foreach (var request in _requestChannel.Reader.ReadAllAsync(_cts.Token))
        {
            using var latencyToken = _latencyOptimizer.BeginOperation();
            var workbook = _workbookPool.Get();
            try
            {
                ProcessWorkbook(workbook, request.Data);
                await SaveWorkbookAsync(workbook, request.FilePath);
            }
            finally
            {
                _workbookPool.Return(workbook);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task ProcessOperationsAsync()
    {
        await foreach (var operation in _operationChannel.Reader.ReadAllAsync(_cts.Token))
        {
            using var latencyToken = _latencyOptimizer.BeginOperation();
            var workbook = _workbookPool.Get();
            try
            {
                using var stream = new FileStream(operation.FilePath, FileMode.Open);
                workbook.Load(stream);
                
                operation.Processor(workbook.Worksheet(1));
                
                await SaveWorkbookAsync(workbook, operation.FilePath);
            }
            finally
            {
                _workbookPool.Return(workbook);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private void ProcessWorkbook<T>(XLWorkbook workbook, IEnumerable<T> data)
    {
        var worksheet = workbook.Worksheets.Add("Data");
        worksheet.Cell(1, 1).InsertTable(data);
        
        worksheet.Style.Font.SetFontSize(12);
        worksheet.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        worksheet.Columns().AdjustToContents();
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private T MapRowToModel<T>(IXLRow row) where T : class, new()
    {
        var model = new T();
        var properties = typeof(T).GetProperties();
        
        for (int i = 0; i < properties.Length; i++)
        {
            var cellValue = row.Cell(i + 1).Value;
            properties[i].SetValue(model, Convert.ChangeType(cellValue, properties[i].PropertyType));
        }
        
        return model;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task SaveWorkbookAsync(XLWorkbook workbook, string filePath)
    {
        using var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);
        
        await using var fileStream = new FileStream(
            filePath, 
            FileMode.Create, 
            FileAccess.Write, 
            FileShare.None, 
            bufferSize: 4096, 
            useAsync: true);
        
        await memoryStream.CopyToAsync(fileStream);
    }

    // 高级功能方法
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task AddCommentAsync(string filePath, string cellAddress, string commentText)
    {
        var operation = new ExcelOperation(filePath, ws => {
            var cell = ws.Cell(cellAddress);
            cell.Comment.Delete();
            cell.Comment.AddText(commentText);
            cell.Comment.Style.Alignment.SetAutomaticSize();
        });
        await _operationChannel.Writer.WriteAsync(operation, _cts.Token);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task ConfigurePrintSettingsAsync(string filePath, Action<IXLPageSetup> setup)
    {
        var operation = new ExcelOperation(filePath, ws => {
            setup(ws.PageSetup);
            ws.PageSetup.PrintAreas.Add(ws.RangeUsed());
        });
        await _operationChannel.Writer.WriteAsync(operation, _cts.Token);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task AddMacroAsync(string sourcePath, string targetPath, string macroCode)
    {
        var operation = new ExcelOperation(sourcePath, ws => {
            ws.Workbook.AddVbaProject(macroCode);
            ws.Workbook.SaveOptions.CompressionLevel = ZipCompressionLevel.BestSpeed;
        });
        await _operationChannel.Writer.WriteAsync(operation, _cts.Token);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task ProtectWorksheetAsync(string filePath, string password)
    {
        var operation = new ExcelOperation(filePath, ws => {
            ws.Protect(password, XLSheetProtection.All);
        });
        await _operationChannel.Writer.WriteAsync(operation, _cts.Token);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task AddDataValidationAsync(string filePath, string rangeAddress, 
        XLDataType validationType, string formula)
    {
        var operation = new ExcelOperation(filePath, ws => {
            var range = ws.Range(rangeAddress);
            range.DataValidation.Clear();
            range.DataValidation.Add(validationType);
            range.DataValidation.Value = formula;
        });
        await _operationChannel.Writer.WriteAsync(operation, _cts.Token);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task CalculateFormulasAsync(string filePath)
    {
        var operation = new ExcelOperation(filePath, ws => {
            ws.RecalculateAllFormulas();
            ws.CalculateMode = XLCalculateMode.Auto;
        });
        await _operationChannel.Writer.WriteAsync(operation, _cts.Token);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task CreatePivotTableAsync(string sourcePath, string targetPath, 
        string sourceRange, string pivotRange)
    {
        var operation = new ExcelOperation(sourcePath, ws => {
            var pivotTable = ws.PivotTables.Add("PivotTable", ws.Range(pivotRange), 
                ws.Range(sourceRange));
            
            pivotTable.RowLabels.Add("Category");
            pivotTable.Values.Add("Amount").NumberFormat = "$#,##0.00";
        });
        await _operationChannel.Writer.WriteAsync(operation, _cts.Token);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task ApplyStyleTemplateAsync(string filePath, Action<IXLStyle> styleConfig)
    {
        var operation = new ExcelOperation(filePath, ws => {
            var style = new XLStyle(ws.Workbook);
            styleConfig(style);
            ws.Style = style;
        });
        await _operationChannel.Writer.WriteAsync(operation, _cts.Token);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task ApplyConditionalFormattingAsync(string filePath, Action<IXLWorksheet> formatter)
    {
        var operation = new ExcelOperation(filePath, formatter);
        await _operationChannel.Writer.WriteAsync(operation, _cts.Token);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task AddChartAsync(string filePath, Action<IXLWorksheet> chartConfig)
    {
        var operation = new ExcelOperation(filePath, ws => {
            chartConfig(ws);
            ws.AddChart(XLChartType.ColumnClustered);
        });
        await _operationChannel.Writer.WriteAsync(operation, _cts.Token);
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _requestChannel.Writer.Complete();
        _operationChannel.Writer.Complete();
        await Task.WhenAll(
            _requestChannel.Reader.Completion,
            _operationChannel.Reader.Completion);
    }
}

[SkipLocalsInit]
internal sealed class WorkbookPooledPolicy : PooledObjectPolicy<XLWorkbook>
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override XLWorkbook Create() => new XLWorkbook();

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override bool Return(XLWorkbook obj)
    {
        obj.Dispose();
        return true;
    }
}

// 启动配置
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ExcelDocumentService>();

var app = builder.Build();
app.MapGet("/", () => "Excel Document Service");
app.Run();

internal record ExcelRequest(IEnumerable<object> Data, string FilePath);
internal record ExcelOperation(string FilePath, Action<IXLWorksheet> Processor);