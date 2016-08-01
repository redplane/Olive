﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class OlivesHealthEntities : DbContext
    {
        public OlivesHealthEntities()
            : base("name=OlivesHealthEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AccountCode> AccountCodes { get; set; }
        public virtual DbSet<Addiction> Addictions { get; set; }
        public virtual DbSet<Allergy> Allergies { get; set; }
        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<BloodPressure> BloodPressures { get; set; }
        public virtual DbSet<Doctor> Doctors { get; set; }
        public virtual DbSet<ExperimentNote> ExperimentNotes { get; set; }
        public virtual DbSet<Heartbeat> Heartbeats { get; set; }
        public virtual DbSet<JunkFile> JunkFiles { get; set; }
        public virtual DbSet<MedicalCategory> MedicalCategories { get; set; }
        public virtual DbSet<MedicalImage> MedicalImages { get; set; }
        public virtual DbSet<MedicalNote> MedicalNotes { get; set; }
        public virtual DbSet<MedicalRecord> MedicalRecords { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<Place> Places { get; set; }
        public virtual DbSet<Prescription> Prescriptions { get; set; }
        public virtual DbSet<PrescriptionImage> PrescriptionImages { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }
        public virtual DbSet<RealTimeConnection> RealTimeConnections { get; set; }
        public virtual DbSet<Relation> Relations { get; set; }
        public virtual DbSet<Specialty> Specialties { get; set; }
        public virtual DbSet<SugarBlood> SugarBloods { get; set; }
    }
}
