using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Product_Managemnet_System_Razor_Core.Data;
using Product_Managemnet_System_Razor_Core.Models;

namespace Product_Managemnet_System_Razor_Core.Pages.Products
{
    public class DetailsModel : PageModel
    {
        private readonly ProductRepository _productRepo;

        public DetailsModel(ProductRepository productRepo)
        {
            _productRepo = productRepo;
        }

        public Product Product { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Fetch product record by Id from repository
            var product = await _productRepo.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            Product = product;
            return Page();
        }
    }
}
