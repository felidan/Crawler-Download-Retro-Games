using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TopRating
{
    class Program
    {
        static void Main(string[] args)
        {
            string URL = @"https://www.emuparadise.me/Atari_7800_ROMs/47";

            using(WebClient client = new WebClient())
            {
                var html = client.DownloadString(URL);
                
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                var node = htmlDoc.GetElementbyId("list-of-games").ChildNodes;

                if(node != null)
                {
                    foreach(var tag in node)
                    {
                        Console.WriteLine(tag.InnerHtml);
                    }
                }
            }

            Console.ReadKey();
        }
    }
}
