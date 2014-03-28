using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Branding365Web.Models
{
    public class HomeIndexViewModel
    {
        public string Log { get; set; }

        public string DeployButtonAttributes
        {
            get
            {
                return HasCustomBranding ? "disabled=\"disabled\"" : "";
            }
        }

        public string RetractButtonAttributes
        {
            get
            {
                return !HasCustomBranding ? "disabled=\"disabled\"" : "";
            }
        }

        public HomeIndexViewModel()
        {
            Items = new List<Listings.Listing>();
        }

        public string SiteName { get; set; }

        public string SiteUrl { get; set; }

        public string UserName { get; set; }

        public bool HasCustomBranding { get; set; }

        public string VersionNumber { get; set; }

        public IList<Listings.Listing> Items { get; set; }
    }
}