using System;
using System.Collections.Generic;

#nullable disable

namespace dotnet_5.Models
{
    public partial class User
    {
        public string Fname { get; set; }
        public string Lname { get; set; }
        public DateTime? CreateTime { get; set; }
        public long Id { get; set; }
    }
}
