using Branding365Web.Models;
using Branding365Web.Services;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Branding365Web.Controllers
{
    public class HomeController : DataController
    {

        private readonly BrandingService _brandingService;

        public HomeController(BrandingService brandingService)
        {
            _brandingService = brandingService;
        }

        public ActionResult Index()
        {
            var model = new HomeIndexViewModel();
            if (ClientContext != null)
            {
                var spWeb = ClientContext.Web;
                User spUser = ClientContext.Web.CurrentUser;

                ClientContext.Load(spWeb, web => web.Title);
                ClientContext.Load(spUser, user => user.Title);

                ClientContext.ExecuteQuery();

                model.SiteName = spWeb.Title;
                model.UserName = spUser.Title;
                model.HasCustomBranding = _brandingService.HasCustomBranding(ClientContext);

                model.VersionNumber = Listings.VersionNumber;
                model.Items = Listings.Items;
            }

            return View(model);
        }

        public ActionResult DeployBranding(DeployBrandingModel model)
        {
            _brandingService.DeployBranding(ClientContext, model.ApplyToSubSites, model.OverrideExistingFiles);

            return RedirectToSPAction("Index");
        }

        public ActionResult RetractBranding()
        {
            _brandingService.RetractBranding(ClientContext);

            return RedirectToSPAction("Index");
        }
    }
}
