using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BioProject.Dtos
{
    public class DepartmentDTO
    {
        public Int64? FacultyId { get; set; }

        public Int64 Id { get; set; }

        public string DepartmentName { get; set; }

    }
}