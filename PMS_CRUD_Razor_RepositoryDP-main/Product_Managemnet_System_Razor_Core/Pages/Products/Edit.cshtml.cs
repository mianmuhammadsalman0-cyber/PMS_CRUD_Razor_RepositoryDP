using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Product_Managemnet_System_Razor_Core.Data;
using Product_Managemnet_System_Razor_Core.Models;

namespace Product_Managemnet_System_Razor_Core.Pages.Products
{
    public class EditModel : PageModel
    {
        private readonly ProductRepository _productRepo;
        private readonly ProductImageService _imageService;

        public EditModel(ProductRepository productRepo, ProductImageService imageService)
        {
            _productRepo = productRepo;
            _imageService = imageService;
        }

        [BindProperty]
        public Product Product { get; set; } = new();

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            Product = product;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (id != Product.Id)
            {
                return BadRequest();
            }

            var existingProduct = await _productRepo.GetByIdAsync(id);

            if (existingProduct == null)
            {
                return NotFound();
            }

            // Validate new image only if user chose to upload a replacement
            if (ImageFile != null && ImageFile.Length > 0)
            {
                string? imgError = _imageService.Validate(ImageFile);
                if (imgError != null)
                {
                    ModelState.AddModelError(nameof(ImageFile), imgError);
                }
            }

            if (!ModelState.IsValid)
            {
                // Retain current image preview on validation failure
                Product.image = existingProduct.image;
                return Page();
            }

            string oldImagePath = existingProduct.image;
            string finalImagePath = oldImagePath;

            // If a new image was uploaded, save it
            if (ImageFile != null && ImageFile.Length > 0)
            {
                finalImagePath = await _imageService.SaveAsync(ImageFile);
            }

            Product.image = finalImagePath;

            try
            {
                bool isUpdated = await _productRepo.UpdateAsync(Product);

                if (!isUpdated)
                {
                    // Clean up newly uploaded image if update fails
                    if (finalImagePath != oldImagePath)
                    {
                        _imageService.Delete(finalImagePath);
                    }
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                if (finalImagePath != oldImagePath)
                {
                    _imageService.Delete(finalImagePath);
                }
                ModelState.AddModelError(string.Empty, "Could not update product: " + ex.Message);
                return Page();
            }

            // Successfully updated: delete the old image file if replaced
            if (finalImagePath != oldImagePath)
            {
                _imageService.Delete(oldImagePath);
            }

            TempData["Success"] = "Product details updated successfully!";
            return RedirectToPage("Index");
        }
    }
}
