namespace SuperTestWPF.Models
{
    public class SpecFlowBindingFileModel (string bindingFileName, string bindingFileContent)
    {
        public string BindingFileName { get; set; } = bindingFileName;
        public string BindingFileContent { get; set; } = bindingFileContent;
    }
}