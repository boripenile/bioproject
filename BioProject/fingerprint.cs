//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BioProject
{
    using System;
    using System.Collections.Generic;
    
    public partial class fingerprint
    {
        public long id { get; set; }
        public long studentId { get; set; }
        public byte[] leftThumb { get; set; }
        public byte[] leftIndex { get; set; }
        public byte[] rightThumb { get; set; }
        public byte[] rightIndex { get; set; }
    
        public virtual student student { get; set; }
    }
}
