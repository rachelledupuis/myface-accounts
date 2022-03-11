using Microsoft.AspNetCore.Mvc;
using MyFace.Models.Request;
using MyFace.Models.Response;
using MyFace.Models.Database;
using MyFace.Repositories;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using Microsoft.AspNetCore.Http;
using MyFace.Helpers;
using MyFace.Services;

namespace MyFace.Controllers
{
    [ApiController]
    [Route("/posts")]
    public class PostsController : ControllerBase
    {    
        private readonly IPostsRepo _posts;
        private readonly IUsersRepo _users;

        public PostsController(IPostsRepo posts, IUsersRepo users)
        {
            _posts = posts;
            _users = users;
        }
        
        [HttpGet("")]
        public ActionResult<PostListResponse> Search([FromQuery] PostSearchRequest searchRequest)
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

            var posts = _posts.Search(searchRequest);
            var postCount = _posts.Count(searchRequest);
            return PostListResponse.Create(searchRequest, posts, postCount);
        }

        [HttpGet("{id}")]
        public ActionResult<PostResponse> GetById([FromRoute] int id)
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
            var post = _posts.GetById(id);
            return new PostResponse(post);
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] CreatePostRequest newPost)
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

            User user = _users.GetByUsername(username);
            newPost.UserId = user.Id;
            // if (user.Id != newPost.UserId)
            // {
            //     return StatusCode(
            //         StatusCodes.Status403Forbidden,
            //         "You are not allowed to create a post for a different user"
            //     );
            // }

            var post = _posts.Create(newPost);
            var url = Url.Action("GetById", new { id = post.Id });
            var postResponse = new PostResponse(post);
            return Created(url, postResponse);
        }

        [HttpPatch("{id}/update")]
        public ActionResult<PostResponse> Update([FromRoute] int id, [FromBody] UpdatePostRequest update)
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

            User user = _users.GetByUsername(username);
            Post userPost = _posts.GetById(id);
            
            if (user.Id != userPost.UserId)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    "You are not allowed to create a post for a different user"
                );
            }

            var post = _posts.Update(id, update);
            return new PostResponse(post);
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

            User user = _users.GetByUsername(username);
            Post userPost = _posts.GetById(id);
            
            if (user.Id != userPost.UserId)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    "You are not allowed to create a post for a different user"
                );
            }
            _posts.Delete(id);
            return Ok();
        }
    }
}