using SuperTestLibrary.Storages;

namespace SuperTestLibrary.Tests.StepDefinitions
{
    [Binding]
    public class FetchReqIFFilesSteps
    {
        private GitReqIFStorage gitReqIFStorage = new(string.Empty);
        private IEnumerable<string> _reqifFilesPath = [];
        private string _errorMessage = string.Empty;

        [Given(@"the application is running")]
        public void GivenTheApplicationIsRunning()
        {
            // Initialize the application or perform any necessary setup
            gitReqIFStorage = new GitReqIFStorage("C:\\Dev\\GitLocalFolderTest");
        }

        [When(@"I request to fetch ReqIF files")]
        public void WhenIRequestToFetchReqIFFiles()
        {
            _reqifFilesPath = gitReqIFStorage.GetAllReqIFsAsync().Result;
        }

        [Then(@"the application should fetch ""(.*)"" from ""(.*)""")]
        public void ThenTheApplicationShouldFetchFrom(string fileName, string directory)
        {
            string expectedFilePath = Path.Combine(directory, fileName);
            Assert.Contains(expectedFilePath, _reqifFilesPath);
        }

        [Given(@"the application has fetched ReqIF files")]
        public void GivenTheApplicationHasFetchedReqIFFiles()
        {
            gitReqIFStorage = new GitReqIFStorage("C:\\Dev\\GitLocalFolderTest");
            _reqifFilesPath = gitReqIFStorage.GetAllReqIFsAsync().Result;
        }

        [When(@"I check the fetched files")]
        public void WhenICheckTheFetchedFiles()
        {
            // This step is implicit in our implementation
        }

        [Then(@"the file ""(.*)"" should exist in the application's data")]
        public void ThenTheFileShouldExistInTheApplicationsData(string fileName)
        {
            Assert.Contains(_reqifFilesPath, path => Path.GetFileName(path) == fileName);
        }

        [Then(@"the file ""(.*)"" should have been fetched from ""(.*)""")]
        public void ThenTheFileShouldHaveBeenFetchedFrom(string fileName, string directory)
        {
            string expectedFilePath = Path.Combine(directory, fileName);
            Assert.Contains(expectedFilePath, _reqifFilesPath);
        }

        [When(@"I request to fetch a non-existent ReqIF file ""(.*)"" from ""(.*)""")]
        public void WhenIRequestToFetchANonExistentReqIFFileFrom(string fileName, string directory)
        {
            try
            {
                gitReqIFStorage.ReadReqIFFileAsync(fileName, directory).Wait();
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
            }
        }

        [When(@"I request to fetch ReqIF files from an invalid directory ""(.*)""")]
        public void WhenIRequestToFetchReqIFFilesFromAnInvalidDirectory(string directory)
        {
            try
            {
                gitReqIFStorage = new GitReqIFStorage(directory);
                gitReqIFStorage.GetAllReqIFsAsync().Wait();
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
            }
        }

        [Then(@"the application should report an error")]
        public void ThenTheApplicationShouldReportAnError()
        {
            Assert.NotNull(_errorMessage);
        }

        [Then(@"the error message should indicate that the file was not found")]
        public void ThenTheErrorMessageShouldIndicateThatTheFileWasNotFound()
        {
            Assert.Contains("not found", _errorMessage);
        }

        [Then(@"the error message should indicate that the directory is invalid or inaccessible")]
        public void ThenTheErrorMessageShouldIndicateThatTheDirectoryIsInvalidOrInaccessible()
        {
            Assert.True(_errorMessage.Contains("not found") || _errorMessage.Contains("invalid") || _errorMessage.Contains("inaccessible"));
        }
    }
}
