using System;
using System.ComponentModel.DataAnnotations;

namespace dotnet_5.Dto.Request.User
{
    public class CreateUserRequestDto
    {
        [Required]
        public string fname { get; set; }

        [Required]
        public string lname { get; set; }
    }
}
