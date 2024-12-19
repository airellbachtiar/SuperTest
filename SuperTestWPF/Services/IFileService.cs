using ReqIFSharp;

namespace SuperTestWPF.Services
{
    public interface IFileService
    {
        string GetFileContent(string filePath);
        string OpenFileDialog(string filter);
        IEnumerable<string> OpenFilesDialog(string filter);
        void SaveFile(string savePath, string fileContent);
        void SaveFile(string savePath, ReqIF reqIf);
        string SelectFolderLocation(string path);
    }
}