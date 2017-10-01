using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BioProject.Dtos
{
    public class StudentDTO
    {
        public string MatricNumber { get; set; }

        public string Surname { get; set; }

        public string FirstName { get; set; }

        public string OtherName { get; set; }

        public string Email { get; set; }

        public Int64 DepartmentId { get; set; }

        public bool? Status { get; set; }

    }
}