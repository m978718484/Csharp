using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.IO;
namespace ProxyTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string content = GetContent("https://www.douban.com/group/explore/culture");
            Regex re = null;
            string strTotalPage = "data-total-page=\"(\\d+)\"";
            int intTotalPate = 0;
            re = new Regex(strTotalPage);
            if (re.IsMatch(content))
            {
                int.TryParse(re.Match(content).Groups[1].Value, out intTotalPate);
            }
            for (int i = 0; i < intTotalPate; i++)
            {
                string url = string.Format("https://www.douban.com/group/explore/culture?start={0}", i * 30);
                re = new Regex("<span class=\"from\">.+(https://www.douban.com/group/.+/\").+</span>");
                content = GetContent(url);
                if (re.IsMatch(content))
                {
                    foreach (Match item in re.Matches(content))
                    {
                        string groupName = item.Groups[1].Value.Replace("https://www.douban.com/group/", "").Replace("/\"", "");
                        using (TextWriter tw = File.CreateText(string.Format("./group/{0}", groupName)))
                        { tw.WriteLineAsync(item.Value); }
                    }
                }
                else
                {
                    Console.WriteLine("error content");
                }
                Console.WriteLine(url);
            }
            Console.ReadKey();
        }

        static string GetContent(string url)
        {
            string[] agent = new string[] {
                "Mozilla/5.0 (Macintosh; U; Intel Mac OS X 10_6_8; en-us) AppleWebKit/534.50 (KHTML, like Gecko) Version/5.1 Safari/534.50",
                "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-us) AppleWebKit/534.50 (KHTML, like Gecko) Version/5.1 Safari/534.50",
                "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0",
                "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; .NET4.0C; .NET4.0E; .NET CLR 2.0.50727; .NET CLR 3.0.30729; .NET CLR 3.5.30729; InfoPath.3; rv:11.0) like Gecko",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.6; rv:2.0.1) Gecko/20100101 Firefox/4.0.1",
                "Mozilla/5.0 (Windows NT 6.1; rv:2.0.1) Gecko/20100101 Firefox/4.0.1",
                "Opera/9.80 (Macintosh; Intel Mac OS X 10.6.8; U; en) Presto/2.8.131 Version/11.11",
                "Opera/9.80 (Windows NT 6.1; U; en) Presto/2.8.131 Version/11.11",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_7_0) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.56 Safari/535.11",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; Maxthon 2.0)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; TencentTraveler 4.0)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; The World)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; Trident/4.0; SE 2.X MetaSr 1.0; SE 2.X MetaSr 1.0; .NET CLR 2.0.50727; SE 2.X MetaSr 1.0)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; 360SE)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; Avant Browser)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)"
            };
            string proxyUri = string.Format("{0}:{1}", "http://0.0.0.0", "0000");
            NetworkCredential proxyCreds = new NetworkCredential("0000", "00000");
            WebProxy proxy = new WebProxy(proxyUri, false)
            {
                UseDefaultCredentials = false,
                Credentials = proxyCreds,
            };
            // Now create a client handler which uses that proxy
            HttpClientHandler httpClientHandler = new HttpClientHandler()
            {
                Proxy = proxy,
                PreAuthenticate = true,
                UseDefaultCredentials = false,
            };
            // You only need this part if the server wants a username and password:
            //string httpUserName = "F3220575", httpPassword = "weiqian123!";
            //httpClientHandler.Credentials = new NetworkCredential(httpUserName, httpPassword);
            Random r = new Random();
            int index = r.Next(0, agent.Length - 1);
            HttpClient client = new HttpClient(httpClientHandler);
            client.DefaultRequestHeaders.Add("User-Agent", agent[index]);
            System.Threading.Thread.Sleep(1000*5);
            HttpResponseMessage message = client.GetAsync(url).Result;
            if (!message.IsSuccessStatusCode)
            {
                throw new Exception("Internal server error, code: " + message.StatusCode);
            }
            return message.Content.ReadAsStringAsync().Result;
        }
    }
}
