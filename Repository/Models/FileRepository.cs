using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
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

        public string GetRootPath()
        {
            return hostingEnvironment.ContentRootPath + "/files/";
        }
    }
}