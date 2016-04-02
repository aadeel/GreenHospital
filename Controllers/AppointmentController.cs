using DoctorPatient.Infrastructure;
using DoctorPatient.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;

using System.Web.Mvc;

namespace DoctorPatient.Controllers
{
    public class AppointmentController : Controller
    {
        private HospitalDbContext db = new HospitalDbContext();

        // GET: /Appointments/
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var appointments = db.Appointments.Include(a => a.Doctor).OrderByDescending(a => a.Date);
            return View(appointments.ToList());
        }

        // GET: /Appointments/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppointmentModel appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return View("Error");
            }
            return View(appointment);
        }

        // GET: /Appointments/Create
        [Authorize(Roles = "Patient")]
        public ActionResult Create()
        {
            ViewBag.DoctorID = new SelectList(db.Doctors.Where(x => x.DisableNewAppointments == false), "ID", "Name");
            ViewBag.TimeBlockHelper = new SelectList(String.Empty);
            var model = new AppointmentModel
            {
                UserID = User.Identity.GetUserId()
            };
            return View(model);
        }

        // POST: /Appointments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Patient")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AppointmentID,DoctorID,Date,TimeBlockHelper,Time")] AppointmentModel appointment)
        {
            //Add userID
            appointment.UserID = User.Identity.GetUserId();

            //Verify Time
            if (appointment.TimeBlockHelper == "DONT")
                ModelState.AddModelError("", "No Appointments Available for " + appointment.Date.ToShortDateString());
            else
            {
                //Set Time
                appointment.Time = DateTime.Parse(appointment.TimeBlockHelper);

                //CheckWorkingHours
                DateTime start = appointment.Date.Add(appointment.Time.TimeOfDay);
                DateTime end = (appointment.Date.Add(appointment.Time.TimeOfDay)).AddMinutes(double.Parse(db.Administrations.Find(1).Value));
                if (!(BuisnessLogic.IsInWorkingHours(start, end)))
                    ModelState.AddModelError("", "Doctor Working Hours are from " + int.Parse(db.Administrations.Find(2).Value) + " to " + int.Parse(db.Administrations.Find(3).Value));

                //Check Appointment Clash
                string check = BuisnessLogic.ValidateNoAppoinmentClash(appointment);
                if (check != "")
                    ModelState.AddModelError("", check);
            }

            //Continue Normally
            if (ModelState.IsValid)
            {
                db.Appointments.Add(appointment);
                db.SaveChanges();
                return RedirectToAction("Details", new { Controller = "RegisteredUsers", Action = "Details" });
            }

            //Fill Neccessary ViewBags
            ViewBag.DoctorID = new SelectList(db.Doctors.Where(x => x.DisableNewAppointments == false), "ID", "Name", appointment.DoctorID);
            ViewBag.TimeBlockHelper = new SelectList(String.Empty);
            return View(appointment);
        }

        // GET: /Appointments/Edit/5
        [Authorize(Roles = "Admin, Patient")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppointmentModel appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return View("Error");
            }
            ViewBag.TimeBlockHelper = new SelectList(String.Empty);
            ViewBag.Date = appointment.Date.Date;
            ViewBag.DoctorID = new SelectList(db.Doctors.Where(x => x.DisableNewAppointments == false), "ID", "Name", appointment.DoctorID);
            return View(appointment);
        }

        // POST: /Appointments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "AppointmentID,DoctorID,Date,TimeBlockHelper,Available")] AppointmentModel appointment)
        {
            //Verify Time
            if (appointment.TimeBlockHelper == "DONT")
                ModelState.AddModelError("", "No Appointments Available for " + appointment.Date.ToShortDateString());
            else
            {
                //Set Time
                appointment.Time = DateTime.Parse(appointment.TimeBlockHelper);
                //Check WorkingHours
                DateTime start = new DateTime(appointment.Date.Year, appointment.Date.Month, appointment.Date.Day, appointment.Time.Hour, appointment.Time.Minute, appointment.Time.Second);
                DateTime end = new DateTime(appointment.Date.Year, appointment.Date.Month, appointment.Date.Day, appointment.Time.Hour, appointment.Time.Minute, appointment.Time.Second).AddMinutes(double.Parse(db.Administrations.Find(1).Value));
                if (!BuisnessLogic.IsInWorkingHours(start, end))
                    ModelState.AddModelError("", "Doctor Working Hours are from " + int.Parse(db.Administrations.Find(2).Value) + " to " + int.Parse(db.Administrations.Find(3).Value));
            }

            //Continue
            if (ModelState.IsValid)
            {
                db.Entry(appointment).State = EntityState.Modified;
                db.Entry(appointment).Property(u => u.UserID).IsModified = false;
                db.SaveChanges();
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Details", new { Controller = "RegisteredUsers", Action = "Details" });

            }
            ViewBag.TimeBlockHelper = new SelectList(String.Empty);
            ViewBag.DoctorID = new SelectList(db.Doctors.Where(x => x.DisableNewAppointments == false), "ID", "Name", appointment.DoctorID);
            return View(appointment);
        }

        // GET: /Appointments/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppointmentModel appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return View("Error");
            }
            return View(appointment);
        }

        // POST: /Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            AppointmentModel appointment = db.Appointments.Find(id);
            db.Appointments.Remove(appointment);
            db.SaveChanges();
            if (User.IsInRole("Admin"))
                return RedirectToAction("Index");
            else if (User.IsInRole("Doctor"))
                return RedirectToAction("Details", new { Controller = "Doctor", Action = "UpcomingAppointments", id = User.Identity.Name });
            else if (User.IsInRole("Patient"))
                return RedirectToAction("Details", new { Controller = "RegisteredUsers", Action = "Details" });
            else
                return RedirectToAction("Index", new { Controller = "Home", Action = "Index" });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //Here or in model?
        [HttpPost]
        public JsonResult GetAvailableAppointments(int docID, DateTime date)
        {
            List<SelectListItem> resultlist = BuisnessLogic.AvailableAppointments(docID, date);
            return Json(resultlist);
        }
    }
}
