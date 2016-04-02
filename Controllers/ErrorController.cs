using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoctorPatient.Controllers
{
    public class ErrorController : Controller
    {
        // GET: /Error/
        [PreventDirectAccess]
        public ActionResult ServerError()
        {
            return View("Error");
        }

        [PreventDirectAccess]
        public ActionResult AccessDenied()
        {
            return View("Error");
        }

        public ActionResult NotFound()
        {
            return View("Error");
        }

        [PreventDirectAccess]
        public ActionResult OtherHttpStatusCode(int httpStatusCode)
        {
            return View("GenericHttpError", httpStatusCode);
        }

        private class PreventDirectAccessAttribute : FilterAttribute, IAuthorizationFilter
        {
            public void OnAuthorization(AuthorizationContext filterContext)
            {
                object value = filterContext.RouteData.Values["fromAppErrorEvent"];
                if (!(value is bool && (bool)value))
                    filterContext.Result = new ViewResult { ViewName = "Error404" };
            }
        }
    }
}