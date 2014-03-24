using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Branding365Web.Controllers
{
    [SharePointContextFilter]
    public class DataController : Controller
    {
        protected ClientContext ClientContext { get; private set; }
        protected SharePointContext Context { get; private set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            Context = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            if (Context == null)
                return;
            ClientContext = Context.CreateUserClientContextForSPHost();
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            if (ClientContext != null)
                ClientContext.Dispose();
            ClientContext = null;
            Context = null;
        }

        protected RedirectToRouteResult RedirectToSPAction(string name)
        {
            return RedirectToAction(name, new { Context.SPHostUrl });
        }
    }
}