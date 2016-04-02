using DoctorPatient.Infrastructure;
using DoctorPatient.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DoctorPatient.Controllers
{
    public class RegisteredUsersController : Controller
    {
        private HospitalDbContext db = new HospitalDbContext();

        // GET: /RegisteredUser/
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var users = db.ApplicationUsers;    //Users to Applicationusers
            var model = new List<EditUserViewModel>();
            foreach (var user in users)
            {
                var u = new EditUserViewModel(user);
                model.Add(u);
            }
            return View(model);
        }

        // GET: /RegisteredUser/Details/userName
        [Authorize]
        public ActionResult Details()
        {
            string id = User.Identity.Name;
            var Db = new HospitalDbContext();
            var user = Db.ApplicationUsers.First(u => u.UserName == id);
            var model = new EditUserViewModel(user);
            model.Appointments.Sort();
            return View(model);
        }

        // GET: /RegisteredUser/Edit/userName
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id, DoctorPatient.Controllers.AccountController.ManageMessageId? Message = null)
        {
            var Db = new HospitalDbContext();
            var user = Db.ApplicationUsers.First(u => u.UserName == id);
            var model = new EditUserViewModel(user);
            ViewBag.MessageId = Message;
            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var Db = new HospitalDbContext();
                var user = Db.ApplicationUsers.First(u => u.UserName == model.UserName);

                // Update the user data:
                user.Name = model.Name;
                user.BirthDate = model.BirthDate;
                user.Sex = model.Sex;
                user.Appointments = model.Appointments;
                user.Blocked = model.Blocked;
                Db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                await Db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /RegisteredUser/Delete/5
        [Authorize(Roles = "Admin, Patient")]
        public ActionResult Delete(string id = null)
        {
            var Db = new HospitalDbContext();
            var user = Db.ApplicationUsers.First(u => u.UserName == id);
            var model = new EditUserViewModel(user);
            if (user == null)
            {
                return View("Error");
            }
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Patient")]
        public ActionResult DeleteConfirmed(string id)
        {
            var Db = new HospitalDbContext();
            var user = Db.ApplicationUsers.First(u => u.UserName == id);
            Db.Users.Remove(user);
            Db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //USER ROLES!!
        [Authorize(Roles = "Admin")]
        public ActionResult UserRoles(string id)
        {
            var Db = new HospitalDbContext();
            var user = Db.ApplicationUsers.First(u => u.UserName == id);
            var model = new SelectUserRolesViewModel(user);
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult UserRoles(SelectUserRolesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var idManager = new IdentityManager();
                var Db = new HospitalDbContext();
                var user = Db.ApplicationUsers.First(u => u.UserName == model.UserName);
                idManager.ClearUserRoles(user.Id);
                foreach (var role in model.Roles)
                {
                    if (role.Selected)
                    {
                        idManager.AddUserToRole(user.Id, role.RoleName);
                    }
                }
                return RedirectToAction("index");
            }
            return View();
        }
    }
}