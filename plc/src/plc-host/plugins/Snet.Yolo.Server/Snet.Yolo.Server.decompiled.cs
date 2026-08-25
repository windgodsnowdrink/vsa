using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SkiaSharp;
using Snet.Core.@abstract;
using Snet.Core.extend;
using Snet.DB;
using Snet.DB.sugar;
using Snet.Model.data;
using Snet.Utility;
using Snet.Yolo.Server.handler;
using Snet.Yolo.Server.@interface;
using Snet.Yolo.Server.models.data;
using Snet.Yolo.Server.models.@enum;
using YoloDotNet;
using YoloDotNet.Enums;
using YoloDotNet.Models;
using YoloDotNet.Models.Interfaces;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETCoreApp,Version=v8.0", FrameworkDisplayName = ".NET 8.0")]
[assembly: AssemblyCompany("Shun")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyDescription("识别组件：Server（Yolo多模型管理与智能识别服务，检测、定向检测、分类、分割、姿态）")]
[assembly: AssemblyFileVersion("26.235.2.0")]
[assembly: AssemblyInformationalVersion("26.235.2+1244958fb38bb9d761aacac13617c9e9d08d222d")]
[assembly: AssemblyProduct("Yolo")]
[assembly: AssemblyTitle("Snet.Yolo.Server")]
[assembly: AssemblyVersion("26.235.2.0")]
[module: RefSafetyRules(11)]
namespace Snet.Yolo.Server
{
	/// <summary>
	/// YOLO 视觉识别操作类，基于 YoloDotNet 实现对象检测、图像分割、分类、姿态估计、定向检测等 AI 视觉任务。
	/// </summary>
	public class IdentityOperate : CoreUnify<IdentityOperate, IdentityData>, IIdentity, IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// 生命周期
		/// </summary>
		private CancellationTokenSource tokenSource;

		/// <summary>
		/// yolo 对象<br />
		/// https://github.com/NickSwardh/YoloDotNet
		/// </summary>
		private Yolo _yolo;

		/// <inheritdoc />
		protected override string CN => "视觉识别";

		/// <inheritdoc />
		protected override string CD => "一个速度极快、功能齐全的 C# 库，用于使用 YOLOv5u–v26、YOLO-World 和 YOLO-E 模型进行实时物体检测、OBB、分割、分类、位姿估计和跟踪";

		/// <summary>
		/// 识别操作<br />
		/// 无参构造函数
		/// </summary>
		public IdentityOperate()
		{
		}

		/// <summary>
		/// 识别操作<br />
		/// 有参构造函数
		/// </summary>
		/// <param name="data">基础数据</param>
		public IdentityOperate(IdentityData data)
			: base(data)
		{
		}

		/// <summary>
		/// 初始化
		/// </summary>
		private Yolo Init()
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected O, but got Unknown
			if (_yolo == null)
			{
				_yolo = new Yolo(new YoloOptions
				{
					ExecutionProvider = base.basics.Hardware,
					ImageResize = (ImageResize)0
				});
			}
			return _yolo;
		}

		/// <inheritdoc />
		public async Task<OperateResult> RunAsync(IData data)
		{
			if (tokenSource == null)
			{
				tokenSource = new CancellationTokenSource();
			}
			return (OperateResult)(base.basics.IdentifyType switch
			{
				OnnxType.ObjectDetection => await RunAsync(ExtendMethod.GetSource<ObjectDetectionData>((object)data), tokenSource.Token), 
				OnnxType.Segmentation => await RunAsync(ExtendMethod.GetSource<SegmentationData>((object)data), tokenSource.Token), 
				OnnxType.Classification => await RunAsync(ExtendMethod.GetSource<ClassificationData>((object)data), tokenSource.Token), 
				OnnxType.PoseEstimation => await RunAsync(ExtendMethod.GetSource<PoseEstimationData>((object)data), tokenSource.Token), 
				OnnxType.ObbDetection => await RunAsync(ExtendMethod.GetSource<ObbDetectionData>((object)data), tokenSource.Token), 
				_ => await Task.FromResult<OperateResult>(OperateResult.CreateFailureResult("识别类型错误", (int?)null)).ConfigureAwait(continueOnCapturedContext: false), 
			});
		}

		/// <inheritdoc />
		public async Task<OperateResult> RunAsync(ClassificationData data, CancellationToken token)
		{
			await base.BegOperateAsync(token, "RunAsync");
			try
			{
				SKImage image = SKImage.FromEncodedData(data.File);
				try
				{
					List<ClassificationResultData> list = Init().RunClassification(image, data.Classes).ToClassificationResultData();
					return await base.EndOperateAsync(true, (string)null, (object)list, (Exception)null, true, true, token, "F:\\Snet\\VisualIdentity\\Snet.Yolo.Server\\IdentityOperate.cs", "RunAsync", 96);
				}
				finally
				{
					((IDisposable)image)?.Dispose();
				}
			}
			catch (Exception ex)
			{
				return await base.EndOperateAsync(false, ex.Message, (object)ex, (Exception)null, true, true, token, "F:\\Snet\\VisualIdentity\\Snet.Yolo.Server\\IdentityOperate.cs", "RunAsync", 100);
			}
		}

		/// <inheritdoc />
		public async Task<OperateResult> RunAsync(ObbDetectionData data, CancellationToken token)
		{
			await base.BegOperateAsync(token, "RunAsync");
			try
			{
				SKImage image = SKImage.FromEncodedData(data.File);
				try
				{
					List<ObbDetectionResultData> list = Init().RunObbDetection(image, data.Confidence, data.Iou, (SKRectI?)null).ToObbDetectionResultData();
					return await base.EndOperateAsync(true, (string)null, (object)list, (Exception)null, true, true, token, "F:\\Snet\\VisualIdentity\\Snet.Yolo.Server\\IdentityOperate.cs", "RunAsync", 113);
				}
				finally
				{
					((IDisposable)image)?.Dispose();
				}
			}
			catch (Exception ex)
			{
				return await base.EndOperateAsync(false, ex.Message, (object)ex, (Exception)null, true, true, token, "F:\\Snet\\VisualIdentity\\Snet.Yolo.Server\\IdentityOperate.cs", "RunAsync", 117);
			}
		}

		/// <inheritdoc />
		public async Task<OperateResult> RunAsync(ObjectDetectionData data, CancellationToken token)
		{
			await base.BegOperateAsync(token, "RunAsync");
			try
			{
				SKImage image = SKImage.FromEncodedData(data.File);
				try
				{
					List<ObjectDetectionResultData> list = Init().RunObjectDetection(image, data.Confidence, data.Iou, (SKRectI?)null).ToObjectDetectionResultData();
					return await base.EndOperateAsync(true, (string)null, (object)list, (Exception)null, true, true, token, "F:\\Snet\\VisualIdentity\\Snet.Yolo.Server\\IdentityOperate.cs", "RunAsync", 130);
				}
				finally
				{
					((IDisposable)image)?.Dispose();
				}
			}
			catch (Exception ex)
			{
				return await base.EndOperateAsync(false, ex.Message, (object)ex, (Exception)null, true, true, token, "F:\\Snet\\VisualIdentity\\Snet.Yolo.Server\\IdentityOperate.cs", "RunAsync", 134);
			}
		}

		/// <inheritdoc />
		public async Task<OperateResult> RunAsync(PoseEstimationData data, CancellationToken token)
		{
			await base.BegOperateAsync(token, "RunAsync");
			try
			{
				SKImage image = SKImage.FromEncodedData(data.File);
				try
				{
					List<PoseEstimationResultData> list = Init().RunPoseEstimation(image, data.Confidence, data.Iou, (SKRectI?)null).ToPoseEstimationResultData();
					return await base.EndOperateAsync(true, (string)null, (object)list, (Exception)null, true, true, token, "F:\\Snet\\VisualIdentity\\Snet.Yolo.Server\\IdentityOperate.cs", "RunAsync", 147);
				}
				finally
				{
					((IDisposable)image)?.Dispose();
				}
			}
			catch (Exception ex)
			{
				return await base.EndOperateAsync(false, ex.Message, (object)ex, (Exception)null, true, true, token, "F:\\Snet\\VisualIdentity\\Snet.Yolo.Server\\IdentityOperate.cs", "RunAsync", 151);
			}
		}

		/// <inheritdoc />
		public async Task<OperateResult> RunAsync(SegmentationData data, CancellationToken token)
		{
			await base.BegOperateAsync(token, "RunAsync");
			try
			{
				SKImage image = SKImage.FromEncodedData(data.File);
				try
				{
					List<SegmentationResultData> list = Init().RunSegmentation(image, data.Confidence, data.PixelConfidence, data.Iou, (SKRectI?)null).ToSegmentationResultData();
					return await base.EndOperateAsync(true, (string)null, (object)list, (Exception)null, true, true, token, "F:\\Snet\\VisualIdentity\\Snet.Yolo.Server\\IdentityOperate.cs", "RunAsync", 164);
				}
				finally
				{
					((IDisposable)image)?.Dispose();
				}
			}
			catch (Exception ex)
			{
				return await base.EndOperateAsync(false, ex.Message, (object)ex, (Exception)null, true, true, token, "F:\\Snet\\VisualIdentity\\Snet.Yolo.Server\\IdentityOperate.cs", "RunAsync", 168);
			}
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			if (tokenSource != null)
			{
				tokenSource.Cancel();
				tokenSource.Dispose();
				tokenSource = null;
			}
			if (_yolo != null)
			{
				_yolo.Dispose();
				_yolo = null;
			}
			base.Dispose();
		}

		/// <inheritdoc />
		public override async ValueTask DisposeAsync()
		{
			if (tokenSource != null)
			{
				await tokenSource.CancelAsync();
				tokenSource.Dispose();
				tokenSource = null;
			}
			if (_yolo != null)
			{
				_yolo.Dispose();
				_yolo = null;
			}
			await base.DisposeAsync();
		}
	}
	/// <summary>
	/// ONNX 模型管理操作类，基于 SQLite 实现模型文件的增删改查管理。
	/// </summary>
	public class ManageOperate : CoreUnify<ManageOperate, string>, IManage, IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// 初始化状态
		/// </summary>
		private OperateResult? _initResult;

		/// <summary>
		/// 数据库路径
		/// </summary>
		private readonly string DbPath = Path.Combine(PublicHandler.DefaultPath, "db");

		/// <inheritdoc />
		protected override string CN => "轻量级数据库";

		/// <inheritdoc />
		protected override string CD => "一个轻量级、嵌入式的关系型数据库";

		/// <summary>
		/// 数据库操作对象
		/// </summary>
		private DBOperate operate
		{
			get
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Expected O, but got Unknown
				return CoreUnify<DBOperate, Basics>.Instance(new Basics
				{
					SN = PublicHandler.DefaultSN,
					ConnectStr = "Data Source=" + Path.Combine(DbPath, PublicHandler.DefaultDBName),
					DBType = (DBType)3,
					HandlerType = (DBHandlerType)0
				});
			}
		}

		/// <summary>
		/// 无参构造函数
		/// </summary>
		public ManageOperate()
		{
		}

		/// <summary>
		/// 管理操作<br />
		/// 有参构造函数
		/// </summary>
		/// <param name="data">基础数据</param>
		public ManageOperate(string data)
			: base(data)
		{
		}

		/// <summary>
		/// 初始化
		/// </summary>
		private async Task<OperateResult> InitAsync(CancellationToken token = default(CancellationToken))
		{
			_ = 2;
			try
			{
				if (!Directory.Exists(DbPath))
				{
					Directory.CreateDirectory(DbPath);
				}
				OperateResult result = await ((DaqAbstract<DBOperate, Basics>)(object)operate).OnAsync(token);
				if (!((BaseModel)(await operate.ExistAsync<OnnxData>(token))).Status)
				{
					await operate.CreateAsync<OnnxData>(token);
				}
				return result;
			}
			catch (Exception ex)
			{
				return OperateResult.CreateFailureResult(ex.Message, (int?)null);
			}
		}

		/// <inheritdoc />
		public async Task<OperateResult> AddAsync(string file, string describe, OnnxType onnxType, CancellationToken token = default(CancellationToken))
		{
			if (_initResult == null)
			{
				_initResult = await InitAsync(token);
			}
			if (!((BaseModel)_initResult).Status)
			{
				return _initResult;
			}
			string path = Path.GetDirectoryName(file);
			string name = Path.GetFileName(file);
			if (!((BaseModel)(await operate.QueryAsync<OnnxData>((Expression<Func<OnnxData, bool>>)((OnnxData c) => c.path == path && c.name == name), token))).Status)
			{
				return await operate.InsertAsync<OnnxData>(new OnnxData
				{
					size = new FileInfo(file).Length.GetFileSize(),
					path = path,
					name = name,
					onnxType = onnxType,
					describe = describe
				}, token);
			}
			return OperateResult.CreateFailureResult(name + "文件已存在", (int?)null);
		}

		/// <inheritdoc />
		public async Task<OperateResult> UpdateAsync(int index, string describe, OnnxType? onnxType = null, CancellationToken token = default(CancellationToken))
		{
			if (_initResult == null)
			{
				_initResult = await InitAsync(token);
			}
			if (!((BaseModel)_initResult).Status)
			{
				return _initResult;
			}
			OperateResult val = await operate.QueryAsync<OnnxData>((Expression<Func<OnnxData, bool>>)((OnnxData c) => c.index == index), token);
			List<OnnxData> list = default(List<OnnxData>);
			if (val != null && ((ResultModel)val).GetDetails<List<OnnxData>>(ref list) && list != null && list.Count > 0)
			{
				OnnxData onnxData = list[0];
				if (!ExtendMethod.IsNullOrWhiteSpace(describe))
				{
					onnxData.describe = describe;
				}
				if (onnxType.HasValue)
				{
					onnxData.onnxType = onnxType;
				}
				onnxData.updateTime = DateTime.Now;
				return await operate.UpdateAsync<OnnxData>(onnxData, (Expression<Func<OnnxData, object>>)((OnnxData u) => new { u.describe, u.onnxType, u.updateTime }), (Expression<Func<OnnxData, bool>>)((OnnxData c) => c.index == index), token);
			}
			return val;
		}

		/// <inheritdoc />
		public async Task<OperateResult> DeleteAsync(int index, bool deleteFile = true, CancellationToken token = default(CancellationToken))
		{
			if (_initResult == null)
			{
				_initResult = await InitAsync(token);
			}
			if (!((BaseModel)_initResult).Status)
			{
				return _initResult;
			}
			if (deleteFile)
			{
				OperateResult val = await operate.QueryAsync<OnnxData>((Expression<Func<OnnxData, bool>>)((OnnxData c) => c.index == index), token);
				List<OnnxData> list = default(List<OnnxData>);
				if (!((ResultModel)val).GetDetails<List<OnnxData>>(ref list))
				{
					return val;
				}
				File.Delete(Path.Combine(list[0].path, list[0].name));
			}
			return await operate.DeleteAsync<OnnxData>((Expression<Func<OnnxData, bool>>)((OnnxData c) => c.index == index), default(CancellationToken));
		}

		/// <inheritdoc />
		public async Task<OperateResult> QueryAsync(int index, CancellationToken token = default(CancellationToken))
		{
			if (_initResult == null)
			{
				_initResult = await InitAsync(token);
			}
			if (!((BaseModel)_initResult).Status)
			{
				return _initResult;
			}
			return await operate.QueryAsync<OnnxData>((Expression<Func<OnnxData, bool>>)((OnnxData c) => c.index == index), token);
		}

		/// <inheritdoc />
		public async Task<OperateResult> QueryAsync(CancellationToken token = default(CancellationToken))
		{
			if (_initResult == null)
			{
				_initResult = await InitAsync(token);
			}
			if (!((BaseModel)_initResult).Status)
			{
				return _initResult;
			}
			return await operate.QueryAsync<OnnxData>((Expression<Func<OnnxData, bool>>)null, default(CancellationToken));
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			((CoreUnify<DBOperate, Basics>)(object)operate).Dispose();
			base.Dispose();
		}

		/// <inheritdoc />
		public override async ValueTask DisposeAsync()
		{
			await ((CoreUnify<DBOperate, Basics>)(object)operate).DisposeAsync();
			await base.DisposeAsync();
		}
	}
}
namespace Snet.Yolo.Server.models.@enum
{
	/// <summary>
	/// 识别类型<br />
	/// https://docs.ultralytics.com/zh/tasks/#detection
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum OnnxType
	{
		/// <summary>
		/// 对象检测<br />
		/// 检测是YOLO支持的主要任务。它包括识别图像或视频帧中的对象，并在其周围绘制边界框。检测到的对象根据其特征被分类为不同的类别。YOLO可以以高精度和速度检测单个图像或视频帧中的多个对象，使其成为监控系统和自动驾驶汽车等实时应用的理想选择。
		/// </summary>
		[Description("对象检测")]
		ObjectDetection,
		/// <summary>
		/// 图像分割<br />
		/// 分割通过将图像分割成基于内容的不同区域，进一步推进了对象检测。每个区域都被分配一个标签，为医学成像、农业分析和制造业质量控制等应用提供像素级精度。YOLO 实现了 U-Net 架构的变体，以执行高效而准确的分割。
		/// </summary>
		[Description("图像分割")]
		Segmentation,
		/// <summary>
		/// 分类<br />
		/// 分类涉及根据图像的内容对整个图像进行分类。YOLO 的分类功能利用 EfficientNet 架构的变体来提供高性能的图像分类。此任务对于电子商务中的 产品分类 、内容审核 和 野生动物监测 等应用至关重要。
		/// </summary>
		[Description("分类")]
		Classification,
		/// <summary>
		/// 姿态估计<br />
		/// 姿态估计检测图像或视频帧中的特定关键点，以跟踪运动或估计姿势。这些关键点可以代表人体的关节、面部特征或其他重要的兴趣点。YOLO 在关键点检测方面表现出色，具有高精度和高速度，使其在 健身应用、运动分析 和 人机交互 领域具有重要价值。
		/// </summary>
		[Description("姿态估计")]
		PoseEstimation,
		/// <summary>
		/// 定向检测<br />
		/// 定向边界框（OBB）检测通过增加一个方向角来更好地定位旋转对象，从而增强了传统的对象检测。这种能力对于航空图像分析、文档处理和工业应用尤其有价值，在这些应用中，对象以各种角度出现。YOLO 在各种场景中为检测旋转对象提供高精度和速度。
		/// </summary>
		[Description("定向检测")]
		ObbDetection
	}
}
namespace Snet.Yolo.Server.models.data
{
	/// <summary>
	/// 分类
	/// </summary>
	public class ClassificationData : IData
	{
		/// <summary>
		/// 传进来的图片
		/// </summary>
		public byte[] File { get; set; }

		/// <summary>
		/// 分类
		/// </summary>
		public int Classes { get; set; } = 1;

		public ClassificationData()
		{
		}

		public ClassificationData(byte[] file, int classes = 1)
		{
			File = file;
			Classes = classes;
		}
	}
	/// <summary>
	/// 分类结果
	/// </summary>
	public class ClassificationResultData : IClassification
	{
		public string Label { get; set; }

		public double Confidence { get; set; }
	}
	/// <summary>
	/// 识别数据
	/// </summary>
	public class IdentityData
	{
		/// <summary>
		/// 唯一标识符
		/// </summary>
		[Description("唯一标识符")]
		public string? SN { get; set; } = ExtendMethod.ToUpperNString(Guid.NewGuid());

		/// <summary>
		/// 识别类型
		/// </summary>
		[Description("识别类型")]
		public OnnxType IdentifyType { get; set; }

		/// <summary>
		/// 硬件<br />
		/// 使用什么硬件来进行运算
		/// </summary>
		[Description("硬件")]
		public IExecutionProvider Hardware { get; set; }
	}
	/// <summary>
	/// 识别结果数据
	/// </summary>
	public class IdentityResultData<T>
	{
		/// <summary>
		/// 结果数据
		/// </summary>
		[Description("结果数据")]
		public T Result { get; set; }

		/// <summary>
		/// 标记图片
		/// </summary>
		[Description("标记图片")]
		public string MarkImage { get; set; }

		/// <summary>
		/// 原始图片
		/// </summary>
		[Description("原始图片")]
		public string OriginalImage { get; set; }

		/// <summary>
		/// 识别构造函数
		/// </summary>
		/// <param name="result">结果数据</param>
		/// <param name="markImage">标记图片</param>
		/// <param name="originalImage">原始图片</param>
		public IdentityResultData(T result, string markImage, string originalImage)
		{
			Result = result;
			MarkImage = markImage;
			OriginalImage = originalImage;
		}
	}
	/// <summary>
	/// 定向检测
	/// </summary>
	public class ObbDetectionData : IData
	{
		/// <summary>
		/// 传进来的图片
		/// </summary>
		public byte[] File { get; set; }

		/// <summary>
		/// 置信度
		/// </summary>
		public double Confidence { get; set; } = 0.2;

		/// <summary>
		/// 交并比
		/// </summary>
		public double Iou { get; set; } = 0.7;

		public ObbDetectionData()
		{
		}

		public ObbDetectionData(byte[] file, double confidence = 0.2, double iou = 0.7)
		{
			File = file;
			Confidence = confidence;
			Iou = iou;
		}
	}
	/// <summary>
	/// 定向检测
	/// </summary>
	public class ObbDetectionResultData : TrackingInfo, IDetection
	{
		[CompilerGenerated]
		private readonly SKRectI <BoundingBox>k__BackingField;

		public LabelModel Label { get; init; }

		public double Confidence { get; init; }

		[JsonIgnore]
		[JsonIgnore]
		public SKRectI BoundingBox
		{
			[CompilerGenerated]
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return <BoundingBox>k__BackingField;
			}
			[CompilerGenerated]
			init
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				<BoundingBox>k__BackingField = value;
			}
		}

		public string Position { get; init; }

		public float OrientationAngle { get; set; }

		public ObbDetectionResultData()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			Label = new LabelModel();
			((TrackingInfo)this)..ctor();
		}
	}
	/// <summary>
	/// 检测
	/// </summary>
	public class ObjectDetectionData : IData
	{
		/// <summary>
		/// 传进来的图片
		/// </summary>
		public byte[] File { get; set; }

		/// <summary>
		/// 置信度
		/// </summary>
		public double Confidence { get; set; } = 0.2;

		/// <summary>
		/// 交并比
		/// </summary>
		public double Iou { get; set; } = 0.7;

		public ObjectDetectionData()
		{
		}

		public ObjectDetectionData(byte[] file, double confidence = 0.2, double iou = 0.7)
		{
			File = file;
			Confidence = confidence;
			Iou = iou;
		}
	}
	/// <summary>
	/// 检测
	/// </summary>
	public class ObjectDetectionResultData : TrackingInfo, IDetection
	{
		[CompilerGenerated]
		private readonly SKRectI <BoundingBox>k__BackingField;

		public LabelModel Label { get; init; }

		public double Confidence { get; init; }

		[JsonIgnore]
		[JsonIgnore]
		public SKRectI BoundingBox
		{
			[CompilerGenerated]
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return <BoundingBox>k__BackingField;
			}
			[CompilerGenerated]
			init
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				<BoundingBox>k__BackingField = value;
			}
		}

		public string Position { get; init; }

		public ObjectDetectionResultData()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			Label = new LabelModel();
			((TrackingInfo)this)..ctor();
		}
	}
	/// <summary>
	/// 模型
	/// </summary>
	public class OnnxData
	{
		/// <summary>
		/// 下标<br />
		/// 无需手动设置(自增)
		/// </summary>
		[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
		public int index { get; set; }

		/// <summary>
		/// 名称<br />
		/// 无需手动设置
		/// </summary>
		public string? name { get; set; }

		/// <summary>
		/// 描述
		/// </summary>
		public string describe { get; set; }

		/// <summary>
		/// 文件大小<br />
		/// 无需手动设置
		/// </summary>
		public string? size { get; set; }

		/// <summary>
		/// 路径<br />
		/// 无需手动设置
		/// </summary>
		public string? path { get; set; }

		/// <summary>
		/// 模型类型
		/// </summary>
		public OnnxType? onnxType { get; set; }

		/// <summary>
		/// 创建时间<br />
		/// 无需手动设置
		/// </summary>
		public DateTime createTime { get; set; } = DateTime.Now;

		/// <summary>
		/// 更新时间<br />
		/// 无需手动设置
		/// </summary>
		public DateTime updateTime { get; set; } = DateTime.Now;
	}
	/// <summary>
	/// 姿态
	/// </summary>
	public class PoseEstimationData : IData
	{
		/// <summary>
		/// 传进来的图片
		/// </summary>
		public byte[] File { get; set; }

		/// <summary>
		/// 置信度
		/// </summary>
		public double Confidence { get; set; } = 0.2;

		/// <summary>
		/// 交并比
		/// </summary>
		public double Iou { get; set; } = 0.7;

		public PoseEstimationData()
		{
		}

		public PoseEstimationData(byte[] file, double confidence = 0.2, double iou = 0.7)
		{
			File = file;
			Confidence = confidence;
			Iou = iou;
		}
	}
	/// <summary>
	/// 姿态结果
	/// </summary>
	public class PoseEstimationResultData : TrackingInfo, IDetection
	{
		[CompilerGenerated]
		private readonly SKRectI <BoundingBox>k__BackingField;

		public LabelModel Label { get; init; }

		public double Confidence { get; init; }

		[JsonIgnore]
		[JsonIgnore]
		public SKRectI BoundingBox
		{
			[CompilerGenerated]
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return <BoundingBox>k__BackingField;
			}
			[CompilerGenerated]
			init
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				<BoundingBox>k__BackingField = value;
			}
		}

		public string Position { get; init; }

		public KeyPoint[] KeyPoints { get; set; }

		public PoseEstimationResultData()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			Label = new LabelModel();
			KeyPoints = Array.Empty<KeyPoint>();
			((TrackingInfo)this)..ctor();
		}
	}
	/// <summary>
	/// 分割
	/// </summary>
	public class SegmentationData : IData
	{
		/// <summary>
		/// 传进来的图片
		/// </summary>
		public byte[] File { get; set; }

		/// <summary>
		/// 置信度
		/// </summary>
		public double Confidence { get; set; } = 0.2;

		/// <summary>
		/// 交并比
		/// </summary>
		public double Iou { get; set; } = 0.7;

		/// <summary>
		/// 像素置信度
		/// </summary>
		public double PixelConfidence { get; set; } = 0.65;

		public SegmentationData()
		{
		}

		public SegmentationData(byte[] file, double confidence = 0.2, double pixelConfedence = 0.65, double iou = 0.7)
		{
			File = file;
			Confidence = confidence;
			Iou = iou;
			PixelConfidence = pixelConfedence;
		}
	}
	/// <summary>
	/// 分割结果
	/// </summary>
	public class SegmentationResultData : TrackingInfo, IDetection
	{
		[CompilerGenerated]
		private readonly SKRectI <BoundingBox>k__BackingField;

		public LabelModel Label { get; init; }

		public double Confidence { get; init; }

		[JsonIgnore]
		[JsonIgnore]
		public SKRectI BoundingBox
		{
			[CompilerGenerated]
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return <BoundingBox>k__BackingField;
			}
			[CompilerGenerated]
			init
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				<BoundingBox>k__BackingField = value;
			}
		}

		public string Position { get; init; }

		public byte[] BitPackedPixelMask { get; set; }

		public SegmentationResultData()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			Label = new LabelModel();
			BitPackedPixelMask = Array.Empty<byte>();
			((TrackingInfo)this)..ctor();
		}
	}
}
namespace Snet.Yolo.Server.@interface
{
	/// <summary>
	/// 统一数据接口 — marker interface for type-safe dispatch in inference pipeline.
	/// All inference data types (ClassificationData, ObjectDetectionData, etc.) implement this.
	/// </summary>
	public interface IData
	{
		/// <summary>
		/// Raw image bytes for inference.
		/// </summary>
		byte[] File { get; set; }
	}
	/// <summary>
	/// 操作接口
	/// </summary>
	public interface IIdentity
	{
		/// <summary>
		/// 运行
		/// </summary>
		/// <param name="data">数据<br />
		/// ClassificationData(byte[] file, int classes = 1):分类数据<br />
		/// OBBDetectionData(byte[] file, double confidence = 0.2, double iou = 0.7):定向检测数据<br />
		/// ObjectDetectionData(byte[] file, double confidence = 0.2, double iou = 0.7):检测数据<br />
		/// PoseEstimationData(byte[] file, double confidence = 0.2, double iou = 0.7):姿态识别数据<br />
		/// SegmentationData(byte[] file, double confidence = 0.2, double pixelConfedence = 0.65, double iou = 0.7):分割数据
		/// </param>
		/// <returns>结果</returns>
		Task<OperateResult> RunAsync(IData data);

		/// <summary>
		/// 分类
		/// </summary>
		/// <param name="data">数据</param>
		/// <param name="token">生命周期</param>
		/// <returns>结果</returns>
		Task<OperateResult> RunAsync(ClassificationData data, CancellationToken token);

		/// <summary>
		/// 定向检测
		/// </summary>
		/// <param name="data">数据</param>
		/// <param name="token">生命周期</param>
		/// <returns>结果</returns>
		Task<OperateResult> RunAsync(ObbDetectionData data, CancellationToken token);

		/// <summary>
		/// 检测
		/// </summary>
		/// <param name="data">数据</param>
		/// <param name="token">生命周期</param>
		/// <returns>结果</returns>
		Task<OperateResult> RunAsync(ObjectDetectionData data, CancellationToken token);

		/// <summary>
		/// 姿态
		/// </summary>
		/// <param name="data">数据</param>
		/// <param name="token">生命周期</param>
		/// <returns>结果</returns>
		Task<OperateResult> RunAsync(PoseEstimationData data, CancellationToken token);

		/// <summary>
		/// 分割
		/// </summary>
		/// <param name="data">数据</param>
		/// <param name="token">生命周期</param>
		/// <returns>结果</returns>
		Task<OperateResult> RunAsync(SegmentationData data, CancellationToken token);
	}
	/// <summary>
	/// 模型管理接口
	/// </summary>
	public interface IManage
	{
		/// <summary>
		/// 添加
		/// </summary>
		/// <param name="file">文件</param>
		/// <param name="describe">描述</param>
		/// <param name="onnxType">模型类型</param>
		/// <param name="token">取消通知</param>
		/// <returns>结果</returns>
		Task<OperateResult> AddAsync(string file, string describe, OnnxType onnxType, CancellationToken token = default(CancellationToken));

		/// <summary>
		/// 修改
		/// </summary>
		/// <param name="index">下标</param>
		/// <param name="describe">描述</param>
		/// <param name="onnxType">类型</param>
		/// <param name="token">取消通知</param>
		/// <returns>结果</returns>
		Task<OperateResult> UpdateAsync(int index, string describe, OnnxType? onnxType = null, CancellationToken token = default(CancellationToken));

		/// <summary>
		/// 删除
		/// </summary>
		/// <param name="index">下标</param>
		/// <param name="deleteFile">是否删除文件</param>
		/// <param name="token">取消通知</param>
		/// <returns>结果</returns>
		Task<OperateResult> DeleteAsync(int index, bool deleteFile = true, CancellationToken token = default(CancellationToken));

		/// <summary>
		/// 指定查询
		/// </summary>
		/// <param name="index">下标</param>
		/// <param name="token">取消通知</param>
		/// <returns>结果</returns>
		Task<OperateResult> QueryAsync(int index, CancellationToken token = default(CancellationToken));

		/// <summary>
		/// 查询所有
		/// </summary>
		/// <param name="token">取消通知</param>
		/// <returns>结果</returns>
		Task<OperateResult> QueryAsync(CancellationToken token = default(CancellationToken));
	}
}
namespace Snet.Yolo.Server.handler
{
	/// <summary>
	/// 姿态估计自定义关键点颜色处理程序
	/// </summary>
	public class PoseEstimationCustomKeyPointColorHandler
	{
		/// <summary>
		/// 关键点类型（KeyPointType）
		/// </summary>
		/// <remarks>
		/// 此枚举定义了常见的人体解剖关键点类型，必须与训练模型输出中的关键点顺序保持完全一致，<br />
		/// 否则在推理或渲染过程中会出现关键点错位的问题。<br />
		/// <br />
		/// 通常用于人体姿态估计（Pose Estimation）或动作识别中，枚举值按照人体结构从上到下、由中线到两侧的顺序排列。<br />
		/// <br />
		/// 示例：<br />
		/// - Nose（鼻子）作为人体中心参考点<br />
		/// - Eyes / Ears（眼睛 / 耳朵）常用于头部方向估计<br />
		/// - Shoulders / Elbows / Wrists（肩、肘、腕）用于上肢姿态跟踪<br />
		/// - Hips / Knees / Ankles（髋、膝、踝）用于下肢姿态与运动分析
		/// </remarks>
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public enum KeyPointType
		{
			/// <summary>鼻子</summary>
			Nose,
			/// <summary>左眼</summary>
			LeftEye,
			/// <summary>右眼</summary>
			RightEye,
			/// <summary>左耳</summary>
			LeftEar,
			/// <summary>右耳</summary>
			RightEar,
			/// <summary>左肩</summary>
			LeftShoulder,
			/// <summary>右肩</summary>
			RightShoulder,
			/// <summary>左肘</summary>
			LeftElbow,
			/// <summary>右肘</summary>
			RightElbow,
			/// <summary>左手腕</summary>
			LeftWrist,
			/// <summary>右手腕</summary>
			RightWrist,
			/// <summary>左髋部</summary>
			LeftHip,
			/// <summary>右髋部</summary>
			RightHip,
			/// <summary>左膝</summary>
			LeftKnee,
			/// <summary>右膝</summary>
			RightKnee,
			/// <summary>左脚踝</summary>
			LeftAnkle,
			/// <summary>右脚踝</summary>
			RightAnkle
		}

		/// <summary>
		/// 关键点颜色类型（KeyPointColor）
		/// </summary>
		/// <remarks>
		/// 此枚举用于标识在可视化时关键点的显示颜色，颜色值通常以十六进制形式表示，<br />
		/// 例如 "Green" 对应 "#00FF00"。<br />
		/// <br />
		/// 这些颜色可用于：<br />
		/// - 区分不同类型的关键点（如头部、上肢、下肢）<br />
		/// - 增强人体姿态渲染的可读性<br />
		/// - 便于在调试或展示时快速识别关键点分组<br />
		/// </remarks>
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public enum KeyPointColor
		{
			/// <summary>绿色 (#00FF00)，常用于头部关键点</summary>
			Green,
			/// <summary>浅蓝色 (#ADD8E6)，常用于上肢关键点</summary>
			LightBlue,
			/// <summary>黄色 (#FFFF00)，常用于躯干关键点</summary>
			Yellow,
			/// <summary>亮粉色 (#FF69B4)，常用于下肢关键点</summary>
			HotPink
		}

		/// <summary>
		/// 返回一组代表解剖学标志的关键点标记，每种关键点类型都可以有可选的自定义颜色。
		/// </summary>
		/// <remarks>
		/// 返回的数组包含常见解剖学关键点的标记，例如鼻子、眼睛、耳朵、肩膀、肘部、手腕、髋部、膝盖和脚踝。  
		/// 如果未提供 colors 参数，将为每种关键点类型应用一组预定义的颜色。  
		/// </remarks>
		/// <param name="colors">
		/// 一个可选的字典，用于将每个关键点类型映射到其颜色值的字符串表示。  
		/// 如果为 null，则所有关键点都会使用默认颜色。  
		/// </param>
		/// <returns>
		/// 一个 KeyPointMarker 对象数组，每个对象描述一个关键点及其相关的颜色和连接信息。  
		/// </returns>
		public KeyPointMarker[] GetKeyPoints(Dictionary<KeyPointColor, string>? colors = null)
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Expected O, but got Unknown
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Expected O, but got Unknown
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Expected O, but got Unknown
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Expected O, but got Unknown
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Expected O, but got Unknown
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Expected O, but got Unknown
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Expected O, but got Unknown
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Expected O, but got Unknown
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Expected O, but got Unknown
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Expected O, but got Unknown
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Expected O, but got Unknown
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Expected O, but got Unknown
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Expected O, but got Unknown
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Expected O, but got Unknown
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Expected O, but got Unknown
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Expected O, but got Unknown
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Expected O, but got Unknown
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Expected O, but got Unknown
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Expected O, but got Unknown
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Expected O, but got Unknown
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Expected O, but got Unknown
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Expected O, but got Unknown
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Expected O, but got Unknown
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Expected O, but got Unknown
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Expected O, but got Unknown
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Expected O, but got Unknown
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Expected O, but got Unknown
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Expected O, but got Unknown
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Expected O, but got Unknown
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Expected O, but got Unknown
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Expected O, but got Unknown
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Expected O, but got Unknown
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Expected O, but got Unknown
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Expected O, but got Unknown
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Expected O, but got Unknown
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Expected O, but got Unknown
			if (colors == null)
			{
				colors = new Dictionary<KeyPointColor, string>
				{
					{
						KeyPointColor.Green,
						"#A2FF33"
					},
					{
						KeyPointColor.LightBlue,
						"#33ACFF"
					},
					{
						KeyPointColor.Yellow,
						"#FFF633"
					},
					{
						KeyPointColor.HotPink,
						"#FF33AC"
					}
				};
			}
			KeyPointMarker[] array = new KeyPointMarker[17];
			KeyPointMarker val = new KeyPointMarker();
			val.set_Color(colors[KeyPointColor.Green]);
			val.set_Connections((KeyPointConnection[])(object)new KeyPointConnection[2]
			{
				new KeyPointConnection(1, colors[KeyPointColor.Green]),
				new KeyPointConnection(2, colors[KeyPointColor.Green])
			});
			array[0] = val;
			val = new KeyPointMarker();
			val.set_Color(colors[KeyPointColor.Green]);
			val.set_Connections((KeyPointConnection[])(object)new KeyPointConnection[1]
			{
				new KeyPointConnection(2, colors[KeyPointColor.Green])
			});
			array[1] = val;
			KeyPointMarker val2 = new KeyPointMarker();
			val2.set_Color(colors[KeyPointColor.Green]);
			array[2] = val2;
			val = new KeyPointMarker();
			val.set_Color(colors[KeyPointColor.Green]);
			val.set_Connections((KeyPointConnection[])(object)new KeyPointConnection[2]
			{
				new KeyPointConnection(1, colors[KeyPointColor.Green]),
				new KeyPointConnection(5, colors[KeyPointColor.Green])
			});
			array[3] = val;
			val = new KeyPointMarker();
			val.set_Color(colors[KeyPointColor.Green]);
			val.set_Connections((KeyPointConnection[])(object)new KeyPointConnection[2]
			{
				new KeyPointConnection(2, colors[KeyPointColor.Green]),
				new KeyPointConnection(6, colors[KeyPointColor.Green])
			});
			array[4] = val;
			val = new KeyPointMarker();
			val.set_Color(colors[KeyPointColor.LightBlue]);
			val.set_Connections((KeyPointConnection[])(object)new KeyPointConnection[3]
			{
				new KeyPointConnection(6, colors[KeyPointColor.LightBlue]),
				new KeyPointConnection(7, colors[KeyPointColor.LightBlue]),
				new KeyPointConnection(11, colors[KeyPointColor.HotPink])
			});
			array[5] = val;
			val = new KeyPointMarker();
			val.set_Color(colors[KeyPointColor.LightBlue]);
			val.set_Connections((KeyPointConnection[])(object)new KeyPointConnection[2]
			{
				new KeyPointConnection(8, colors[KeyPointColor.LightBlue]),
				new KeyPointConnection(12, colors[KeyPointColor.HotPink])
			});
			array[6] = val;
			val = new KeyPointMarker();
			val.set_Color(colors[KeyPointColor.LightBlue]);
			val.set_Connections((KeyPointConnection[])(object)new KeyPointConnection[1]
			{
				new KeyPointConnection(9, colors[KeyPointColor.LightBlue])
			});
			array[7] = val;
			val = new KeyPointMarker();
			val.set_Color(colors[KeyPointColor.LightBlue]);
			val.set_Connections((KeyPointConnection[])(object)new KeyPointConnection[1]
			{
				new KeyPointConnection(10, colors[KeyPointColor.LightBlue])
			});
			array[8] = val;
			KeyPointMarker val3 = new KeyPointMarker();
			val3.set_Color(colors[KeyPointColor.LightBlue]);
			array[9] = val3;
			KeyPointMarker val4 = new KeyPointMarker();
			val4.set_Color(colors[KeyPointColor.LightBlue]);
			array[10] = val4;
			val = new KeyPointMarker();
			val.set_Color(colors[KeyPointColor.Yellow]);
			val.set_Connections((KeyPointConnection[])(object)new KeyPointConnection[2]
			{
				new KeyPointConnection(12, colors[KeyPointColor.HotPink]),
				new KeyPointConnection(13, colors[KeyPointColor.Yellow])
			});
			array[11] = val;
			val = new KeyPointMarker();
			val.set_Color(colors[KeyPointColor.Yellow]);
			val.set_Connections((KeyPointConnection[])(object)new KeyPointConnection[1]
			{
				new KeyPointConnection(14, colors[KeyPointColor.Yellow])
			});
			array[12] = val;
			val = new KeyPointMarker();
			val.set_Color(colors[KeyPointColor.Yellow]);
			val.set_Connections((KeyPointConnection[])(object)new KeyPointConnection[1]
			{
				new KeyPointConnection(15, colors[KeyPointColor.Yellow])
			});
			array[13] = val;
			val = new KeyPointMarker();
			val.set_Color(colors[KeyPointColor.Yellow]);
			val.set_Connections((KeyPointConnection[])(object)new KeyPointConnection[1]
			{
				new KeyPointConnection(16, colors[KeyPointColor.Yellow])
			});
			array[14] = val;
			KeyPointMarker val5 = new KeyPointMarker();
			val5.set_Color(colors[KeyPointColor.Yellow]);
			array[15] = val5;
			KeyPointMarker val6 = new KeyPointMarker();
			val6.set_Color(colors[KeyPointColor.Yellow]);
			array[16] = val6;
			return (KeyPointMarker[])(object)array;
		}
	}
	/// <summary>
	/// 视觉识别公共工具类，提供默认配置常量、文件大小格式化等功能。
	/// </summary>
	public static class PublicHandler
	{
		/// <summary>
		/// 默认SN
		/// </summary>
		public static readonly string DefaultSN = "Snet.Yolo";

		/// <summary>
		/// 默认路径
		/// </summary>
		public static readonly string DefaultPath = Path.Combine(AppContext.BaseDirectory, "wwwroot");

		/// <summary>
		/// 默认数据库名称
		/// </summary>
		public static readonly string DefaultDBName = "manage.db";

		/// <summary>
		/// 自动转换文件大小单位，保留两位小数
		/// </summary>
		/// <param name="bytes">字节</param>
		/// <returns>大小</returns>
		public static string GetFileSize(this long bytes)
		{
			if (bytes >= 1073741824)
			{
				return ((double)bytes / 1073741824.0).ToString("F2") + " GB";
			}
			if (bytes >= 1048576)
			{
				return ((double)bytes / 1048576.0).ToString("F2") + " MB";
			}
			if (bytes >= 1024)
			{
				return ((double)bytes / 1024.0).ToString("F2") + " KB";
			}
			return bytes + " B";
		}
	}
	/// <summary>
	/// 结果处理
	/// </summary>
	public static class ResultHandler
	{
		/// <summary>
		/// 获取分类结果
		/// </summary>
		/// <param name="result">统一结果</param>
		/// <returns>指定结果</returns>
		public static List<ClassificationResultData>? GetClassificationResult(this OperateResult result)
		{
			List<ClassificationResultData> result2 = default(List<ClassificationResultData>);
			if (!((ResultModel)result).GetDetails<List<ClassificationResultData>>(ref result2))
			{
				return null;
			}
			return result2;
		}

		/// <summary>
		/// 获取定向检测结果
		/// </summary>
		/// <param name="result">统一结果</param>
		/// <returns>指定结果</returns>
		public static List<ObbDetectionResultData>? GetOBBDetectionResult(this OperateResult result)
		{
			List<ObbDetectionResultData> result2 = default(List<ObbDetectionResultData>);
			if (!((ResultModel)result).GetDetails<List<ObbDetectionResultData>>(ref result2))
			{
				return null;
			}
			return result2;
		}

		/// <summary>
		/// 获取姿态结果
		/// </summary>
		/// <param name="result">统一结果</param>
		/// <returns>指定结果</returns>
		public static List<PoseEstimationResultData>? GetPoseEstimationResult(this OperateResult result)
		{
			List<PoseEstimationResultData> result2 = default(List<PoseEstimationResultData>);
			if (!((ResultModel)result).GetDetails<List<PoseEstimationResultData>>(ref result2))
			{
				return null;
			}
			return result2;
		}

		/// <summary>
		/// 获取分割结果
		/// </summary>
		/// <param name="result">统一结果</param>
		/// <returns>指定结果</returns>
		public static List<SegmentationResultData>? GetSegmentationResult(this OperateResult result)
		{
			List<SegmentationResultData> result2 = default(List<SegmentationResultData>);
			if (!((ResultModel)result).GetDetails<List<SegmentationResultData>>(ref result2))
			{
				return null;
			}
			return result2;
		}

		/// <summary>
		/// 获取检测结果
		/// </summary>
		/// <param name="result">统一结果</param>
		/// <returns>指定结果</returns>
		public static List<ObjectDetectionResultData>? GetObjectDetectionResult(this OperateResult result)
		{
			List<ObjectDetectionResultData> result2 = default(List<ObjectDetectionResultData>);
			if (!((ResultModel)result).GetDetails<List<ObjectDetectionResultData>>(ref result2))
			{
				return null;
			}
			return result2;
		}

		/// <summary>
		/// 转换成分类结果
		/// </summary>
		/// <param name="resultDatas">结果数据</param>
		/// <returns>数据集合</returns>
		public static List<Classification> ToClassification(this List<ClassificationResultData> resultDatas)
		{
			return ((IEnumerable<ClassificationResultData>)resultDatas).Select((Func<ClassificationResultData, Classification>)delegate(ClassificationResultData s)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Expected O, but got Unknown
				return new Classification
				{
					Label = s.Label,
					Confidence = s.Confidence
				};
			}).ToList();
		}

		/// <summary>
		/// 转换成定向检测结果
		/// </summary>
		/// <param name="resultDatas">结果数据</param>
		/// <returns>数据集合</returns>
		public static List<OBBDetection> ToObbDetection(this List<ObbDetectionResultData> resultDatas)
		{
			return ((IEnumerable<ObbDetectionResultData>)resultDatas).Select((Func<ObbDetectionResultData, OBBDetection>)delegate(ObbDetectionResultData s)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Expected O, but got Unknown
				OBBDetection val = new OBBDetection();
				val.set_Label(s.Label);
				val.set_Confidence(s.Confidence);
				val.set_BoundingBox(s.BoundingBox);
				val.OrientationAngle = s.OrientationAngle;
				return val;
			}).ToList();
		}

		/// <summary>
		/// 转换成检测结果
		/// </summary>
		/// <param name="resultDatas">结果数据</param>
		/// <returns>数据集合</returns>
		public static List<ObjectDetection> ToObjectDetection(this List<ObjectDetectionResultData> resultDatas)
		{
			return ((IEnumerable<ObjectDetectionResultData>)resultDatas).Select((Func<ObjectDetectionResultData, ObjectDetection>)delegate(ObjectDetectionResultData s)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Expected O, but got Unknown
				ObjectDetection val = new ObjectDetection();
				val.set_Label(s.Label);
				val.set_Confidence(s.Confidence);
				val.set_BoundingBox(s.BoundingBox);
				return val;
			}).ToList();
		}

		/// <summary>
		/// 转换成姿态结果
		/// </summary>
		/// <param name="resultDatas">结果数据</param>
		/// <returns>数据集合</returns>
		public static List<PoseEstimation> ToPoseEstimation(this List<PoseEstimationResultData> resultDatas)
		{
			return ((IEnumerable<PoseEstimationResultData>)resultDatas).Select((Func<PoseEstimationResultData, PoseEstimation>)delegate(PoseEstimationResultData s)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Expected O, but got Unknown
				PoseEstimation val = new PoseEstimation();
				val.set_Label(s.Label);
				val.set_Confidence(s.Confidence);
				val.set_BoundingBox(s.BoundingBox);
				val.KeyPoints = s.KeyPoints;
				return val;
			}).ToList();
		}

		/// <summary>
		/// 转换成分割结果
		/// </summary>
		/// <param name="resultDatas">结果数据</param>
		/// <returns>数据集合</returns>
		public static List<Segmentation> ToSegmentation(this List<SegmentationResultData> resultDatas)
		{
			return ((IEnumerable<SegmentationResultData>)resultDatas).Select((Func<SegmentationResultData, Segmentation>)delegate(SegmentationResultData s)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Expected O, but got Unknown
				Segmentation val = new Segmentation();
				val.set_Label(s.Label);
				val.set_Confidence(s.Confidence);
				val.set_BoundingBox(s.BoundingBox);
				val.BitPackedPixelMask = s.BitPackedPixelMask;
				return val;
			}).ToList();
		}

		/// <summary>
		/// 转换成分类结果
		/// </summary>
		/// <param name="resultDatas">结果数据</param>
		/// <returns>数据集合</returns>
		public static List<ClassificationResultData> ToClassificationResultData(this List<Classification> resultDatas)
		{
			return resultDatas.Select((Classification s) => new ClassificationResultData
			{
				Label = s.Label,
				Confidence = s.Confidence
			}).ToList();
		}

		/// <summary>
		/// 转换成定向检测结果
		/// </summary>
		/// <param name="resultDatas">结果数据</param>
		/// <returns>数据集合</returns>
		public static List<ObbDetectionResultData> ToObbDetectionResultData(this List<OBBDetection> resultDatas)
		{
			return resultDatas.Select(delegate(OBBDetection s)
			{
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				return new ObbDetectionResultData
				{
					Label = s.Label,
					Confidence = s.Confidence,
					BoundingBox = s.BoundingBox,
					OrientationAngle = s.OrientationAngle,
					Position = ((object)s.BoundingBox/*cast due to constrained. prefix*/).ToString()
				};
			}).ToList();
		}

		/// <summary>
		/// 转换成检测结果
		/// </summary>
		/// <param name="resultDatas">结果数据</param>
		/// <returns>数据集合</returns>
		public static List<ObjectDetectionResultData> ToObjectDetectionResultData(this List<ObjectDetection> resultDatas)
		{
			return resultDatas.Select(delegate(ObjectDetection s)
			{
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				return new ObjectDetectionResultData
				{
					Label = s.Label,
					Confidence = s.Confidence,
					BoundingBox = s.BoundingBox,
					Position = ((object)s.BoundingBox/*cast due to constrained. prefix*/).ToString()
				};
			}).ToList();
		}

		/// <summary>
		/// 转换成姿态结果
		/// </summary>
		/// <param name="resultDatas">结果数据</param>
		/// <returns>数据集合</returns>
		public static List<PoseEstimationResultData> ToPoseEstimationResultData(this List<PoseEstimation> resultDatas)
		{
			return resultDatas.Select(delegate(PoseEstimation s)
			{
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				return new PoseEstimationResultData
				{
					Label = s.Label,
					Confidence = s.Confidence,
					BoundingBox = s.BoundingBox,
					KeyPoints = s.KeyPoints,
					Position = ((object)s.BoundingBox/*cast due to constrained. prefix*/).ToString()
				};
			}).ToList();
		}

		/// <summary>
		/// 转换成分割结果
		/// </summary>
		/// <param name="resultDatas">结果数据</param>
		/// <returns>数据集合</returns>
		public static List<SegmentationResultData> ToSegmentationResultData(this List<Segmentation> resultDatas)
		{
			return resultDatas.Select(delegate(Segmentation s)
			{
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				return new SegmentationResultData
				{
					Label = s.Label,
					Confidence = s.Confidence,
					BoundingBox = s.BoundingBox,
					BitPackedPixelMask = s.BitPackedPixelMask,
					Position = ((object)s.BoundingBox/*cast due to constrained. prefix*/).ToString()
				};
			}).ToList();
		}
	}
}
