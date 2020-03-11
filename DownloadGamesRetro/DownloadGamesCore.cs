using DownloadGamesRetro.Model;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace DownloadGamesRetro
{
    public class DownloadGamesCore
    {
        #region Propriedades

        const int A = 65;
        const int Z = 90;

        readonly string URL;
        readonly string UrlSaveDownloads;
        readonly bool CriarRelatorioFinal;
        private string BaseUrl = @"https://edgeemu.net/";
        private List<string> lista = new List<string>();
        private List<Jogo> linkJogos = new List<Jogo>();
        private List<string> erros = new List<string>();
        private bool encontrou = false;
        private bool Log;
        private string NomeBase;

        #endregion

        #region Construtores
        public DownloadGamesCore(string urlLista, string urlDiretorioSave, bool criarRelatorio, string nomeBase, bool log = false)
        {
            this.URL = urlLista;
            this.UrlSaveDownloads = urlDiretorioSave;
            this.CriarRelatorioFinal = criarRelatorio;
            this.Log = log;
            this.NomeBase = nomeBase;
        }

        #endregion

        #region Metodos
        public void Executar()
        {
            Uteis.Log("-- -----------------------------------------------------------------", Log);

            LerArquivos();
            RasparLisnksNomes();
            DownloadJogos();
            CriarRelatorioErrosTxt();
            MostrarRelatorioFinal(CriarRelatorioFinal);

            Uteis.Log("-- -----------------------------------------------------------------", Log);
        }

        private void MostrarRelatorioFinal(bool criarRelatorioFinal)
        {
            Uteis.Log("", Log);
            Uteis.Log("", Log);
            Uteis.Log($"            ### Processo Finalizado [{NomeBase}] ###", Log);
            Uteis.Log($"[{NomeBase}] Diretorio com Downloads:........................... {UrlSaveDownloads + lista[0]}", Log);
            Uteis.Log($"[{NomeBase}] Logs:.............................................. {UrlSaveDownloads + @"log\"}", Log);
            Uteis.Log($"[{NomeBase}] Total de nomes de jogos para Download {lista[0]}:.. {lista.Count - 2}", Log);
            Uteis.Log($"[{NomeBase}] Total de Links coletados do emulador {lista[0]}:... {linkJogos.Count}", Log);
            Uteis.Log($"[{NomeBase}] Total de Erros:.................................... {erros.Count}", Log);

            if (criarRelatorioFinal)
            {
                CriarRelatorioFinalTxt();
                Uteis.Log($"[{NomeBase}] Relatorio Final:................................... {UrlSaveDownloads}log/relatorio-{lista[0]}.txt", Log);
            }

            Uteis.Log($"                     ### FIM ###", Log);
        }

        private void CriarRelatorioFinalTxt()
        {
            Uteis.ValidaDiretorio($"{UrlSaveDownloads}log/");

            using (var file = new StreamWriter($"{UrlSaveDownloads}log/relatorio-{lista[0]}.txt"))
            {
                file.WriteLine($"          ###  Processo Finalizado [{NomeBase}]  ###");
                file.WriteLine($"[{NomeBase}] Diretorio com Downloads:........................... {UrlSaveDownloads + lista[0]}");
                file.WriteLine($"[{NomeBase}] Logs:.............................................. {UrlSaveDownloads + @"log\"}");
                file.WriteLine($"[{NomeBase}] Total de nomes de jogos para Download {lista[0]}:.. {lista.Count - 2}");
                file.WriteLine($"[{NomeBase}] Total de Links coletados do emulador {lista[0]}:... {linkJogos.Count}");
                file.WriteLine($"[{NomeBase}] Total de Erros:.................................... {erros.Count}");
                file.WriteLine($"                  ### FIM ###");

            }
        }

        private void CriarRelatorioErrosTxt()
        {
            Uteis.Log("", Log);
            Uteis.Log($"[{NomeBase}] Gerando logs...", Log);

            Uteis.ValidaDiretorio($"{UrlSaveDownloads}log/");

            using (var file = new StreamWriter($"{UrlSaveDownloads}log/{lista[0]}-erros.txt"))
            {
                foreach (var err in erros)
                {
                    file.WriteLine(err);
                }
            }
        }

        private void DownloadJogos()
        {
            Uteis.Log("", Log);
            Uteis.Log($"[{NomeBase}] Iniciando Download dos jogos...", Log);
            Uteis.Log("", Log);

            Uteis.ValidaDiretorio(UrlSaveDownloads + lista[0]);

            for (int i = 2; i < lista.Count; i++)
            {
                foreach (var jo in linkJogos)
                {
                    if (jo.Nome.Contains(lista[i]))
                    {
                        encontrou = true;
                        using (WebClient client = new WebClient())
                        {
                            client.DownloadFile(jo.Url, $"{UrlSaveDownloads}{lista[0]}/{lista[i]}.7z");

                            Uteis.Log($"{lista[i] + ".7z"}... ", Log);
                        }
                        break;
                    }
                }

                if (!encontrou)
                {
                    erros.Add(lista[i]);
                }
                encontrou = false;
            }
        }
        private void RasparLisnksNomes()
        {
            using (WebClient client = new WebClient())
            {
                Uteis.Log("", Log);
                Uteis.Log($"[{NomeBase}] Iniciando raspagem dos Nomes e Links...", Log);

                for (int x = A; x <= Z; x++)
                {
                    string html = client.DownloadString(lista[1].Replace('@', (char)x));

                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(html);

                    var links = htmlDoc.DocumentNode.SelectNodes("//table/tr/td/a");

                    if (links != null)
                    {
                        foreach (var link in links)
                        {
                            linkJogos.Add(new Jogo()
                            {
                                Url = BaseUrl + link.Attributes["href"].Value,
                                Nome = link.InnerHtml.LimpaString()
                            });
                        }

                        Uteis.Log($"{(char)x}.. {linkJogos.Count} Links coletados", Log);
                    }
                    else
                    {
                        Uteis.Log($"{(char)x}.. NÃO HA DADOS", Log);
                    }
                }
            }
        }

        private void LerArquivos()
        {
            Uteis.Log($"[{NomeBase}] Lendo lista...", Log);

            using (var file = new StreamReader(URL))
            {
                lista = file.ReadToEnd().Split(';').ToList();
            }

            Uteis.Log($"[{NomeBase}] Nome: {lista[0]} URL: {lista[1]} - {lista.Count} Nomes lidos...", Log);
        }

        #endregion
    }
}