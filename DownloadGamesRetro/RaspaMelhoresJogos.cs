using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using DownloadGamesRetro.Model;

namespace DownloadGamesRetro
{
    public class RaspaMelhoresJogos
    {
        readonly string Url;
        readonly bool Log;
        readonly string NomeBase;
        
        public RaspaMelhoresJogos(string urlLinks, bool log, string nomeBase)
        {
            this.Url = urlLinks;
            this.Log = log;
            this.NomeBase = nomeBase;
        }

        public List<string> Executar()
        {
            List<string> lista = new List<string>();

            Uteis.Log($"[{NomeBase}] Coletando Top Jogos..", Log);

            using (WebClient client = new WebClient())
            {
                var html = client.DownloadString(Url);

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                try
                {
                    var node = htmlDoc.GetElementbyId("list-of-games").ChildNodes;

                    if (node != null)
                    {
                        foreach (var tag in node)
                        {
                            if(!lista.Contains(tag.InnerHtml.LimpaString()))
                                lista.Add(tag.InnerHtml.LimpaString());
                        }
                    }
                }
                catch
                {
                    var node = htmlDoc.DocumentNode.SelectNodes("//table/tr/td/a");

                    if(node != null)
                    {
                        foreach(var tag in node)
                        {
                            if (!lista.Contains(tag.InnerHtml.LimpaString()))
                                lista.Add(tag.InnerHtml.LimpaString());
                        }
                    }
                }

                Uteis.Log($"[{NomeBase}] - {lista.Count} nomes coletados..", Log);
            }
            return lista;
        }
    }
}
