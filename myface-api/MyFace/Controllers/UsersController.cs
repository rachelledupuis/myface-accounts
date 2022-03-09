﻿using Microsoft.AspNetCore.Mvc;
using MyFace.Models.Request;
using MyFace.Models.Response;
using MyFace.Repositories;
using MyFace.Models.Database;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Http;
using MyFace.Helpers;
using MyFace.Services;
namespace MyFace.Controllers
{
    [ApiController]
    [Route("/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepo _users;

        public UsersController(IUsersRepo users)
        {
            _users = users;
        }
        
        [HttpGet("")]
        public ActionResult<UserListResponse> Search([FromQuery] UserSearchRequest searchRequest)
        {
            var users = _users.Search(searchRequest);
            var userCount = _users.Count(searchRequest);
            return UserListResponse.Create(searchRequest, users, userCount);
        }

        [HttpGet("{id}")]
        public ActionResult<UserResponse> GetById([FromRoute] int id)
        {
            var authHeader = Request.Headers["Authorization"];

            if (authHeader == StringValues.Empty)
            {
                return Unauthorized();
            }

            var authHeaderString = authHeader[0];
            var usernamePassword = AuthHelper.GetUsernamePasswordFromAuth(authHeaderString);

            var username = usernamePassword.Username;
            var password = usernamePassword.Password;

            var auth = new AuthService(_users);
            if (auth.IsValidUsernameAndPassword(username, password) == false)
            {
                return Unauthorized();
            }
            var user = _users.GetById(id);
            return new UserResponse(user);
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] CreateUserRequest newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _users.Create(newUser);

            var url = Url.Action("GetById", new { id = user.Id });
            var responseViewModel = new UserResponse(user);
            return Created(url, responseViewModel);
        }

        [HttpPatch("{id}/update")]
        public ActionResult<UserResponse> Update([FromRoute] int id, [FromBody] UpdateUserRequest update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var authHeader = Request.Headers["Authorization"];

            if (authHeader == StringValues.Empty)
            {
                return Unauthorized();
            }

            var authHeaderString = authHeader[0];
            var usernamePassword = AuthHelper.GetUsernamePasswordFromAuth(authHeaderString);

            var username = usernamePassword.Username;
            var password = usernamePassword.Password;

            var auth = new AuthService(_users);
            if (auth.IsValidUsernameAndPassword(username, password) == false)
            {
                return Unauthorized();
            }

            User currentUser = _users.GetByUsername(username);
        
            
            if (currentUser.Id != id)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    "You are not allowed to update a different user's profile"
                );
            }

            var user = _users.Update(id, update);
            return new UserResponse(user);
        }
        
        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var authHeader = Request.Headers["Authorization"];

            if (authHeader == StringValues.Empty)
            {
                return Unauthorized();
            }

            var authHeaderString = authHeader[0];
            var usernamePassword = AuthHelper.GetUsernamePasswordFromAuth(authHeaderString);

            var username = usernamePassword.Username;
            var password = usernamePassword.Password;

            var auth = new AuthService(_users);
            if (auth.IsValidUsernameAndPassword(username, password) == false)
            {
                return Unauthorized();
            }

            User currentUser = _users.GetByUsername(username);
        
            
            if (currentUser.Id != id)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    "You are not allowed to delete a user"
                );
            }
            _users.Delete(id);
            return Ok();
        }
    }
}