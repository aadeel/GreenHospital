namespace DoctorPatient.Migrations
{
    using DoctorPatient.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<DoctorPatient.Infrastructure.HospitalDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "DoctorPatient.Infrastructure.HospitalDbContext";
        }

        protected override void Seed(DoctorPatient.Infrastructure.HospitalDbContext context)
        {
            var admin = new List<AdministrationModel>
            {
                 new AdministrationModel { Name = "Appointment Duration in Minutes [Default:30]",   Value = "30"},
                 new AdministrationModel { Name = "Working Hours Start in 24-Hour Format [Default:8]",   Value = "8"},
                 new AdministrationModel { Name = "Working Hours End in 24-Hour Format [Default:18]",   Value = "18"},
            };
            admin.ForEach(s => context.Administrations.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();
            this.AddUserAndRoles();
        }

        bool AddUserAndRoles()
        {
            bool success = false;

            var idManager = new IdentityManager();
            success = idManager.CreateRole("Admin");
            if (!success == true) return success;

            success = idManager.CreateRole("Doctor");
            if (!success == true) return success;

            success = idManager.CreateRole("Patient");
            if (!success) return success;

            var newUser = new ApplicationUser()
            {
                UserName = "Admin",
                Name = "Administrator",
                Sex = Enums.Sex.Male,
                BirthDate = DateTime.Parse("14-10-1992"),
            };

            success = idManager.CreateUser(newUser, "12345678");
            if (!success) return success;

            success = idManager.AddUserToRole(newUser.Id, "Admin");
            if (!success) return success;

            return success;
        }
    }
}