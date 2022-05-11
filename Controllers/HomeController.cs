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
            //    Task task = Task.Run(() => DownloadProcess());
            DownloadProcess();
            return ("Запущено скачивание файлов!");
        }

        private async Task DownloadProcess()
        {
            FileHelper fileHelper = new FileHelper();
            string year = "2022";
            List<string> fileNameList = new List<string>();
            List<string> directoryList = new List<string>();
            List<Master> listMaster = new List<Master>();
            WebClient client = fileHelper.GetClient();
            int counter = 0;

            //////Извлекаем имена папок
            string contentDirectoryQtr = client.DownloadString($"https://www.sec.gov/Archives/edgar/daily-index/{year}/");
            directoryList = fileHelper.GetDirectoryName(contentDirectoryQtr);

            foreach (string directory in directoryList)
            {
                var localPath = $"D:\\XML\\daily-index\\{year}\\{directory}";
                fileHelper.CreateDirectory(localPath);
                //Из страницы с именами файлов вытаскиваем имена и формируем список
                var dirPath = $"https://www.sec.gov/Archives/edgar/daily-index/{year}/{directory}/";

                client = fileHelper.GetClient();

                //Скачиваем страницу
                string contentFileName = client.DownloadString(dirPath);

                //Получаем имена файлов со страницы
                fileNameList = fileHelper.GetFileName(contentFileName);

                foreach (var file in fileNameList)
                {
                    client = fileHelper.GetClient();
                    var fullpath = dirPath + file;
                    var localFullPath = $"{localPath}\\{file}";
                    if (!System.IO.File.Exists(localFullPath))
                    {
                        client.DownloadFile(fullpath, localFullPath);
                    }
                    fileHelper.ParseFile(localFullPath, listMaster);
                }
            }

            fileHelper.SaveDb(listMaster);

            //2 этап считывание из базы URL и скачивание второй партии файлов
            //   var readFileList = fileHelper.ReadFileFromDb();
        }
    }
}