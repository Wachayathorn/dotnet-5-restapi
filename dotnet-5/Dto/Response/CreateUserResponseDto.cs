using System;
using dotnet_5.Models;

namespace dotnet_5.Dto.Response
{
    public class CreateUserResponseDto
    {
        public string id { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string create_time { get; set; }
    }
}
