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
        private readonly Logger _logger;

        public HomeController(BrandingService brandingService, Logger logger)
        {
            _brandingService = brandingService;
            _logger = logger;
        }

        public ActionResult Index()
        {
            var model = new HomeIndexViewModel();
            if (ClientContext != null)
            {
                var spWeb = ClientContext.Web;
                User spUser = ClientContext.Web.CurrentUser;

                ClientContext.Load(spWeb, web => web.Title, web => web.Url);
                ClientContext.Load(spUser, user => user.Title);

                ClientContext.ExecuteQuery();

                model.SiteName = spWeb.Title;
                model.SiteUrl = spWeb.Url;
                model.UserName = spUser.Title;
                model.HasCustomBranding = _brandingService.HasCustomBranding(ClientContext);
                model.ThemeInfo = _brandingService.GetThemeInfo(ClientContext);

                model.VersionNumber = Listings.VersionNumber;
                model.Items = Listings.Items;
                model.Log = _logger.ToString();
                
                _logger.Clear();
            }

            return View(model);
        }

        public ActionResult DeployBranding(DeployBrandingModel model)
        {
            _brandingService.Overwrite = model.OverrideExistingFiles;
            _brandingService.DeployBranding(ClientContext, model.ApplyToSubSites);

            return RedirectToSPAction("Index");
        }

        public ActionResult RetractBranding()
        {
            _brandingService.RetractBranding(ClientContext);

            return RedirectToSPAction("Index");
        }
    }
}
