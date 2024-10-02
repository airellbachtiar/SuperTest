namespace SuperTestLibrary.Services
{
    public interface ILLM
    {
        Task<string> GenerateSpecFlowFeatureFileAsync(string requirement);
    }
}
