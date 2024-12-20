using SuperTestWPF.Models;
using System.Collections.ObjectModel;

namespace SuperTestWPF.Services
{
    public interface IPromptVerboseService
    {
        ObservableCollection<PromptHistory> PromptHistories { get; set; }
        void AddPrompt(PromptHistory prompt);
    }
}