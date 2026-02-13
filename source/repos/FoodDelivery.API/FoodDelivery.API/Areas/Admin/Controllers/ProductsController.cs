using FoodDelivery.Application.DTOs.Products;
using FoodDelivery.Domain.Entities;
using FoodDelivery.Domain.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.API.Areas.Admin.Controllers;

[Area("Admin")]
[Route("api/[area]/[controller]")]
[ApiController]
    [Authorize(Roles = "Admin")]
public class ProductsController(IRepository<Product> productRepo) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var products = await productRepo.GetAsync();
        var response = products.Select(p => new ProductDto(p.Id, p.RestaurantId, p.Name, p.Description, p.Price, p.ImageUrl, p.IsAvailable));
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await productRepo.GetOneAsync(p => p.Id == id);
        if (product is null) return NotFound();
        return Ok(new ProductDto(product.Id, product.RestaurantId, product.Name, product.Description, product.Price, product.ImageUrl, product.IsAvailable));
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto request)
    {
        var product = new Product
        {
            RestaurantId = request.RestaurantId,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            ImageUrl = request.ImageUrl,
            IsAvailable = request.IsAvailable
        };

        await productRepo.AddAsync(product);
        await productRepo.CommitAsync();

        var response = new ProductDto(product.Id, product.RestaurantId, product.Name, product.Description, product.Price, product.ImageUrl, product.IsAvailable);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto request)
    {
        if (id != request.Id) return BadRequest("ID mismatch");

        var product = await productRepo.GetOneAsync(p => p.Id == id);
        if (product is null) return NotFound();

        product.RestaurantId = request.RestaurantId;
        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.ImageUrl = request.ImageUrl;
        product.IsAvailable = request.IsAvailable;

        productRepo.Update(product);
        await productRepo.CommitAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await productRepo.GetOneAsync(p => p.Id == id);
        if (product is null) return NotFound();

        productRepo.Delete(product);
        await productRepo.CommitAsync();
        return NoContent();
    }
}
