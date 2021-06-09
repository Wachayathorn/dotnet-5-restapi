using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_5.Dto.Request.User;
using dotnet_5.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_5.Controllers
{
    [Route("/user")]
    public partial class UserController : Controller
    {
        private DOTNET5Context db;
        public UserController(DOTNET5Context db)
        {
            this.db = db;
        }

        [HttpPost]
        public async Task<ActionResult> createUser([FromBody] CreateUserRequestDto data) {
            try
            {
                DateTime dateNow = DateTime.Now;
                User user = new User { Fname = data.fname, Lname = data.lname, CreateTime = dateNow};
                await db.Users.AddAsync(user);
                await db.SaveChangesAsync();
                return StatusCode(StatusCodes.Status201Created, user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet]
        public async Task<ActionResult> getUser() 
        {
            try
            {
                return StatusCode(StatusCodes.Status200OK, db.Users.ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> getUserById(string id)
        {
            try
            {
                var user = from _user in db.Users
                           where _user.Id.Equals(long.Parse(id))
                           select _user;

                return StatusCode(StatusCodes.Status200OK, user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPut]
        public async Task<ActionResult> updateUser([FromBody] UpdateUserRequestDto data)
        {
            try
            {
                DateTime dateNow = DateTime.Now;
                var user = db.Users.Where(x => x.Id.Equals(long.Parse(data.id))).First();
                user.Fname = data.fname;
                user.Lname = data.lname;
                user.CreateTime = dateNow;
                await db.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK, user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> deleteUser(string id) 
        {
            try
            {
                var user = db.Users.Where(x => x.Id.Equals(long.Parse(id))).FirstOrDefault();
                db.Users.Remove(user);
                await db.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
