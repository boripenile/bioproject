using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BioProject.Dtos
{
    public class StudentRegistrationDTO
    {
        public StudentDTO Biodata { get; set; }

        public FingerPrintDTO Biometric { get; set; }

        public string Picture { get; set; }

    }
}