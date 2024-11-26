namespace SuperTestWPF.Services
{
    public interface IFileService
    {
        string GetFileContent(string filePath);
        string OpenFileDialog(string filter);
        IEnumerable<string> OpenFilesDialog(string filter);
        string SelectFolderLocation(string path);
    }
}