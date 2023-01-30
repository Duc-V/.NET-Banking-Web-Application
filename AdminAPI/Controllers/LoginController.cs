using Microsoft.AspNetCore.Mvc;
using AdminAPI.Models;

namespace AdminAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (model.Username == "admin" && model.Password == "admin")
            {
                return Ok("Login Successful");
            }
            else
            {
                return Unauthorized("Incorrect username or password");
            }
        }

    }
}
