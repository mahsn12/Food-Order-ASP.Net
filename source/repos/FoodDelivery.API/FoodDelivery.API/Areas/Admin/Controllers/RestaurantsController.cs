using FoodDelivery.Application.DTOs.Restaurants;
using FoodDelivery.Domain.Entities;
using FoodDelivery.Domain.IRepository;
using FoodDelivery.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.API.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RestaurantsController : ControllerBase
    {
        private readonly IRepository<Restaurant> _restaurantRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public RestaurantsController(IRepository<Restaurant> restaurantRepo, UserManager<ApplicationUser> userManager)
        {
            _restaurantRepo = restaurantRepo;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RestaurantDto>>> GetAll()
        {
            var restaurants = await _restaurantRepo.GetAsync();

            var response = restaurants.Select(r => new RestaurantDto(
                r.Id,
                r.Name,
                r.Description,
                r.Phone,
                r.Email,
                r.RatingAvg,
                r.IsOpen,
                r.CreatedAt
            ));

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RestaurantDto>> GetById(int id)
        {
            var restaurant = await _restaurantRepo.GetOneAsync(x => x.Id == id);

            if (restaurant == null)
                return NotFound($"Restaurant with ID {id} not found.");

            var response = new RestaurantDto(
                restaurant.Id,
                restaurant.Name,
                restaurant.Description,
                restaurant.Phone,
                restaurant.Email,
                restaurant.RatingAvg,
                restaurant.IsOpen,
                restaurant.CreatedAt
            );

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<RestaurantDto>> Create([FromBody] CreateRestaurantDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _userManager.FindByEmailAsync(request.Email) is not null)
                return BadRequest("Email is already used.");

            var identityUser = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.Name,
                PhoneNumber = request.Phone,
                EmailConfirmed = true
            };

            var createUserResult = await _userManager.CreateAsync(identityUser, request.Password);
            if (!createUserResult.Succeeded)
                return BadRequest(createUserResult.Errors.Select(e => e.Description));

            var addRoleResult = await _userManager.AddToRoleAsync(identityUser, "Restaurant");
            if (!addRoleResult.Succeeded)
                return BadRequest(addRoleResult.Errors.Select(e => e.Description));

            var restaurant = new Restaurant
            {
                Name = request.Name,
                Description = request.Description,
                Phone = request.Phone,
                Email = request.Email,
                IdentityUserId = identityUser.Id,
                RatingAvg = 0,
                IsOpen = true,
                CreatedAt = DateTime.UtcNow
            };

            await _restaurantRepo.AddAsync(restaurant);
            await _restaurantRepo.CommitAsync();

            var response = new RestaurantDto(
                restaurant.Id,
                restaurant.Name,
                restaurant.Description,
                restaurant.Phone,
                restaurant.Email,
                restaurant.RatingAvg,
                restaurant.IsOpen,
                restaurant.CreatedAt
            );

            return CreatedAtAction(nameof(GetById), new { id = restaurant.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRestaurantDto request)
        {
            if (id != request.Id)
                return BadRequest("ID mismatch");

            var restaurant = await _restaurantRepo.GetOneAsync(x => x.Id == id);

            if (restaurant == null)
                return NotFound();

            restaurant.Name = request.Name;
            restaurant.Description = request.Description;
            restaurant.Phone = request.Phone;
            restaurant.Email = request.Email;
            restaurant.IsOpen = request.IsOpen;

            _restaurantRepo.Update(restaurant);
            await _restaurantRepo.CommitAsync();

            if (!string.IsNullOrWhiteSpace(restaurant.IdentityUserId))
            {
                var identityUser = await _userManager.FindByIdAsync(restaurant.IdentityUserId);
                if (identityUser is not null)
                {
                    identityUser.Email = request.Email;
                    identityUser.UserName = request.Email;
                    identityUser.FullName = request.Name;
                    identityUser.PhoneNumber = request.Phone;
                    await _userManager.UpdateAsync(identityUser);
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var restaurant = await _restaurantRepo.GetOneAsync(x => x.Id == id);

            if (restaurant == null)
                return NotFound();

            _restaurantRepo.Delete(restaurant);
            await _restaurantRepo.CommitAsync();

            if (!string.IsNullOrWhiteSpace(restaurant.IdentityUserId))
            {
                var identityUser = await _userManager.FindByIdAsync(restaurant.IdentityUserId);
                if (identityUser is not null)
                {
                    await _userManager.DeleteAsync(identityUser);
                }
            }

            return NoContent();
        }
    }
}
