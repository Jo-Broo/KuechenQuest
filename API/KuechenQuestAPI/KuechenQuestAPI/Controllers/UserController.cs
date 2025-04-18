﻿using KuechenQuestAPI.Classes;
using KuechenQuestAPI.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace KuechenQuestAPI.Controllers
{
    [Route("KuechenQuest/User")]
    public class UserController : ControllerBase
    {
        private Database database = new Database("SERVER=localhost;DATABASE=KuechenQuest;UID=root;PASSWORD=;");

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            User? user = this.database.GetUserByID(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] MyLoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Invalid input");
            }

            User? user = this.database.Login(request.Username, request.Password);
            if (user == null)
            {
                return BadRequest();
            }

            return Ok(JsonSerializer.Serialize(user));
        }

        [HttpPost("Register")]
        public IActionResult RegisterUser([FromBody] MyRegisterRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Invalid input");
            }

            User? result = this.database.Register(request.Username, request.Email, request.Password);

            if (result == null)
            {
                return Conflict("User already exists"); // Falls User mit dieser Email/Name schon existiert
            }

            return CreatedAtAction(nameof(GetUser), new { id = result.ID }, result);
        }

        [HttpGet("Achievement/{UserID}")]
        public IActionResult GetAchievmentsByUserID(int UserID)
        {
            List<Achievment> achievments = this.database.GetAchievments(UserID);

            if (achievments.Count == 0)
            {
                return StatusCode(500);
            }

            return Ok(JsonSerializer.Serialize(achievments));
        }
    }
}
