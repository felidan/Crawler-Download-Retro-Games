﻿using System;
using System.IO;

namespace DownloadGamesRetro.Model
{
    public static class Uteis
    {
        public static string LimpaString(this string a)
        {
            a = a.Replace("'", "").ToUpper();
            return a.IndexOf('(') > -1 ? a.Substring(0, a.IndexOf('(')).Trim() : a;
        }

        public static void ValidaDiretorio(string url)
        {
            if (!Directory.Exists(url))
            {
                Directory.CreateDirectory(url);
            }
        }

        public static void Log(string str, bool log = false)
        {
            if (log)
            {
                Console.WriteLine(str);
            }
        }
    }
}
