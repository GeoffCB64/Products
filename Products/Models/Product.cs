using System.ComponentModel.DataAnnotations;

namespace Products.Models
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [MinLength(1, ErrorMessage = "Name cannot be empty.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Colour is required.")]
        [MinLength(1, ErrorMessage = "Colour cannot be empty.")]
        public string Colour { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.01")]
        public decimal Price { get; set; }
    }
    public record Product(int Id, string Name, string Colour, decimal Price, DateTime CreatedDate);

}
