using Microsoft.Extensions.DependencyInjection;
using SuperTestWPF.Models;
using SuperTestWPF.Services;
using System.Collections.ObjectModel;

namespace SuperTestWPF.ViewModels
{
    public class PromptVerboseViewModel : ViewModelBase
    {
        public string Title { get; set; } = "Prompt";
        public ObservableCollection<PromptHistory> PromptHistories => _promptVerboseService.PromptHistories;

        private readonly IPromptVerboseService _promptVerboseService;

        public PromptVerboseViewModel(IServiceProvider serviceProvider)
        {
            _promptVerboseService = serviceProvider.GetRequiredService<IPromptVerboseService>();
        }
    }
}
