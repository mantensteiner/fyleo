using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace fyleo.Repository.Models
{
    public interface IFileRepository
    {
        string GetRootPath();
        Task SaveFile(string path, string fileName, Stream readStream);
        FolderInfo GetCurrentFilesAndFolders(string path);
        Task<byte[]> GetFile(string path, string fileName);
        void EditFile(string path, string oldFileName, string fileName);
        void EditFolder(string path, string oldFolderName, string folderName);
        string DeleteFile(string path, string fileName);
        string DeleteFolder(string path, string folderName);
        void CreateFolder(string path, string newFolderName);
        string Combine(params string[] paths);
    }
}