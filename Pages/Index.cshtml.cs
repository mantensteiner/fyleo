using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using fyleo.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using fyleo.EventLog;
using fyleo.Repository.Models;

namespace fyleo.Pages
{
    public class IndexModel : BasePageModel
    {
        public string RootPath { get { return fileRepository.GetRootPath(); } }
        public string Path { get; private set; }                
        public string Trash { get { return Translations.Trash; } }
        public string[] Folders { get; private set; }
        public FileData[] Files { get; private set; }
        private readonly IWebHostEnvironment hostingEnvironment;
        public Activity[] Activities { get; private set; }
        private readonly IFileRepository fileRepository;

        public IndexModel(IWebHostEnvironment hostingEnvironment, IEventLog eventLog, ISiteConfigRepository siteConfigRepo, 
            IFileRepository fileRepository, ITranslations translations)
            : base(eventLog, siteConfigRepo, translations)
        {
            this.fileRepository = fileRepository;
            this.hostingEnvironment = hostingEnvironment;
        }

        public async Task OnGet()
        {
            Path = RootPath;
            await GetLastestActivities();
            GetCurrentFilesAndFolders(Path);
        }

        public IActionResult OnGetIsLoggedIn()
        {
            return new OkResult();
        }

        public async Task<IActionResult>  OnPostAsync(string path, List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var filePath = path + formFile.FileName;

                    // Move existing file to trash for backup version
                    if (System.IO.File.Exists(filePath))
                    {
                        var trashPath = RootPath + "/" + Translations.Trash + "/" + formFile.FileName + "." + DateTime.UtcNow.Ticks.ToString();
                        System.IO.File.Copy(filePath, trashPath);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                        await LogEvent(Actions.UPLOAD_FILE, filePath);
                    }
                }
            }

            await GetLastestActivities();
            GetCurrentFilesAndFolders(path);
            return LocalRedirect(GetCurrentUrl(path));
        }

        public async Task OnGetChangeFolder(string path, string folderName)
        {
            path = Uri.UnescapeDataString(path);
            if (!path.EndsWith("/"))
            {
                path += "/";
            }

            // Do not navigate down from root, keep root path
            if ((path.Equals(RootPath, StringComparison.OrdinalIgnoreCase))
                && string.IsNullOrEmpty(folderName))
            {
                Path = path;
                await GetLastestActivities();
                GetCurrentFilesAndFolders(Path);
                return;
            }
            
            if (!string.IsNullOrEmpty(folderName))
            {
                Path = path + folderName + "/";
            }
            else
            {
                Path = path.Split('/').Take(path.Split('/').Length - 2).Aggregate((c, n) => c + "/" + n) + "/";
            }

            await GetLastestActivities();
            GetCurrentFilesAndFolders(Path);
        }

        public async Task<IActionResult> OnPostFileEdit(string path, string oldFileName, string fileName)
        {
            try
            {
                System.IO.File.Move(path + oldFileName, path + fileName);
            }
            catch (Exception ex)
            {
                return LocalRedirect("/Error?message=" + ex.Message);
            }

            await GetLastestActivities();
            return LocalRedirect(GetCurrentUrl(path));
        }



        public async Task<IActionResult> OnPostFolderEdit(string path, string oldFolderName, string folderName)
        {
            try
            {
                System.IO.Directory.Move(path + oldFolderName, path + folderName);
            }
            catch (Exception ex)
            {
                return LocalRedirect("/Error?message=" + ex.Message);
            }

            string url = GetCurrentUrl(path);
            await LogEvent(Actions.EDIT_FOLDER, path + folderName);
            await GetLastestActivities();
            return LocalRedirect(url);
        }

        public async Task<IActionResult> OnPostFolderNew(string path, string newFolderName)
        {
            try
            {
                System.IO.Directory.CreateDirectory(path + newFolderName);
            }
            catch (Exception ex)
            {
                return LocalRedirect("/Error?message=" + ex.Message);
            }

            string url = GetCurrentUrl(path);
            await LogEvent(Actions.CREATE_FOLDER, path + newFolderName);
            await GetLastestActivities();
            return LocalRedirect(url);
        }

        private static string GetCurrentUrl(string path)
        {
            var currentFolder = path.Split('/').Take(path.Split('/').Length - 2).Aggregate((c, n) => c + "/" + n);
            var currentLeaf = path.Split('/')[path.Split('/').Length - 2];
            var url = $"/changeFolder?folderName={currentLeaf}&path={currentFolder}";
            return url;
        }

        public async Task<IActionResult> OnGetFileDownload(string path, string fileName)
        {
            var filePath = path + fileName;
            filePath = Uri.UnescapeDataString(filePath);

            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            await LogEvent(Actions.DOWNLOAD_FILE, filePath);
            await GetLastestActivities();
            return File(fileBytes, "application/force-download", Uri.UnescapeDataString(fileName));
        }

        public async Task<IActionResult> OnPostFileDelete(string path, string fileName)
        {
            var filePath = path + fileName;
            try
            {
                var trashPath = RootPath + "/" + Translations.Trash + "/" + fileName + "." + DateTime.UtcNow.Ticks.ToString();

                System.IO.File.Copy(filePath, trashPath);
                System.IO.File.Delete(filePath);
            }
            catch (Exception ex)
            {
                return LocalRedirect("/Error?message=" + ex.Message);
            }

            await LogEvent(Actions.DELETE_FILE, filePath);

            await GetLastestActivities();
            string url = GetCurrentUrl(path);
            return LocalRedirect(url);
        }

        public async Task<IActionResult> OnPostFolderDelete(string path, string folderName)
        {
            var folderPath = path + folderName;
            try
            {
                var trashPath = RootPath + "/" + Translations.Trash + "/" + folderName + DateTime.UtcNow.Ticks.ToString();
                System.IO.Directory.Move(folderPath, trashPath);
            }
            catch (Exception ex)
            {
                return LocalRedirect("/Error?message=" + ex.Message);
            }

            await LogEvent(Actions.DELETE_FOLDER, folderPath);

            await GetLastestActivities();
            string url = GetCurrentUrl(folderPath);
            return LocalRedirect(url);
        }

        private async Task GetLastestActivities()
        {
            Activities = await EventLog.GetLatest();
        }

        private async Task LogEvent(string action, string filePath)
        {
            await EventLog.Write((string)Request.HttpContext.Items["UserName"], action, filePath);
        }

        private void GetCurrentFilesAndFolders(string path)
        {
            var folderInfo = fileRepository.GetCurrentFilesAndFolders(path);
            Folders = folderInfo.Folders;
            Files = folderInfo.Files;
        }
    }
}
