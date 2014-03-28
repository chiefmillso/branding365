using System;
using Microsoft.SharePoint.Client;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Branding365Web.Services
{
    public class BrandingService : IResolvableService
    {
        private const string Key = "Branding365.Enabled";
        private readonly Logger _logger;
        private HttpServerUtilityBase _server;

        public BrandingService(Logger logger, HttpServerUtilityBase server)
        {
            _logger = logger;
            _server = server;
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

        public void DeployBranding(ClientContext ctx, bool applyToSubsites)
        {
            _logger.AppendLine("Deploying Branding...");

            this.UploadSiteContents(ctx);
            this.UploadTheme(ctx);
            this.UploadMasterPage(ctx);

            SetCustomBranding(true, ctx);
        }

        public void RetractBranding(ClientContext ctx)
        {
            _logger.AppendLine("Retracting Branding...");

            SetCustomBranding(false, ctx);
        }

        public string GetThemeInfo(ClientContext clientContext)
        {
            Web hostWebObj = clientContext.Web;
            ThemeInfo initialThemeInfo = hostWebObj.ThemeInfo;

            // Get the initial theme info for the web. We do not need to load the entire web object here.
            clientContext.Load(hostWebObj, w => w.ThemeInfo, w => w.CustomMasterUrl);

            // Theme component info is available via a method call that must be executed
            var linkShade = initialThemeInfo.GetThemeShadeByName("Hyperlink");
            var titleFont = initialThemeInfo.GetThemeFontByName("title", 1033);

            // Execute
            clientContext.ExecuteQuery();

            // Use the ThemeInfo to show some data about the theme currently applied to the web
            StringBuilder initialInfo = new StringBuilder();
            initialInfo.AppendFormat("Current master page:\r\n {0}\r\n", hostWebObj.CustomMasterUrl);
            initialInfo.AppendFormat("Background Image:\r\n {0}\r\n", initialThemeInfo.ThemeBackgroundImageUri);
            initialInfo.AppendFormat("The \"Hyperlink\" Color for this theme is:\r\n {0}\r\n", linkShade.Value);
            initialInfo.AppendFormat("The \"title\" Font for this theme is:\r\n {0}\r\n", titleFont.Value);
            return initialInfo.ToString();
        }

        private void UploadSiteContents(ClientContext clientContext)
        {
            // First, copy theme files to a temporary location (the web's Site Assets Library)
            Web hostWebObj = clientContext.Web;
            Site hostSiteObj = clientContext.Site;
            Web hostRootWebObj = hostSiteObj.RootWeb;

            // Get the necessary lists & libraries
            List assetLibrary = hostRootWebObj.Lists.GetByTitle("Site Assets");
            List styleLibrary = hostRootWebObj.Lists.GetByTitle("Style Library");

            clientContext.Load(assetLibrary, l => l.RootFolder);
            clientContext.Load(styleLibrary, l => l.RootFolder);

            Upload(clientContext, Listings.ListingTypes.StyleLibrary, styleLibrary);
            Upload(clientContext, Listings.ListingTypes.SiteAsset, assetLibrary);

            clientContext.ExecuteQuery();
        }

        private void Upload(ClientContext clientContext, Listings.ListingTypes listingType, List list, Action<Microsoft.SharePoint.Client.File> action = null)
        {
            foreach (var listing in Listings.Items.Where(x => x.Type == listingType))
            {
                var path = '/' + listing.Source.TrimStart('/');
                var themeFile = new FileInfo(_server.MapPath(path));

                FileCreationInformation newFile = new FileCreationInformation();
                newFile.Content = System.IO.File.ReadAllBytes(themeFile.FullName);
                newFile.Url = themeFile.Name;
                newFile.Overwrite = Overwrite;

                _logger.AppendLine("Uploading: {0}", listing.Source);

                Microsoft.SharePoint.Client.File file = list.RootFolder.Files.Add(newFile);
                if (action != null)
                    action(file);
                clientContext.Load(file);
            }
        }

        private void UploadMasterPage(ClientContext clientContext)
        {
            Web hostWebObj = clientContext.Web;
            Site hostSiteObj = clientContext.Site;
            Web hostRootWebObj = hostSiteObj.RootWeb;

            List masterLibrary = hostRootWebObj.Lists.GetByTitle("Master Page Gallery");
            var contentTypes = masterLibrary.ContentTypes;
            clientContext.Load(contentTypes);
            clientContext.ExecuteQuery();

            var ct = contentTypes.Where(x => x.Name == "Master Page").Single();

            clientContext.Load(masterLibrary, l => l.RootFolder);

            Upload(clientContext, Listings.ListingTypes.MasterPage, masterLibrary, x =>
            {
                var listItem = x.ListItemAllFields;
                listItem["ContentTypeId"] = ct.Id.StringValue;
                listItem["UIVersion"] = 15;
                listItem.Update();

                CheckInPublishApprove(clientContext, listItem, masterLibrary);
            });
        }

        private void UploadTheme(ClientContext clientContext)
        {
            Web hostWebObj = clientContext.Web;
            Site hostSiteObj = clientContext.Site;
            Web hostRootWebObj = hostSiteObj.RootWeb;

            List themeLibrary = hostRootWebObj.Lists.GetByTitle("Theme Gallery");
            Folder themeFolder = themeLibrary.RootFolder.Folders.GetByUrl("15");
            List looksGallery = hostRootWebObj.Lists.GetByTitle("Composed Looks");
            List masterLibrary = hostRootWebObj.Lists.GetByTitle("Master Page Gallery");
            List assetLibrary = hostRootWebObj.Lists.GetByTitle("Site Assets");

            clientContext.Load(themeFolder, f => f.ServerRelativeUrl);
            clientContext.Load(masterLibrary, l => l.RootFolder);
            clientContext.Load(assetLibrary, l => l.RootFolder);

            foreach (var listing in Listings.Items.Where(x => x.Type == Listings.ListingTypes.Theme))
            {
                var path = '/' + listing.Source.TrimStart('/');
                var themeFile = new FileInfo(_server.MapPath(path));

                FileCreationInformation newFile = new FileCreationInformation();
                newFile.Content = System.IO.File.ReadAllBytes(themeFile.FullName);
                newFile.Url = themeFile.Name;
                newFile.Overwrite = Overwrite;

                _logger.AppendLine("Uploading: {0}", listing.Source);

                switch (themeFile.Extension)
                {
                    case ".spcolor":
                    case ".spfont":
                        Microsoft.SharePoint.Client.File uploadTheme = themeFolder.Files.Add(newFile);
                        clientContext.Load(uploadTheme);
                        break;
                    case ".master":
                    case ".html":
                        Microsoft.SharePoint.Client.File updloadMaster = masterLibrary.RootFolder.Files.Add(newFile);
                        clientContext.Load(updloadMaster);
                        break;
                    default:
                        Microsoft.SharePoint.Client.File uploadAsset = assetLibrary.RootFolder.Files.Add(newFile);
                        clientContext.Load(uploadAsset);
                        break;
                }

            }
            clientContext.ExecuteQuery();
        }

        public bool Overwrite { get; set; }

        private void CheckInPublishApprove(ClientContext clientContext, ListItem listItem, List list)
        {
            clientContext.Load(listItem, l => l.File, l => l.Id);
            clientContext.Load(list, l => l.EnableVersioning, l => l.EnableModeration, l => l.EnableMinorVersions, l => l.EnableModeration);
            clientContext.ExecuteQuery();

            var file = listItem.File;

            if (list.EnableVersioning && list.EnableMinorVersions)
            {
                file.Publish("");
                file = list.GetItemById(listItem.Id).File;
            }

            if (list.EnableModeration)
            {
                file.Approve("");
            }

            clientContext.ExecuteQuery();
        }
    }
}