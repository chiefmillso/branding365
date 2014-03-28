using System;
using System.Collections.Generic;

namespace Branding365Web
{
    public class Listings
    {
		public enum ListingTypes {
			MasterPage,
			Theme,
			Page,
			Script,
			SiteAsset,
			StyleLibrary,
			Unknown
		}

		public class Listing {
			public string Source { get; set; }
			public ListingTypes Type { get; set; }

		}

		public static string VersionNumber { get { return "1.0.0.0";}}

		public static readonly IList<Listing> Items = new List<Listing>();

		static Listings()
		{
			Items.Add(new Listing() { Source = @"Deployment/Master Page Gallery/Responsive.master",
									  Type = ListingTypes.MasterPage });
			Items.Add(new Listing() { Source = @"Deployment/Style Library/OBS/bootstrap2-custom.min.css",
									  Type = ListingTypes.StyleLibrary });
			Items.Add(new Listing() { Source = @"Deployment/Style Library/OBS/bootstrap3-custom.min.css",
									  Type = ListingTypes.StyleLibrary });
			Items.Add(new Listing() { Source = @"Deployment/Style Library/OBS/bootstrap3.min.css",
									  Type = ListingTypes.StyleLibrary });
			Items.Add(new Listing() { Source = @"Deployment/Style Library/OBS/JavaScript/bootstrap3-custom.js",
									  Type = ListingTypes.StyleLibrary });
			Items.Add(new Listing() { Source = @"Deployment/Style Library/OBS/JavaScript/bootstrap3.min.js",
									  Type = ListingTypes.StyleLibrary });
			Items.Add(new Listing() { Source = @"Deployment/Style Library/OBS/JavaScript/jquery-1.9.1.js",
									  Type = ListingTypes.StyleLibrary });
			Items.Add(new Listing() { Source = @"Deployment/Style Library/OBS/sp-responsive.css",
									  Type = ListingTypes.StyleLibrary });
			Items.Add(new Listing() { Source = @"Deployment/Theme Gallery/ThemeSample.spcolor",
									  Type = ListingTypes.Theme });
			Items.Add(new Listing() { Source = @"Deployment/Theme Gallery/ThemeSample.spfont",
									  Type = ListingTypes.Theme });
		}
	}
}


