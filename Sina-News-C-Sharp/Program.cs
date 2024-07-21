using System.Collections.Generic;
using System.Drawing;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleApp1
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // API URL
            string apiUrl = "https://zhibo.sina.com.cn/api/zhibo/feed?zhibo_id=152&id=&tag_id=0&page=1&page_size=30&type=0";

            // 创建 HttpClient 实例
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // 发送 GET 请求
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();

                    // 读取响应内容
                    string responseBody = await response.Content.ReadAsStringAsync();

                    JArray jsonObject = (JArray)JObject.Parse(responseBody)["result"]["data"]["feed"]["list"];

                    List<News> newsList = JsonConvert.DeserializeObject<List<News>>(jsonObject.ToString());

                    foreach (var news in newsList)
                    {
                        news.setUpdateTime(news.getUpdateTime().Substring(5));
                    }

                    string allNews = string.Join("", newsList.Select(news => news.ToString()));

                    string currentDirectory = Directory.GetCurrentDirectory();
                    string filePath = Path.Combine(currentDirectory, "sina.html");
                    if (File.Exists(filePath))
                    {
                        string fileContents = File.ReadAllText(filePath);
                        Console.WriteLine(fileContents);
                    }

                    Console.WriteLine("filePath is " + filePath);
                    // 获取 Program.cs 所在的目录
                    // 假设 Program.cs 文件在项目的根目录，而输出文件位于 bin/Debug 或 bin/Release
                    string projectRootPath = Path.GetFullPath(Path.Combine(currentDirectory, @"..\..\.."));

                    //// 定义文件路径，将 index.html 文件写入到 Program.cs 同一级目录                    
                    //string filePath = "sina.html";
                    //Console.WriteLine("filePath is " + filePath);
                    //Console.WriteLine("projectRootPath is " + projectRootPath);

                    string sinaHtml = File.ReadAllText(filePath, Encoding.UTF8);

                    // 替换 ${content} 占位符
                    sinaHtml = sinaHtml.Replace("${content}", allNews);

                    //string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

                    // 获取 Program.cs 所在的目录
                    // 假设 Program.cs 文件在项目的根目录，而输出文件位于 bin/Debug 或 bin/Release
                    //string projectRootPath = Path.GetFullPath(Path.Combine(currentDirectory, @"..\..\.."));

                    // 定义文件路径，将 index.html 文件写入到 Program.cs 同一级目录
                    string indexPath = Path.Combine(projectRootPath, "index.html");

                    // 删除现有文件（如果存在）
                    if (File.Exists(indexPath))
                    {
                        File.Delete(indexPath);
                    }

                    // 创建新文件并写入内容
                    File.WriteAllText(indexPath, sinaHtml, Encoding.UTF8);

                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"请求错误: {e.Message}");
                }
            }
        }
    }

    public class News
    {
        [JsonProperty("rich_text")]
        public String richText { get; set; }
        [JsonProperty("update_time")]
        public String updateTime { get; set; }

        public String getUpdateTime()
        {
            return updateTime;
        }

        public void setUpdateTime(String updateTime)
        {
            this.updateTime = updateTime;
        }

        public override string ToString()
        {
            return "<div class=\"w3-container w3-justify\"><p>" + updateTime + " " + richText.Trim() + "</p></div>";
        }
    }
}