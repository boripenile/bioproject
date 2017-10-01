using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BioProject.Response
{
    public class VerifiedStudentResponseDTO
    {
        public Int64 StudentId { get; set; }
        public string MatricNumber { get; set; }

        public string Surname { get; set; }

        public string FirstName { get; set; }

        public string OtherName { get; set; }

        public string Email { get; set; }

        public string UniversityName { get; set; }

        public string Faculty { get; set; }

        public string Department { get; set; }

        public bool Status { get; set; }

        public string Picture { get; set; }
    }
}