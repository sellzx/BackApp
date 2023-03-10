using System;
using System.Threading.Tasks;
using BackApp.Models.Input;
using BackApp.Services.S3;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : Controller
    {

        [HttpPost]
        [Route("PostImage")]
        public async Task<IActionResult> PostImage([FromBody] ImageInputModel request)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(request.Image);
                var result = await new S3Service().DataTransferAsync(imageBytes, request.Owner);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}