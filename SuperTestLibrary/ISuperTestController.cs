namespace SuperTestLibrary
{
    public interface ISuperTestController
    {
        string GenerateSpecFlowFeatureFile();

        Task<IEnumerable<string>> GetAllReqIFFilesAsync();
    }
}
