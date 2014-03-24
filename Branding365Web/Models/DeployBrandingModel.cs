using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Branding365Web.Models
{
    public class DeployBrandingModel
    {
        public bool OverrideExistingFiles { get; set; }
        public bool ApplyToSubSites { get; set; }
    }
}