using System;
using System.ComponentModel.DataAnnotations;

namespace DoctorPatient.Models
{
    public class MyAppointmentDateValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime dt = (DateTime)value;
            DateTime today = DateTime.Now;
            if (dt.Date.Date.CompareTo(today.Date) >= 0)
                return true;
            else
                return false;
        }
    }

}