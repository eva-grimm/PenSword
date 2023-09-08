using PenSword.Enums;
using PenSword.Services.Interfaces;

namespace PenSword.Services
{
    public class ImageService : IImageService
    {
        private readonly string? _defaultBlogImage = "/img/DefaultBlogImage.png";
        private readonly string? _defaultUserImage = "/img/DefaultUserImage.png";
        private readonly string? _defaultCategoryImage = "/img/DefaultCategoryImage.png";
        private readonly string? _blogAuthorImage = "/img/Cadence-Eva-Headshot.jpg";
        public string? ConvertByteArrayToFile(byte[]? fileData, string? extension, DefaultImage defaultImage)
        {
            try
            {
                if (fileData is null || fileData.Length == 0)
                {
                    switch (defaultImage)
                    {
                        case DefaultImage.AuthorImage: return _blogAuthorImage;
                        case DefaultImage.BlogPostImage: return _defaultBlogImage;
                        case DefaultImage.BlogUserImage: return _defaultUserImage;
                        case DefaultImage.CategoryImage: return _defaultCategoryImage;
                    }
                }

                string? imageBase64Data = Convert.ToBase64String(fileData!);
                imageBase64Data = string.Format($"data:{extension};base64,{imageBase64Data}");
                return imageBase64Data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile? file)
        {
            try
            {
                using MemoryStream memoryStream = new();
                await file!.CopyToAsync(memoryStream);
                byte[] byteFile = memoryStream.ToArray();
                memoryStream.Close();
                return byteFile;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}