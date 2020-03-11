using DownloadGamesRetro.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace DownloadGamesRetro
{
    class Program
    {
        public static string Urlbase = @"C:\Users\felip\Desktop\games\DownloadGamesRetro\_Resultado\";
        public static string UrlArquivo = Urlbase + "links.txt";
        static void Main(string[] args)
        {
            DateTime dataAtual = DateTime.Now;
            Queue<string> listas = new Queue<string>();

            Uteis.ValidaDiretorio($@"{Urlbase}resultado\");
            
            List<string> listaUrls;

            using (var file = new StreamReader(UrlArquivo))
            {
                listaUrls = file.ReadToEnd().Split(';').ToList();
            }
            
            foreach (var u in listaUrls)
            {
                listas.Enqueue(u);
            }

            while (listas.Any())
            {
                Thread t1 = new Thread(ExecutaProcesso);
                Thread t2 = new Thread(ExecutaProcesso);

                var str_1 = listas.Dequeue();
                bool exec_t2 = false;
                
                Urls ulrs_1 = new Urls()
                {
                    Nome = str_1.Split('|')[0].Replace('\n', ' ').Replace('\r', ' ').Trim(),
                    UrlMelhoresjogos = str_1.Split('|')[1],
                    UrlDownload = str_1.Split('|')[2]
                };

                Uteis.Log("### INICIANDO THREADS..\n", true);

                t1.Start(ulrs_1);
                
                if (listas.Any())
                {
                    var str_2 = listas.Dequeue();
                    exec_t2 = true;

                    Urls ulrs_2 = new Urls()
                    {
                        Nome = str_2.Split('|')[0].Replace('\n', ' ').Replace('\r', ' ').Trim(),
                        UrlMelhoresjogos = str_2.Split('|')[1],
                        UrlDownload = str_2.Split('|')[2]
                    };
                    
                    t2.Start(ulrs_2);
                }
                
                if (exec_t2)
                {
                    t1.Join();
                    t2.Join();
                }
                else
                {
                    t1.Join();
                }

                Uteis.Log("\n### FINALIZANDO THREADS..\n", true);
            }
            
            Console.WriteLine($"Inicio: {dataAtual}, Fim: {DateTime.Now}");
            Console.WriteLine($"Total de tempo: {DateTime.Now - dataAtual}");
            
            Console.WriteLine("###### FIM ######");

            Console.ReadKey();
        }
        
        static void ExecutaProcesso(object urls)
        {
            Urls u = (Urls)urls;

            Uteis.Log($"[{u.Nome}] Inicio da base..", true);

            RaspaMelhoresJogos topGames = new RaspaMelhoresJogos(u.UrlMelhoresjogos, true, u.Nome);

            var listaMelhoresGames = topGames.Executar();

            using (var file = new StreamWriter($@"{Urlbase}resultado\{u.Nome}.txt"))
            {
                file.Write(u.Nome + ";");
                file.Write(u.UrlDownload);

                foreach (var a in listaMelhoresGames)
                {
                    file.Write(";" + a);
                }
            }

            Uteis.Log($"[{u.Nome}] Iniciando Download..", true);

            DownloadGamesCore core = new DownloadGamesCore
                (
                    $@"{Urlbase}resultado\{u.Nome}.txt",
                    $@"{Urlbase}resultado\",
                    true,
                    u.Nome.ToString(), 
                    false
                );

            core.Executar();


            Uteis.Log($"[{u.Nome}] Download concluido..", true);

            Uteis.Log($"[{u.Nome}] Fim da base..", true);
        }
    }
}
