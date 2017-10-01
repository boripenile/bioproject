using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BioProject.Response
{
    public class UniversityAdminResponseDTO
    {
        public Int64 Id { get; set; }
        public string UniversityCode { get; set; }

        public String UniversityName { get; set; }

        public String UniversityAddress { get; set; }

        public String EmailAddress { get; set; }

        public String PhoneNumber { get; set; }

        public bool Activated { get; set; }
    }
}