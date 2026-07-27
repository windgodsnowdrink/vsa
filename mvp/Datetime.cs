#:sdk Microsoft.NET.Sdk.Web
#:sdk Aspire.AppHost.Sdk@9.4.2
#:package Aspire.Hosting.AppHost@9.4.2
#:package System.Threading.Channels@9.0.9
#:package Microsoft.Extensions.ObjectPool@9.0.9
#:package Scalar.AspNetCore@2.8.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using System.Text.Json;

Console.OutputEncoding = System.Text.Encoding.UTF8;
var dt = DateTimeExtensions.ConvertToDateTime("1761211740000");
Console.WriteLine(dt);

public static class DateTimeExtensions
{
    public static DateTime ConvertToDateTime(string timestamp)
    {
        System.DateTime time = System.DateTime.MinValue;
        ConvertToDateTime start = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        try
        {
            time = timestamp.Length == 10 ? start.AddSeconds(long.Parse(timestamp)) : start.AddMilliseconds(long.Parse(timestamp));
        }
        catch (global::System.Exception)
        {
            throw;
        }

        return time;
    }

    public static string ConvertTimestamp(DateTime time)
    {
        double intResult = 0;
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        intResult = (time - startTime).TotalMilliseconds;
        return Math.Round(intResult, 0).ToString();
    }
}