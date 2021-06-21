using System;
using Microsoft.Extensions.Configuration;

namespace dotnet_5
{
    public class EnvConfiguration
    {
        public const string SectionName = "EnvConfiguration";

        public long UPLOAD_FILE_SIZE { get; set; }
    }
}
