using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BioProject.Response
{
    public class UniversityAdminLiteResponseDTO
    {
        public Int64 Id { get; set; }
        public string EmailAddress { get; set; }

        public string Password { get; set; }

        public bool Super { get; set; }
    }
}