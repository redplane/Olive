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
    
    public partial class Prescription
    {
        public int Id { get; set; }
        public int Owner { get; set; }
        public int MedicalRecordId { get; set; }
        public double From { get; set; }
        public double To { get; set; }
        public string Note { get; set; }
        public double Created { get; set; }
        public Nullable<double> LastModified { get; set; }
    
        public virtual MedicalRecord MedicalRecord { get; set; }
        public virtual Person Person { get; set; }
    }
}
