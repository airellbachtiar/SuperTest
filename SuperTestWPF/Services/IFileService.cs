namespace SuperTestWPF.Services
{
    public interface IFileService
    {
        string GetFileContent(string filePath);
        string OpenFileDialog(string filter);
        IEnumerable<string> OpenFilesDialog(string filter);
        void SaveFile(string savePath, string fileContent);
        string SelectFolderLocation(string path);
    }
}