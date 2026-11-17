using Microsoft.AspNetCore.Mvc;
using PMS_MVC_Lab9.Data;
using PMS_MVC_Lab9.Models;

namespace PMS_MVC_Lab9.Controllers
{
    /// <summary>
    /// MVC controller for the Product CRUD screens.
    /// Data work is delegated to ProductRepository (dependency injection).
    /// </summary>
    public class ProductsController : Controller
    {
        private readonly ProductRepository _repository;

        public ProductsController(ProductRepository repository)
        {
            _repository = repository;
        }

        // READ - /Products
        public async Task<IActionResult> Index()
        {
            return View(await _repository.GetAllAsync());
        }

        // CREATE - show the empty form
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // CREATE - save the posted product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            await _repository.CreateAsync(product);
            TempData["Success"] = "Product created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // READ - details of one product
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            return product is null ? NotFound() : View(product);
        }

        // UPDATE - show the product in an editable form
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            return product is null ? NotFound() : View(product);
        }

        // UPDATE - save the posted changes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            if (await _repository.GetByIdAsync(id) is null)
            {
                return NotFound();
            }

            await _repository.UpdateAsync(product);
            TempData["Success"] = "Product updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // DELETE - confirmation page
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            return product is null ? NotFound() : View(product);
        }

        // DELETE - remove the record after confirmation
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await _repository.GetByIdAsync(id) is null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(id);
            TempData["Success"] = "Product deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
