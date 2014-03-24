using System;
using Microsoft.SharePoint.Client;

namespace Branding365Web.Services
{
    public class BrandingService : IResolvableService
    {
        private const string Key = "Branding365.Enabled";
        private readonly Logger _logger;

        public BrandingService(Logger logger)
        {
            _logger = logger;
        }

        public bool HasCustomBranding(ClientContext ctx)
        {
            Web web = ctx.Web;
            ctx.Load(web, w => w.AllProperties);
            ctx.ExecuteQuery();

            PropertyValues properties = web.AllProperties;

            return HasValue(properties, Key, true);
        }

        public void SetCustomBranding(bool enabled, ClientContext ctx)
        {
            Web web = ctx.Web;

            ctx.Load(web, w => w.AllProperties);
            PropertyValues properties = web.AllProperties;
            properties[Key] = enabled;
            web.Update();

            _logger.AppendLine("Set Custom Branding Property: {0}", enabled);

            ctx.ExecuteQuery();
        }

        private bool HasValue(PropertyValues values, string key, bool value)
        {
            if (!values.FieldValues.ContainsKey(key))
                return false;

            object val = values[key];
            bool result;
            if (bool.TryParse(string.Format("{0}", val), out result))
                return result == value;
            return false;
        }

        public void DeployBranding(ClientContext ctx, bool applyToSubsites, bool overrideExistingFiles)
        {
            _logger.AppendLine("Deploying Branding...");

            // TODO: do stuff here

            SetCustomBranding(true, ctx);
        }

        public void RetractBranding(ClientContext ctx)
        {
            _logger.AppendLine("Retracting Branding...");

            SetCustomBranding(false, ctx);
        }
    }
}