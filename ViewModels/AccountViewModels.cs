using DoctorPatient.Infrastructure;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoctorPatient.Models
{
    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        //New fields to extend ApplicationUser class.
        [Required]
        [StringLength(60, MinimumLength = 3)]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [MyBirthDateValidation(ErrorMessage = "You cannot be from the Future!")]

        public DateTime BirthDate { get; set; }

        [Required]
        public Enums.Sex Sex { get; set; }

        public Boolean Blocked { get; set; }

        public virtual List<AppointmentModel> Appointments { get; set; }

        // Return a pre populated instance of ApplicationUser, helps in controller!
        public ApplicationUser GetUser()
        {
            var user = new ApplicationUser()
            {
                UserName = this.UserName,
                Name = this.Name,
                BirthDate = this.BirthDate,
                Sex = this.Sex,
                Appointments = this.Appointments,
                Blocked = this.Blocked,
            };
            return user;
        }
    }

    //Not sure if needed or not. Check later. 
    public class EditUserViewModel
    {
        //Attributes
        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 3)]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [MyBirthDateValidation(ErrorMessage = "You cannot be from the Future!")]
        public DateTime BirthDate { get; set; }

        [Required]
        public Enums.Sex Sex { get; set; }

        public Boolean Blocked { get; set; }

        public virtual List<AppointmentModel> Appointments { get; set; }

        //Constructors
        public EditUserViewModel() { }

        // Allow Initialization with an instance of ApplicationUser:
        public EditUserViewModel(ApplicationUser user)
        {
            this.UserName = user.UserName;
            this.Name = user.Name;
            this.BirthDate = user.BirthDate;
            this.Sex = user.Sex;
            this.Appointments = user.Appointments;
            this.Blocked = user.Blocked;
        }
    }

    public class SelectUserRolesViewModel
    {
        //Attributes - No DataAnnotations here, WHy?
        public string UserName { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public Enums.Sex Sex { get; set; }
        public Boolean Blocked { get; set; }
        public List<SelectRoleEditorViewModel> Roles { get; set; }
        public virtual List<AppointmentModel> Appointments { get; set; }

        //Constructors
        public SelectUserRolesViewModel()
        {
            this.Roles = new List<SelectRoleEditorViewModel>();
        }

        // Enable initialization with an instance of ApplicationUser:
        public SelectUserRolesViewModel(ApplicationUser user)
            : this()  //This this(), WHy?
        {
            this.UserName = user.UserName;
            this.Name = user.Name;
            this.BirthDate = user.BirthDate;
            this.Sex = user.Sex;
            this.Appointments = user.Appointments;
            this.Blocked = user.Blocked;

            var Db = new HospitalDbContext();

            // Add all available roles to the list of EditorViewModels:
            var allRoles = Db.Roles;
            foreach (var role in allRoles)
            {
                // An EditorViewModel will be used by Editor Template:
                var rvm = new SelectRoleEditorViewModel(role);
                this.Roles.Add(rvm);
            }

            // Set the Selected property to true for those roles for 
            // which the current user is a member:
            //Update Fix applied.
            IdentityManager im = new IdentityManager();
            foreach (var userRole in user.Roles)
            {
                string roleName = im.RoleName(userRole.RoleId);
                var checkUserRole = this.Roles.Find(r => r.RoleName == roleName); // userRole.Role.Name
                checkUserRole.Selected = true;
            }
        }
    }

    // Used to display a single role with a checkbox, within a list structure:
    public class SelectRoleEditorViewModel
    {
        //Attributes
        public bool Selected { get; set; }

        [Required]
        public string RoleName { get; set; }

        //Constructors
        public SelectRoleEditorViewModel() { }
        public SelectRoleEditorViewModel(IdentityRole role)
        {
            this.RoleName = role.Name;
        }
    }
}
