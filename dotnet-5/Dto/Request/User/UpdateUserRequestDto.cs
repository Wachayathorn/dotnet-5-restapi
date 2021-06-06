using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_5.Dto.Request.User
{
    public class UpdateUserRequestDto
    {
        [Required]
        public string id { get; set; }

        [Required]
        public string fname { get; set; }

        [Required]
        public string lname { get; set; }
    }
}
