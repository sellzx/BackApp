using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackApp.Models.Input;
using BackApp.Services.DynamoDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendsController : ControllerBase
    {
        [HttpPost]
        [Route("Request")]
        public async Task<IActionResult> FriendRequest([FromBody] RequestInputModel model)
        {
            try
            {
                var result = await new LoginService().FriendRequest(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("Friends")]
        public async Task<IActionResult> GetFriends(string user)
        {
            try
            {
                var result = await new LoginService().GetFriends(user);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("Requests")]
        public async Task<IActionResult> GetRequests(string user)
        {
            try
            {
                var result = await new LoginService().GetRequests(user);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("AcceptFriend")]
        public async Task<IActionResult> AcceptFriend([FromBody] RequestInputModel model)
        {
            try
            {
                var result = await new LoginService().AcceptFriends(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("DeclineRequest")]
        public async Task<IActionResult> DeclineRequest([FromBody] RequestInputModel model)
        {
            try
            {
                var result = await new LoginService().DeclineRequest(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("DeleteFriend")]
        public async Task<IActionResult> DeleteFriend([FromBody] RequestInputModel model)
        {
            try
            {
                var result = await new LoginService().DeleteFriend(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}