using DoctorPatient.Infrastructure;
using DoctorPatient.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace DoctorPatient.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private HospitalDbContext db = new HospitalDbContext();

        // GET: /Administration/
        public ActionResult Index()
        {
            return View(db.Administrations.ToList());
        }

        // GET: /Administration/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdministrationModel Administrationmodel = db.Administrations.Find(id);
            if (Administrationmodel == null)
            {
                return View("Error");
            }
            return View(Administrationmodel);
        }

        // POST: /Administration/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Value")] AdministrationModel Administrationmodel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(Administrationmodel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Administrationmodel);
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
