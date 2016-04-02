using DoctorPatient.Infrastructure;
using DoctorPatient.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace DoctorPatient.Controllers
{
    public class DoctorController : Controller
    {
        private HospitalDbContext db = new HospitalDbContext();

        // GET: /Doctors/
        public ActionResult Index(string docDept, string searchstring)
        {
            var deptList = new List<Enums.HospitalDepartment>();
            deptList = Enum.GetValues(typeof(Enums.HospitalDepartment))
                                    .Cast<Enums.HospitalDepartment>()
                                    .ToList();
            ViewBag.docDept = new SelectList(deptList);

            var doctors = from d in db.Doctors
                          select d;

            if (!string.IsNullOrEmpty(searchstring))
            {
                doctors = doctors.Where(s => s.Name.Contains(searchstring));
            }

            if (!string.IsNullOrEmpty(docDept))
            {
                doctors = doctors.Where(x => x.Department.ToString() == docDept);
            }
            return View(doctors);
        }

        //Doctor Availability
        public ActionResult Availability(int id)
        {
            DoctorModel doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return View("Error");
            }
            AppointmentModel test = new AppointmentModel
            {
                DoctorID = id
            };
            ViewBag.TimeBlockHelper = new SelectList(String.Empty);
            ViewBag.DoctorName = doctor.Name;
            return View(test);
        }

        // GET: /Doctors/UpcomingAppointments/5
        [Authorize(Roles = "Admin, Doctor")]
        public ActionResult UpcomingAppointments(string id, string SearchString)
        {
            int n;
            bool isInt = int.TryParse(id, out n);
            if (!isInt)
            {
                var user = db.ApplicationUsers.First(u => u.UserName == id);
                var model = new EditUserViewModel(user);
                DoctorModel doctor = db.Doctors.First(u => u.Name == user.Name);
                if (doctor == null)
                {
                    return View("Error");
                }
                if (!String.IsNullOrEmpty(SearchString))
                {
                    doctor.Appointments = doctor.Appointments.Where(s => s.ApplicationUser.Name.ToUpper().Contains(SearchString.ToUpper())).ToList();
                }
                doctor.Appointments.Sort();
                return View(doctor);
            }
            else
            {
                if (!User.IsInRole("Admin"))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                DoctorModel doctor = db.Doctors.Find(n);
                if (doctor == null)
                {
                    return View("Error");
                }
                doctor.Appointments.Sort();
                return View(doctor);
            }
        }

        // GET: /Doctors/History
        [Authorize(Roles = "Admin, Doctor")]
        public ActionResult History(string id)
        {
            int n;
            bool isInt = int.TryParse(id, out n);
            if (!isInt)
            {
                var user = db.ApplicationUsers.First(u => u.UserName == id);
                var model = new EditUserViewModel(user);
                DoctorModel doctor = db.Doctors.First(u => u.Name == user.Name);
                if (doctor == null)
                {
                    return View("Error");
                }
                doctor.Appointments.Sort();
                return View(doctor);
            }
            else
            {
                if (!User.IsInRole("Admin"))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                DoctorModel doctor = db.Doctors.Find(n);
                if (doctor == null)
                {
                    return View("Error");
                }
                doctor.Appointments.Sort();
                return View(doctor);
            }
        }

        // GET: /Doctors/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Doctors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more UpcomingAppointments see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,BirthDate,Sex,Department,Degree")] DoctorModel doctor)
        {
            if (ModelState.IsValid)
            {
                db.Doctors.Add(doctor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(doctor);
        }

        // GET: /Doctors/Edit/5
        [Authorize(Roles = "Admin, Doctor")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorModel doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return View("Error");
            }
            return View(doctor);
        }

        // POST: /Doctors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more UpcomingAppointments see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin, Doctor")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,BirthDate,Sex,Department,Degree,DisableNewAppointments")] DoctorModel doctor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(doctor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(doctor);
        }

        // GET: /Doctors/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorModel doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return View("Error");
            }
            return View(doctor);
        }

        // POST: /Doctors/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DoctorModel doctor = db.Doctors.Find(id);
            db.Doctors.Remove(doctor);
            db.SaveChanges();
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
    }
}
