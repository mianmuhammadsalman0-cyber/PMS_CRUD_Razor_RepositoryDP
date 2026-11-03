using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Product_Managemnet_System_Razor_Core.Data;
using Product_Managemnet_System_Razor_Core.Models;

namespace Product_Managemnet_System_Razor_Core.Pages.Products
{
    public class DeleteModel : PageModel
    {
        private readonly ProductRepository _productRepo;
        private readonly ProductImageService _imageService;

        public DeleteModel(ProductRepository productRepo, ProductImageService imageService)
        {
            _productRepo = productRepo;
            _imageService = imageService;
        }

        public Product Product { get; set; } = new();

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
            var product = await _productRepo.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            // Execute delete query in database via repository
            bool isDeleted = await _productRepo.DeleteAsync(id);

            if (!isDeleted)
            {
                return NotFound();
            }

            // Delete product image from file system
            _imageService.Delete(product.image);

            TempData["Success"] = "Product deleted successfully!";
            return RedirectToPage("Index");
        }
    }
}
