using SuperTestLibrary.Storages;

namespace SuperTestLibrary.Tests.StepDefinitions
{
    [Binding]
    public class FetchReqIFFilesSteps
    {
        private GitReqIFStorage gitReqIFStorage = new GitReqIFStorage("");
        private IEnumerable<string> _reqifFilesPath= [];

        [Given(@"I have a ReqIF file located at ""(.*)""")]
        public void GivenIHaveAReqIFFileLocatedAt(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"ReqIF file not found at {filePath}");
            }
        }

        [Given(@"I have a Git local folder located at ""(.*)""")]
        public void GivenIHaveAGitLocalFolderLocatedAt(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException($"Git local folder not found at {folderPath}");
            }
            gitReqIFStorage = new GitReqIFStorage(folderPath);
        }

        [When(@"I fetch ReqIF files from the Git local folder")]
        public void WhenIFetchTheReqIFFiles()
        {
            _reqifFilesPath = gitReqIFStorage.GetAllReqIFsAsync().Result;
        }

        [Then(@"I should get the ReqIF file path ""(.*)""")]
        public void ThenIShouldSeeTheReqIFFiles(string filePath)
        {
            Assert.Contains(filePath, _reqifFilesPath);
        }
    }
}
