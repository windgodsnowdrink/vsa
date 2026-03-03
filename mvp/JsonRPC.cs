using System.Text.Json;
#:sdk Microsoft.NET.Sdk.Web
#:sdk Aspire.AppHost.Sdk@9.4.2
#:package Aspire.Hosting.AppHost@9.4.2
#:package System.Threading.Channels@9.0.9
#:package Microsoft.Extensions.ObjectPool@9.0.9
#:package Scalar.AspNetCore@2.8.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

// ���ÿ���̨������壬ȷ��ʵʱͨ��
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine(ConsoleColor.Black); // ��������κη�JSON����

var random = new Random();

// MCP Э��Ҫ�󣺶�ȡ JSON-RPC ����
string? input;
while ((input = Console.ReadLine()) != null)
{
    try
    {
        // ���� JSON-RPC ����
        using var jsonDoc = JsonDocument.Parse(input);
        var root = jsonDoc.RootElement;

        if (root.TryGetProperty("method", out var method))
        {
            switch (method.GetString())
            {
                case "generateRandom":
                    var randomNumber = random.Next(1, 100);

                    // ���� JSON-RPC ��Ӧ
                    var response = new
                    {
                        jsonrpc = "2.0",
                        result = new { number = randomNumber },
                        id = root.GetProperty("id").GetInt32()
                    };

                    Console.WriteLine(JsonSerializer.Serialize(response));
                    break;

                case "initialize":
                    // ��ʼ����Ӧ
                    var initResponse = new
                    {
                        jsonrpc = "2.0",
                        result = new { status = "initialized" },
                        id = root.GetProperty("id").GetInt32()
                    };
                    Console.WriteLine(JsonSerializer.Serialize(initResponse));
                    break;
            }
        }
    }
    catch (Exception ex)
    {
        // ���ش�����Ӧ
        var errorResponse = new
        {
            jsonrpc = "2.0",
            error = new { code = -32600, message = ex.Message },
            id = 0
        };
        Console.WriteLine(JsonSerializer.Serialize(errorResponse));
    }
}