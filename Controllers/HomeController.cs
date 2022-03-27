using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Text;
using XmlImport.Models;

namespace XmlImport.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //Метод с WebClient
        public string DownloadFile()
        {
            string year = "2022";
            List<string> fileNameList = new List<string>();
            List<string> directoryList = new List<string>();
            WebClient client = GetClient();

            ////Извлекаем имена папок
            string contentDirectoryQtr = client.DownloadString($"https://www.sec.gov/Archives/edgar/daily-index/{year}/");
            directoryList = GetDirectoryName(contentDirectoryQtr);

            foreach (string directory in directoryList)
            {
                var localPath = $"D:\\XML\\daily-index\\{year}\\{directory}";
                CreateDirectory(localPath);
                //Из страницы с именами файлов вытаскиваем имена и формируем список
                var dirPath = $"https://www.sec.gov/Archives/edgar/daily-index/{year}/{directory}/";

                client = GetClient();

                //Скачиваем страницу
                string contentFileName = client.DownloadString(dirPath);
                
                //Получаем имена файлов со страницы
                fileNameList = GetFileName(contentFileName);

                foreach (var file in fileNameList)
                {
                    client = GetClient();
                    var fullpath = dirPath + file;
                    var localFullPath = $"{localPath}\\{file}";
                    client.DownloadFile(fullpath, localFullPath);
                    ParseFile(localFullPath);
                }
            }

            return ("Все файлы скачаны!");


        }

        /// <summary>
        /// Считываем скачанный файл и парсим его
        /// складываем его в список типа Master
        /// </summary>
        /// <param name="path"></param>
        private  void ParseFile(string path)
        {
            List<Master> list = new List<Master>();     

            using (StreamReader reader = new StreamReader(path))
            {
                string sub = "-----";               
                string text =  reader.ReadToEnd();
                int indexOfSubstring = text.LastIndexOf(sub);
                string[] txtArray = text.Substring(indexOfSubstring).Replace("-----", "").Trim().Split(new char[] { '\r','\n' }, StringSplitOptions.RemoveEmptyEntries);         

                foreach (string s in txtArray)
                {
                    string[] lineArr = s.Split("|");
                    list.Add(new Master(int.Parse(lineArr[0]), lineArr[1], lineArr[2], lineArr[3], lineArr[4]));
                }               
            }
            Console.WriteLine($"Файл {path} обработан.");
        }


        /// <summary>
        /// Создаем папки на локальной машине
        /// </summary>
        /// <param name="path"></param>
        private void CreateDirectory(string path)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
        }

        /// <summary>
        /// Возвращаем клиент с нужными хейдерами
        /// </summary>
        /// <returns></returns>
        private WebClient GetClient()
        {
            WebClient client = new WebClient();
            client.Headers.Add("accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            client.Headers.Add(@"cookie:_ga=GA1.2.521904397.1646656948; _gid=GA1.2.909744332.1646656948; _4c_={""_4c_s_"":""lVJNr9MwEPwrlc9N6 / gjtntDDwlxgAN6wLFKbKexGurIcRseVf87u / 2U + riQS7KzM5P1rI9k6vyOrMpKVEqyinGh5Jxs / dtIVkeSgsPXgayIMsy21rQFNaopBJeiaBomC0Wp1DUTNTeczMlv9JJCcGaEppqd5sQOV48jsdF58CrNohQLU7QjKPIfRDSFzyFFt7d5nd8GpE2 + mY1uCw3nD8H69RRc7qDBmVIPtPNh02U0UUwjPCQsFkxCMYWdi9NdWVaMPtCHUkgBcJPiNHpUv3Qp / vIzYwCNEAX5eVbgvMm3PqUzC6oxZBx19HaxiYcrAPFdsOKC / QiIutnryzfAv + JJMVLMq4 + 27tEBtjAnnz6sv3 / +CJVkpaGCG7XAzVSygjShv089NLuch3G1XE7TtLj + eAlZ + D4OMNYcrNB9SA7H8TaHuLuMA / VrCpuNT1987iLsFuraBWTUPUaHROfbet9nLDEb29fjGKzz4zbHgZyuOxaV5pIzwRjcl5xhLl0Jis / pcrzzyvmdrarSlLQ0 + j37EnuBefnd / 0lh1tsF1b7molVVUVKvC6EEK4yqeEGbymslHLVlTZ48maL / 8NzdLB9J3K61LCWoNNLCkK + 884qUhI5g / IkLCHLvjs9e2D + d / gI = ""}; bm_sv=E09EE8FEAD3A4B80201605710304B58E~PZa6vYuYqFOoHWCSvsZegoNSa6sorzdn0kS+KIbq8dm9voHwI/lAyR2gulavp3uA/4Yvg2duidvp5jdL+OG+vK2oygULQQhQuoniI36+Tm0ebg9WIyxubsJvJldIzbjDcqNHHzRPZLFvIm/zdJOlvQ==; bm_mi=EA0C90E7B24D92EE936E240B98097EA4~ORoomL2qiz3BaJhMYXVvwO56Gqxwyj8oRqFI1MFeZBIj+Ehlj7wEgoQImlTpafCTE3G1SB8QASK2XyqsSGHCVYtQfUWpzYdjcd14wNFeax4s+Bdx33gi1Q5z8Ir0dhrUvSOVrTzJukWfDZ0GNzMFn/FUY+MPxEtiZSTVbVVPcYgspbodfECYz4B/GVN3kxsnGhogU4aurlwf5RANCoDo1k3uksIinka49CCNtyv2Xt6NvVhfDS9vC1a06zSCEhE1n81IkCtkvmJjqdYi2DJhPuk/AJTHz6Yl+CSJ+9es7zfb4cMYM9m/vFLFrkiKg7ZLaYRbR3Aw3Bn4bvzYZF9uGg==; ak_bmsc=F4BC3609A186CFA07AF6911167FB695C~000000000000000000000000000000~YAAQFOzvUCMxvVF/AQAAfDxMag/1dBPYijAU+OlKf381ipx2PVdiJZkOn9HPgpOGZ9B7xNFoyMRwfSW01R9afrP7xraPrBRlPpnZCZAY+IBwbGj8AYco5jaDFJk1dF6dQTug+cZInkJNzqWajViMNM/0DadqhK0fL6oI5PxGFiAsnDOoWpsHFxR1/6U2SVXjo2qDjDGEqDifjanbXgJQ9gxp5GmNEmDza106f6OmoaQgx1LW1jtmxRHj6/JLOHWdcTu7f1KkLqx+e8bg+3rpQ338fh/DrM7bErnOXSKQjhOdLrkeJIpYrkt2Qiz+hXC74ZDPsJArFKhdkmfbYQKJU5JNvYs7kXIy/3a7d+waY3JRV4si9k5KaBlUfkVhyPUkCcOLNlwngi1thH44ZznEIKug+Xw=");
            client.Headers.Add(@"user-agent:Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.51 Safari/537.36");

            return client;            
        }

        /// <summary>
        /// Вытаскивает из контекста имена файлов master
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private List<string> GetFileName (string content)
        {
            List<string> result = new List<string>();            
            string[] strArray = content.Split('<');
            foreach (string a in strArray)
            {
                if (a.Contains("\"master."))
                {
                    string temp = a.Replace("a href=\"", "").Replace("\">","");
                    result.Add(temp);
                }                    
            }
            return result;
        }

        /// <summary>
        /// Вытаскивает имена подпапок кварталов
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private List<string> GetDirectoryName(string content)
        {
            List<string> result = new List<string>();

            string[] strArray = content.Split('<');
            foreach (string a in strArray)
            {
                if (a.Contains("\"QTR"))
                {
                    string temp = a.Replace("a href=\"", "").Replace("/\">", "");
                    result.Add(temp);
                }
            }

            return result;
        }



    }
}