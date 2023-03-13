using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackApp.Models.Input;
using BackApp.Services.DynamoDB;
using Microsoft.AspNetCore.Mvc;

namespace BackApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterInputModel registerModel)
        {
            try
            {

                var result = await new LoginService().RegisterUserAsync(registerModel);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("Login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                var result = await new LoginService().CheckUser(email, password);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}