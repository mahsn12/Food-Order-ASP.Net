using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Application.DTOs.Products;

public record ProductDto(int Id, int RestaurantId, string Name, string Description, decimal Price, string ImageUrl, bool IsAvailable);

public record CreateProductDto(
    [Required] int RestaurantId,
    [Required] string Name,
    string Description,
    [Range(0.01, double.MaxValue)] decimal Price,
    string ImageUrl,
    bool IsAvailable);

public record UpdateProductDto(
    [Required] int Id,
    [Required] int RestaurantId,
    [Required] string Name,
    string Description,
    [Range(0.01, double.MaxValue)] decimal Price,
    string ImageUrl,
    bool IsAvailable);
