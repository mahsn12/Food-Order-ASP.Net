using System.Security.Claims;
using FoodDelivery.API.Models.Auth;
using FoodDelivery.API.Services;
using FoodDelivery.Infrastructure.Data;
using FoodDelivery.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IJwtTokenService jwtTokenService,
    AppDbContext appDbContext) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        if (await userManager.FindByEmailAsync(request.Email) is not null)
        {
            return BadRequest("Email already exists.");
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.Select(x => x.Description));
        }

        await userManager.AddToRoleAsync(user, "User");

        var token = await jwtTokenService.CreateTokenAsync(user);
        var roles = await userManager.GetRolesAsync(user);

        return Ok(new AuthResponse(user.Id, user.FullName, user.Email!, token, roles.ToList()));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            var restaurant = await appDbContext.Restaurants.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Email == request.Email);

            if (restaurant is null || restaurant.PasswordHash != request.Password)
            {
                return Unauthorized("Invalid credentials.");
            }

            var restaurantToken = jwtTokenService.CreateRestaurantToken(restaurant);
            return Ok(new AuthResponse(restaurant.Id.ToString(), restaurant.Name, restaurant.Email, restaurantToken, ["Restaurant"]));
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
        {
            return Unauthorized("Invalid credentials.");
        }

        var token = await jwtTokenService.CreateTokenAsync(user);
        var roles = await userManager.GetRolesAsync(user);

        return Ok(new AuthResponse(user.Id, user.FullName, user.Email!, token, roles.ToList()));
    }

    [HttpPost("admin/login")]
    public async Task<ActionResult<AuthResponse>> AdminLogin(AdminLoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return Unauthorized("Invalid credentials.");
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
        {
            return Unauthorized("Invalid credentials.");
        }

        var roles = await userManager.GetRolesAsync(user);
        if (!roles.Contains("Admin"))
        {
            return Forbid();
        }

        var token = await jwtTokenService.CreateTokenAsync(user);
        return Ok(new AuthResponse(user.Id, user.FullName, user.Email!, token, roles.ToList()));
    }

    [HttpPost("forget-password")]
    public async Task<ActionResult<object>> ForgotPassword(ForgotPasswordRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return Ok(new { Message = "If the email exists, reset instructions were generated." });
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        return Ok(new { Message = "Reset token generated.", Token = token });
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult<object>> ResetPassword(ResetPasswordRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return BadRequest("Invalid request.");
        }

        var result = await userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.Select(e => e.Description));
        }

        return Ok(new { Message = "Password reset successful." });
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<ProfileResponse>> Profile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return NotFound();
        }

        return Ok(new ProfileResponse(user.Id, user.FullName, user.Email ?? string.Empty, user.PhoneNumber, user.CreatedAt));
    }

    [HttpPost("external-login")]
    public async Task<ActionResult<AuthResponse>> ExternalLogin(ExternalLoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName
            };

            var createResult = await userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                return BadRequest(createResult.Errors.Select(x => x.Description));
            }

            await userManager.AddToRoleAsync(user, "User");
        }

        var token = await jwtTokenService.CreateTokenAsync(user);
        var roles = await userManager.GetRolesAsync(user);

        return Ok(new AuthResponse(user.Id, user.FullName, user.Email!, token, roles.ToList()));
    }
}
