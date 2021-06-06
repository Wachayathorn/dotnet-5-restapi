using System;
using System.Collections.Generic;
using System.Linq;
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
                return new CreateUserResponseDto { id = user.Id.ToString(), fname = user.Fname, lname = user.Lname, create_time = user.CreateTime.ToString() };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ActionResult<IEnumerable<User>>> getUser()
        {
            try
            {
                return db.Users.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ActionResult<User>> getUserById(string id)
        {
            try
            {
                var user = from _user in db.Users
                           where _user.Id.Equals(id)
                           select _user;

                return user.First();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ActionResult<User>> updateUser(UpdateUserRequestDto data)
        {
            try
            {
                var user = db.Users.Where(x => x.Id.Equals(data.id)).First();

                if (user == null)
                {
                    return new BadRequestResult();
                }

                user.Fname = data.fname;
                user.Lname = data.lname;
                await db.SaveChangesAsync();

                return user;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ActionResult<OkResult>> deleteUser(string id) 
        {
            try
            {
                var user = db.Users.Where(x => x.Id.Equals(id)).FirstOrDefault();
                db.Users.Remove(user);
                await db.SaveChangesAsync();
                return new OkResult();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
