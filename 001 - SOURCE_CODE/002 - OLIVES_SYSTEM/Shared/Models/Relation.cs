//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Shared.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Relation
    {
        public int Id { get; set; }
        public int Source { get; set; }
        public int Target { get; set; }
        public double Created { get; set; }
        public byte Status { get; set; }
    
        public virtual Doctor Doctor { get; set; }
        public virtual Patient Patient { get; set; }
    }
}
