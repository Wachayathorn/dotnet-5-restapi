using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_5.Business;
using dotnet_5.Dto.Request.User;
using dotnet_5.Dto.Response;
using dotnet_5.Models;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_5.Controllers
{
    [Route("/user")]
    public partial class UserController : Controller
    {
        private UserBusiness userBusiness;
        public UserController(UserBusiness userBusiness)
        {
            this.userBusiness = userBusiness;
        }

        [HttpPost]
        public async Task<ActionResult<CreateUserResponseDto>> createUser([FromBody] CreateUserRequestDto data) {
            return await this.userBusiness.createUser(data);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> getUser() 
        {
            return await this.userBusiness.getUser();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> getUserById(string id)
        {
            return await this.userBusiness.getUserById(id);
        }

        [HttpPut]
        public async Task<ActionResult<User>> updateUser([FromBody] UpdateUserRequestDto data)
        {
            return await this.userBusiness.updateUser(data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<OkResult>> deleteUser(string id) 
        {
            return await this.userBusiness.deleteUser(id);
        }
    }
}
