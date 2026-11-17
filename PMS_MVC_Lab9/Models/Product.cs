using System.ComponentModel.DataAnnotations;

namespace PMS_MVC_Lab9.Models
{
    /// <summary>
    /// Product entity used by the MVC views and the ADO.NET repository.
    /// Validation rules are enforced by Data Annotations on both client and server.
    /// </summary>
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the product title.")]
        [StringLength(50, ErrorMessage = "Title cannot exceed 50 characters.")]
        [Display(Name = "Product Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a description.")]
        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter an image path or URL.")]
        [StringLength(255, ErrorMessage = "Image path cannot exceed 255 characters.")]
        [Display(Name = "Image Path")]
        public string Image { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the price.")]
        [Range(0, 1000000, ErrorMessage = "Price must be between 0 and 1,000,000.")]
        public int Price { get; set; }

        [Required(ErrorMessage = "Please enter the quantity.")]
        [Range(0, 50000, ErrorMessage = "Quantity must be between 0 and 50,000.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Please select a status.")]
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        public string Status { get; set; } = "Active";
    }
}
