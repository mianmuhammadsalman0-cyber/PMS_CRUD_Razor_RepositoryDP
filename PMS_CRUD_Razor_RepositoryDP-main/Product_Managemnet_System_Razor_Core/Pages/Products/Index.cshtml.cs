using Microsoft.AspNetCore.Mvc.RazorPages;
using Product_Managemnet_System_Razor_Core.Data;
using Product_Managemnet_System_Razor_Core.Models;

namespace Product_Managemnet_System_Razor_Core.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly ProductRepository _productRepo;

        public IndexModel(ProductRepository productRepo)
        {
            _productRepo = productRepo;
        }

        // Holds products retrieved from the database
        public List<Product> Products { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Fetch all products using repository pattern
            Products = await _productRepo.GetAllAsync();
        }
    }
}
