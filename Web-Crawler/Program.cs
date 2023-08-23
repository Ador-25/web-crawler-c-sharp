using System;
using System.Collections.Generic;
using System.Net.Http;
using HtmlAgilityPack;

namespace Web_Crawler
{
    class Program
    {
        static HashSet<string> visitedUrls = new HashSet<string>();

        static async System.Threading.Tasks.Task Main(string[] args)
        {
            string rootUrl = "http://www.northsouth.edu/";
            int maxDepth = 2;

            await DepthFirstSearch(rootUrl, maxDepth, 0);

            Console.WriteLine("Visited URLs:");
            foreach (var url in visitedUrls)
            {
                Console.WriteLine(url);
            }
        }

        static async System.Threading.Tasks.Task DepthFirstSearch(string url, int maxDepth, int currentDepth)
        {
            if (currentDepth > maxDepth || visitedUrls.Contains(url))
            {
                return;
            }

            visitedUrls.Add(url);

            try
            {
                var httpClient = new HttpClient();
                var html = await httpClient.GetStringAsync(url);
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                // Process the HTML document as needed
                // For example, you can extract links and crawl them recursively

                var links = htmlDocument.DocumentNode.SelectNodes("//a[@href]");
                if (links != null)
                {
                    foreach (var link in links)
                    {
                        var linkUrl = link.GetAttributeValue("href", "");
                        Console.WriteLine(linkUrl);
                        if (!string.IsNullOrEmpty(linkUrl))
                        {
                            if (!linkUrl.StartsWith("http"))
                            {
                                linkUrl = new Uri(new Uri(url), linkUrl).AbsoluteUri;
                            }

                            await DepthFirstSearch(linkUrl, maxDepth, currentDepth + 1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error crawling {url}: {ex.Message}");
            }
        }
    }
}
