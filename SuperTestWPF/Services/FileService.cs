using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System.IO;

namespace SuperTestWPF.Services
{
    public class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;

        public FileService(ILogger<FileService> logger)
        {
            _logger = logger;
        }

        public string OpenFileDialog(string filter)
        {
            var openFileDialog = new OpenFileDialog { Filter = filter };
            return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : string.Empty;
        }

        public IEnumerable<string> OpenFilesDialog(string filter)
        {
            var openFileDialog = new OpenFileDialog { Filter = filter, Multiselect = true };
            return openFileDialog.ShowDialog() == true ? openFileDialog.FileNames : new List<string>();
        }

        public string SelectFolderLocation(string path)
        {
            var folderDialog = new OpenFolderDialog
            {
                DefaultDirectory = path,
            };

            if (folderDialog.ShowDialog() == true)
            {
                return folderDialog.FolderName;
            }
            return path;
        }

        public string GetFileContent(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    return File.ReadAllText(filePath);
                }
                else
                {
                    _logger.LogWarning("File does not exist.");
                }
            }
            catch (IOException ex)
            {
                _logger.LogError($"IOException while reading {filePath}: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, $"UnauthorizedAccessException while reading {filePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception while reading {filePath}");
            }

            return string.Empty;
        }

        public void SaveFile(string savePath, string fileContent)
        {
            File.WriteAllText(savePath, fileContent);
            _logger.LogInformation($"File has been saved to \"{savePath}\".");
        }
    }
}
