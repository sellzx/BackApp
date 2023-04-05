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
    public class PostsController : Controller
    {
        [HttpPost]
        [Route("Post")]
        public async Task<IActionResult> Post(PostInputModel registerModel)
        {
            try
            {

                var result = await new PostService().NewPost(registerModel);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}