using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Product_Managemnet_System_Razor_Core.Data;
using Product_Managemnet_System_Razor_Core.Models;

namespace Product_Managemnet_System_Razor_Core.Pages.Products
{
    public class CreateModel : PageModel
    {
        private readonly ProductRepository _productRepo;
        private readonly ProductImageService _imageService;

        public CreateModel(ProductRepository productRepo, ProductImageService imageService)
        {
            _productRepo = productRepo;
            _imageService = imageService;
        }

        [BindProperty]
        public Product Product { get; set; } = new();

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public void OnGet()
        {
            // Default status value 'A' for Active
            Product.status = "A";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // 1. Validate image upload
            if (ImageFile == null || ImageFile.Length == 0)
            {
                ModelState.AddModelError(nameof(ImageFile), "Please choose a product image.");
            }
            else
            {
                string? imgError = _imageService.Validate(ImageFile);
                if (imgError != null)
                {
                    ModelState.AddModelError(nameof(ImageFile), imgError);
                }
            }

            // 2. Validate overall model state
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 3. Save image file to wwwroot/images/products
            string savedPath = await _imageService.SaveAsync(ImageFile!);

            try
            {
                Product.image = savedPath;

                // 4. Insert into database via repository
                await _productRepo.CreateAsync(Product);

                TempData["Success"] = "Product created successfully!";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                // If database insert errors out, delete uploaded image to prevent orphan files
                _imageService.Delete(savedPath);

                // Add model error to display friendly message on the page instead of HTTP 500 crash
                ModelState.AddModelError(string.Empty, "Could not save product: " + ex.Message);
                return Page();
            }
        }
    }
}
