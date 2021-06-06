using System;
using System.Threading.Tasks;
using dotnet_5.Dto.Request.User;
using dotnet_5.Dto.Response;
using dotnet_5.Models;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_5.Business
{
    public class UserBusiness
    {

        private DOTNET5Context db;

        public UserBusiness(DOTNET5Context db)
        {
            this.db = db;
        }

        public async Task<ActionResult<CreateUserResponseDto>> createUser(CreateUserRequestDto CreateUserRequestDto)
        {
            try
            {
                User user = new User { Fname = CreateUserRequestDto.fname, Lname = CreateUserRequestDto.lname, CreateTime = new DateTime() };
                await db.Users.AddAsync(user);
                await db.SaveChangesAsync();
                return new CreateUserResponseDto { id = user.Id.ToString() , fname = user.Fname , lname = user.Lname , create_time = user.CreateTime.ToString() };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
