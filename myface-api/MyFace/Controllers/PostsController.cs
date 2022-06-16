﻿using Microsoft.AspNetCore.Mvc;
using MyFace.Models.Request;
using MyFace.Models.Response;
using MyFace.Repositories;
using System;

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
            var posts = _posts.Search(searchRequest);
            var postCount = _posts.Count(searchRequest);
            return PostListResponse.Create(searchRequest, posts, postCount);
        }

        [HttpGet("{id}")]
        public ActionResult<PostResponse> GetById([FromRoute] int id)
        {
            var post = _posts.GetById(id);
            return new PostResponse(post);
        }

        [HttpPost("create")]
        public IActionResult Create([FromHeader] string Authorization, [FromBody] CreatePostRequest newPost)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string username;
            string password;
            try
            {
                // check username and password
                string[] splitHeader = Authorization.Split(" ");
                string base64EncodedUsernameAndPassword = splitHeader[1];
                byte[] usernameAndPasswordBytes = Convert.FromBase64String(base64EncodedUsernameAndPassword);
                string usernameAndPassword = System.Text.Encoding.UTF8.GetString(usernameAndPasswordBytes);
                string[] splitUsernameAndPassword = usernameAndPassword.Split(":");
                username = splitUsernameAndPassword[0];
                password = splitUsernameAndPassword[1];
            }
            catch (Exception e)
            {
                return Unauthorized("You did not pass authorization");
            }
            
            if (!_users.CheckUsernameAndPassword(username, password))
            {
                return Unauthorized("Username and password combination is not valid");
            }

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
            var post = _posts.Update(id, update);
            return new PostResponse(post);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            _posts.Delete(id);
            return Ok();
        }
    }
}