using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Raa.RESTfulApi.Entities;
using Raa.RESTfulApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raa.RESTfulApi.Controllers
{
    [Authorize(Roles = "ADMIN")]
    [Route("api/users")]
    public class UsersController : Controller
    {
        private UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;

        }

        [HttpGet("")]
        public IActionResult GetUsers()
        {
            var users = _userManager.Users;
            var returnUsers = Mapper.Map<IEnumerable<ReturnUserDto>>(users);

            return Ok(returnUsers);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();
            var returnUser = Mapper.Map<ReturnUserDto>(user);

            return Ok(returnUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            await _userManager.DeleteAsync(user);

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] JsonPatchDocument<UpdateUserDto> patchDoc)
        {
            if (patchDoc == null) return BadRequest();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            var userToPatch = Mapper.Map<UpdateUserDto>(user);
            patchDoc.ApplyTo(userToPatch);


            Mapper.Map(userToPatch, user);

            await _userManager.UpdateAsync(user);

            return NoContent();
        }
    }
}
