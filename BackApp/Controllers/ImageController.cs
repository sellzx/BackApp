using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackApp.Models.AWS.DynamoDBEntities;
using BackApp.Models.Input;
using BackApp.Services.DynamoDB;
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

        [HttpGet]
        [Route("GetImage")]
        public async Task<IActionResult> GetImage(string url)
        {
            try
            {
                var result = await new S3Service().GetImageAsync(url);
                HttpContext.Response.ContentType = "image/jpeg";
                HttpContext.Response.Headers.Add("Content-Disposition", $"attachment; filename={url}.jpg");
                await result.CopyToAsync(HttpContext.Response.Body);
                await HttpContext.Response.Body.FlushAsync();
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAllPosts")]
        public async Task<IActionResult> GetAllPosts(string username)
        {
            try
            {
                var result = await new PostService().QueryAsync(username);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}