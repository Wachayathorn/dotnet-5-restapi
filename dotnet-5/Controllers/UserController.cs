using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_5.Business;
using dotnet_5.Dto.Request.User;
using dotnet_5.Dto.Response;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_5.Controllers
{
    [Route("api/user")]
    public partial class UserController : Controller
    {
        private UserBusiness userBusiness;
        public UserController(UserBusiness userBusiness)
        {
            this.userBusiness = userBusiness;
        }

        [Route("create")]
        [HttpPost]
        public async Task<ActionResult<CreateUserResponseDto>> createUser([FromBody] CreateUserRequestDto data) {
            return await this.userBusiness.createUser(data);
        }
    }
}
