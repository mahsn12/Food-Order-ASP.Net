using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    [HttpGet("panel")]
    public ActionResult<object> AdminPanel()
    {
        return Ok(new { Message = "Admin panel access granted." });
    }
}
