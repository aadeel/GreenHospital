using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoctorPatient.Models
{
    public class AppointmentModel : IComparable<AppointmentModel>
    {
        [Key]
        public int AppointmentID { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserID { get; set; }

        [Required] //Changes V2
        public int DoctorID { get; set; }

        [Required]
        [Display(Name = "Date for Appointment")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [MyAppointmentDateValidation(ErrorMessage = "Are you creating an appointment for the past?")]
        public DateTime Date { get; set; }

        //Disabling due to variable appointment times now. 
        //[MyTimeValidation(ErrorMessage="Appointments only available for HH:00 and HH:30")]
        [DataType(DataType.Time)]
        public DateTime Time { get; set; }

        public string TimeBlockHelper { get; set; }

        public virtual DoctorModel Doctor { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public int CompareTo(AppointmentModel other)
        {
            return this.Date.Date.Add(this.Time.TimeOfDay).CompareTo(other.Date.Date.Add(other.Time.TimeOfDay));
        }
    }
}