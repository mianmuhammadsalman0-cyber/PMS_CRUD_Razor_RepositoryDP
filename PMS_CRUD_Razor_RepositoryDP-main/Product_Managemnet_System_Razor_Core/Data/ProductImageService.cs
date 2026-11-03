namespace Product_Managemnet_System_Razor_Core.Data
{
    /// <summary>
    /// Service responsible for validating, saving, and deleting product images in wwwroot/images/products
    /// </summary>
    public class ProductImageService
    {
        private readonly IWebHostEnvironment _webHostEnv;

        // Allowed image file formats
        private static readonly HashSet<string> PermittedExtensions =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ".jpg", ".jpeg", ".png", ".webp"
            };

        // 5 MB max file size limit
        private const long MaxFileSizeInBytes = 5 * 1024 * 1024;

        public ProductImageService(IWebHostEnvironment webHostEnv)
        {
            _webHostEnv = webHostEnv;
        }

        /// <summary>
        /// Validates that an image file is provided, within size limit, and has an allowed extension.
        /// </summary>
        public string? Validate(IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                return "Please choose an image file for the product.";
            }

            if (file.Length > MaxFileSizeInBytes)
            {
                return "Selected image size exceeds the 5 MB limit.";
            }

            string extension = Path.GetExtension(file.FileName);

            if (string.IsNullOrEmpty(extension) || !PermittedExtensions.Contains(extension))
            {
                return "Invalid file format. Supported types: JPG, JPEG, PNG, WEBP.";
            }

            return null;
        }

        /// <summary>
        /// Generates a unique GUID file name and saves the image to wwwroot/images/products folder.
        /// Returns the relative URL path for web access.
        /// </summary>
        public async Task<string> SaveAsync(IFormFile file)
        {
            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            // Destination directory under wwwroot
            string uploadDir = Path.Combine(_webHostEnv.WebRootPath, "images", "products");

            // Ensure directory exists
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            // Generate unique filename using GUID to avoid name conflicts
            string uniqueFileName = $"{Guid.NewGuid():N}{extension}";
            string physicalPath = Path.Combine(uploadDir, uniqueFileName);

            await using (var fileStream = new FileStream(physicalPath, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(fileStream);
            }

            // Return relative path stored in database
            return $"images/products/{uniqueFileName}";
        }

        /// <summary>
        /// Deletes an existing product image from the disk to free up storage.
        /// </summary>
        public void Delete(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return;
            }

            const string targetPrefix = "images/products/";

            if (!relativePath.StartsWith(targetPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            string fileName = relativePath.Substring(targetPrefix.Length);

            if (string.IsNullOrWhiteSpace(fileName) || fileName != Path.GetFileName(fileName))
            {
                return;
            }

            string targetFolder = Path.GetFullPath(Path.Combine(_webHostEnv.WebRootPath, "images", "products"));
            string fullPhysicalPath = Path.GetFullPath(Path.Combine(targetFolder, fileName));

            // Security check: ensure path stays within images/products directory
            if (!fullPhysicalPath.StartsWith(targetFolder + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (File.Exists(fullPhysicalPath))
            {
                File.Delete(fullPhysicalPath);
            }
        }
    }
}
