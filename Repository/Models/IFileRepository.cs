using System.Collections.Generic;


namespace fyleo.Repository.Models
{
    public interface IFileRepository
    {
        string GetRootPath();
        FolderInfo GetCurrentFilesAndFolders(string path);
    }
}