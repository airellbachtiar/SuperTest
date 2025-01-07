namespace LargeLanguageModelLibrary.Models
{
    public class ImageContent
    {
        private readonly Uri _imageUri;
        private readonly BinaryData _imageBytes;
        private readonly string _imageBytesMediaType;

        public string Url { get; }

        public ImageContent(Uri uri)
        {
            _imageUri = uri;
            Url = uri.ToString();
        }

        public ImageContent(BinaryData imageBytes, string imageBytesMediaType)
        {
            _imageBytes = imageBytes;
            _imageBytesMediaType = imageBytesMediaType;

            string base64EncodedData = Convert.ToBase64String(_imageBytes.ToArray());
            Url = $"data:{_imageBytesMediaType};base64,{base64EncodedData}";
        }

        public Uri ImageUri => _imageUri;

        public BinaryData ImageBytes => _imageBytes;

        public string ImageBytesMediaType => _imageBytesMediaType;
    }
}
