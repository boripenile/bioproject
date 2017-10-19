using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BioProject.Dtos
{
    public class FingerPrint
    {
        public string FingerImage { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Resolution { get; set; }
    }
}