using System.ComponentModel.DataAnnotations;

namespace Product_Managemnet_System_Razor_Core.Models
{
    /// <summary>
    /// Represents a Product item in the store inventory.
    /// Uses Data Annotations for model validation on Razor Pages forms.
    /// </summary>
    public class Product
    {
        // Primary key (Identity in SQL Server)
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter product title.")]
        [StringLength(50, ErrorMessage = "Title cannot exceed 50 characters.")]
        [Display(Name = "Product Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter product description.")]
        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
        [Display(Name = "Description")]
        public string description { get; set; } = string.Empty;

        // Relative path to uploaded image file (e.g., images/products/xyz.png)
        [StringLength(255)]
        [Display(Name = "Product Image")]
        public string image { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, 1000000, ErrorMessage = "Price must be 0 or a positive value.")]
        [Display(Name = "Price (PKR/USD)")]
        public int price { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(0, 50000, ErrorMessage = "Quantity cannot be negative.")]
        [Display(Name = "Stock Quantity")]
        public int quantity { get; set; }

        // Status: 'Active' / 'Inactive' (or 'Available' / 'Not-Available')
        [Display(Name = "Availability Status")]
        public string status { get; set; } = "Active";
    }
}