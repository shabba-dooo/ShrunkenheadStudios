using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShrunkenHeadStudios.Controllers
{
    public class ErrorController : Controller
    {
        //Error controller that contains actions to return views for error codes.
        [AllowAnonymous]
        public ActionResult Oops(int id)
        {
            Response.StatusCode = id;
            return View();
        }

    }
}
