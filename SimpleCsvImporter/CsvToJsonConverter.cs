#:sdk Microsoft.NET.Sdk
#:package CsvHelper@33.1.0
#:property TargetFramework=net10.0
#:property RollForward=Major
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;

namespace SimpleCsvImporter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("CSV to JSON Converter");
            Console.WriteLine("====================");

            try
            {
                // 定义文件路径
                string csvFilePath = "d:\\Trae\\vsa\\SimpleCsvImporter\\simple_test.csv";
                string jsonFilePath = "d:\\Trae\\vsa\\SimpleCsvImporter\\output.json";

                Console.WriteLine($"读取CSV文件: {csvFilePath}");

                // 读取CSV文件
                var questions = ReadCsvFile(csvFilePath);

                Console.WriteLine($"成功读取 {questions.Count} 条记录");

                // 转换为JSON格式
                Console.WriteLine("转换为JSON格式...");
                var jsonData = ConvertToJson(questions);

                // 保存JSON文件
                Console.WriteLine($"保存JSON文件: {jsonFilePath}");
                SaveJsonFile(jsonData, jsonFilePath);

                Console.WriteLine("\n✅ 转换完成！");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ 转换失败: {ex.Message}");
            }

            Console.WriteLine("\n按任意键退出...");
            Console.ReadKey();
        }

        /// <summary>
        /// 读取CSV文件
        /// </summary>
        private static List<QuestionAnswerModel> ReadCsvFile(string filePath)
        {
            var questions = new List<QuestionAnswerModel>();

            // 尝试多种编码读取文件，解决中文乱码问题
            StreamReader reader = null;

            // 先尝试UTF-8
            try
            {
                reader = new StreamReader(filePath, System.Text.Encoding.UTF8);
                // 读取并测试是否有乱码
                var content = reader.ReadToEnd();
                if (content.Contains('�'))
                {
                    reader.Dispose();
                    reader = null;
                }
                else
                {
                    // UTF-8编码有效，重置流位置
                    reader = new StreamReader(filePath, System.Text.Encoding.UTF8);
                }
            }
            catch { }

            // 如果UTF-8失败，尝试GB2312
            if (reader == null)
            {
                try
                {
                    reader = new StreamReader(filePath, System.Text.Encoding.GetEncoding("GB2312"));
                }
                catch { }
            }

            // 如果GB2312也失败，尝试GBK
            if (reader == null)
            {
                try
                {
                    reader = new StreamReader(filePath, System.Text.Encoding.GetEncoding("GBK"));
                }
                catch { }
            }

            // 如果所有编码都失败，使用默认编码
            if (reader == null)
            {
                reader = new StreamReader(filePath);
            }

            using (var csv = new CsvReader(reader, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null, // 忽略缺失的字段
                BadDataFound = null, // 忽略格式错误的数据
                TrimOptions = TrimOptions.Trim // 去除字段值前后的空格
            }))
            {
                // 检查CSV文件的列头
                csv.Read();
                csv.ReadHeader();
                var headers = csv.HeaderRecord;
                var headerList = headers.Select(h => h.Trim().ToLower()).ToList();

                if (headerList.Contains("序号") && headerList.Contains("类别") && headerList.Contains("人员类别") && headerList.Contains("热点问题") && headerList.Contains("内容"))
                {
                    // 处理格式的CSV文件（序号,类别,人员类别,热点问题,内容）
                    int rowCount = 0;
                    while (csv.Read())
                    {
                        rowCount++;
                        try
                        {
                            var serialNumber = csv.GetField<string>("序号")?.Trim() ?? string.Empty;
                            var category = csv.GetField<string>("类别")?.Trim() ?? string.Empty;
                            var personType = csv.GetField<string>("人员类别")?.Trim() ?? string.Empty;
                            var hotQuestion = csv.GetField<string>("热点问题")?.Trim() ?? string.Empty;
                            var content = csv.GetField<string>("内容")?.Trim() ?? string.Empty;

                            if (!string.IsNullOrWhiteSpace(hotQuestion))
                            {
                                // 组合keywords（类别和人员类别，中间用空格隔开）
                                var keywords = $"{category} {personType}".Trim();

                                questions.Add(new QuestionAnswerModel
                                {
                                    id = serialNumber,
                                    question = hotQuestion,
                                    answer = content,
                                    keywords = keywords,
                                    type = category
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"警告: 读取第 {rowCount} 行数据时发生错误 - {ex.Message}");
                        }
                    }
                }
                else
                {
                    throw new Exception("CSV文件格式不正确，缺少必要的列头");
                }
            }

            return questions;
        }

        /// <summary>
        /// 转换为JSON格式
        /// </summary>
        private static string ConvertToJson(List<QuestionAnswerModel> questions)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true,
                TypeInfoResolver = new System.Text.Json.Serialization.Metadata.DefaultJsonTypeInfoResolver()
            };

            return JsonSerializer.Serialize(questions, options);
        }

        /// <summary>
        /// 保存JSON文件
        /// </summary>
        private static void SaveJsonFile(string jsonData, string filePath)
        {
            File.WriteAllText(filePath, jsonData, System.Text.Encoding.UTF8);
        }
    }

    /// <summary>
    /// 问答模型
    /// </summary>
    public class QuestionAnswerModel
    {
        public string id { get; set; } = string.Empty;
        public string question { get; set; } = string.Empty;
        public string answer { get; set; } = string.Empty;
        public string keywords { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
    }
}