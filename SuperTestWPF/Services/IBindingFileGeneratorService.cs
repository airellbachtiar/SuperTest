using SuperTestWPF.Models;
using System.Collections.ObjectModel;

namespace SuperTestWPF.Services
{
    public interface IBindingFileGeneratorService
    {
        Task<string> GenerateBindingFilesAsync(string selectedLlmString, FileInformation featureFile, ObservableCollection<FileInformation> additionalCode);
    }
}