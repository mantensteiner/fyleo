using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace fyleo.Repository.Models
{
    public class FileRepository : IFileRepository
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly ITranslations translations;

        public FileRepository(IWebHostEnvironment hostingEnvironment, ITranslations translations)
        {
            this.translations = translations;
            this.hostingEnvironment = hostingEnvironment;

            var rootPath = GetRootPath();
            if (!Directory.Exists(rootPath))
                Directory.CreateDirectory(rootPath);

            if (!Directory.Exists(translations.Trash))
                Directory.CreateDirectory(Path.Combine(rootPath, translations.Trash));
        }

        public FolderInfo GetCurrentFilesAndFolders(string path)
        {
            var subDirs = Directory.GetDirectories(path);
            var currentFiles = Directory.GetFiles(path);

            var fileDataResult = new List<FileData>();

            foreach (var f in currentFiles)
            {
                var fi = new FileInfo(f);
                fileDataResult.Add(new FileData()
                {
                    Name = f.Replace(path, ""),
                    LastModified = fi.LastWriteTimeUtc
                });
            }

            IEnumerable<string> dirLeafs = subDirs.Select(x => x.Replace(path, "")).Where(x => x != translations.Trash).OrderBy(x => x).ToList();
            if (subDirs.Select(x => x.Replace(path, "")).Any(x => x == translations.Trash))
                dirLeafs = dirLeafs.Append(translations.Trash);

            return new FolderInfo()
            {
                Folders = dirLeafs.ToArray(),
                Files = fileDataResult.ToArray()
            };
        }

        public async Task<byte[]> GetFile(string path, string fileName)
        {
            var filePath = Uri.UnescapeDataString(Combine(path, fileName));

            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return fileBytes;
        }

        public void EditFile(string path, string oldFileName, string fileName)
        {
            System.IO.File.Move(Path.Combine(path, oldFileName), Path.Combine(path, fileName));
        }

        public void EditFolder(string path, string oldFolderName, string folderName)
        {
            System.IO.Directory.Move(Path.Combine(path, oldFolderName), Path.Combine(path, folderName));
        }

        public string DeleteFile(string path, string fileName)
        {
            var filePath = Path.Combine(path, fileName);
            var trashPath = Path.Combine(GetRootPath(), translations.Trash, fileName + "." + DateTime.UtcNow.Ticks.ToString());

            System.IO.File.Copy(filePath, trashPath);
            System.IO.File.Delete(filePath);

            return filePath;
        }

        public string DeleteFolder(string path, string folderName)
        {
            var folderPath = Path.Combine(path, folderName);
            var trashPath = Path.Combine(GetRootPath(), translations.Trash, folderName + "." + DateTime.UtcNow.Ticks.ToString());
            
            System.IO.Directory.Move(folderPath, trashPath);

            return folderPath;
        }

        public void CreateFolder(string path, string newFolderName)
        {
            System.IO.Directory.CreateDirectory(Combine(path, newFolderName));
        }

        public string GetRootPath()
        {
            return hostingEnvironment.ContentRootPath + "/files/";
        }

        public string Combine(params string[] paths)
        {
            return Path.Combine(paths);
        }

        public async Task SaveFile(string path, string fileName, Stream readStream)
        {
            var filePath = Combine(path, fileName);

            // Move existing file to trash for backup version
            if (System.IO.File.Exists(filePath))
            {
                var trashPath = Combine(GetRootPath(), translations.Trash, fileName + "." + DateTime.UtcNow.Ticks.ToString());
                System.IO.File.Copy(filePath, trashPath);
            }

            using (var writeStream = new FileStream(filePath, FileMode.Create))
            {
                await readStream.CopyToAsync(writeStream);
            }
        }
    }
}