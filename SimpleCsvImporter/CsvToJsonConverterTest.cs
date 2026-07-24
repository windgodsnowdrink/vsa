#load "CsvToJsonConverter.cs"

using System.Reflection;

public class CsvToJsonConverterTest
{
    [Fact]
    public void QuestionAnswerModel_TypeExists()
    {
        var type = typeof(QuestionAnswerModel);
        Assert.NotNull(type);
    }
}