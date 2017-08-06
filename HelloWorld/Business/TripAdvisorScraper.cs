using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HelloWorld.Business
{
	public class TripAdvisorScraper
	{
		private string _cityUrl;
		private string _mainUrl = "https://www.tripadvisor.com.ph";
		private DataSet dsItinerary;
		private DataTable dtItinerary;
		private DataSet dsReviews;
		private DataTable dtReviews;

		public string CityUrl
		{
			get { return _cityUrl; }
			set { _cityUrl = value; }
		}

		public string MainUrl
		{
			get { return _mainUrl; }
		}
		public TripAdvisorScraper(string url)
		{
			CityUrl = url;
		}

		public TripAdvisorScraper()
		{
			
		}

		public string GetCompleteLink(string subUrl)
		{
			return MainUrl + subUrl;
		}

		public HtmlDocument GetDocument(string url)
		{
			HtmlWeb webGet = new HtmlWeb();
			HtmlDocument document = webGet.Load(url);

			string clean = HttpUtility.HtmlDecode(document.DocumentNode.InnerHtml);
			// reload as cleaned html
			document.LoadHtml(clean);

			if (document != null)
			{
				return document;
			}
			else
			{
				throw new NullReferenceException("Unable to download document located at: " + url);
			}
		}

		public Task<DataSet> GetTopAttractionsReviews(string attraction, string reviewsUrl)
		{
			return Task.Run(() => GetReviewsData(attraction, reviewsUrl));
		}

		public Task<DataSet> GetTopAttractions()
		{
			return Task.Run(() => GetAttractionsData());
		}

		private void CreateItineraryData()
		{
			dsItinerary = new DataSet();
			dtItinerary = new DataTable("Data");
			dtItinerary.Columns.Add(new DataColumn("name", typeof(string)));
			dtItinerary.Columns.Add(new DataColumn("url", typeof(string)));
			dtItinerary.Columns.Add(new DataColumn("rating", typeof(string)));
			dtItinerary.Columns.Add(new DataColumn("ranking", typeof(string)));
			dsItinerary.Tables.Add(dtItinerary);
		}

		private void CreateReviewsData()
		{
			dsReviews = new DataSet();
			dtReviews = new DataTable("Data");
			dtReviews.Columns.Add(new DataColumn("name", typeof(string)));
			dtReviews.Columns.Add(new DataColumn("title", typeof(string)));
			dtReviews.Columns.Add(new DataColumn("review", typeof(string)));
			dsReviews.Tables.Add(dtReviews);
		}

		private DataSet GetAttractionsData()
		{
			string thingsToDoURL = GetThingsToDoURL(CityUrl);

			HtmlDocument document = GetDocument(thingsToDoURL);

			CreateItineraryData();

			IEnumerable<HtmlNode> topPlaces = document.DocumentNode
						.Descendants("div")
						.Where(d =>
						   d.Attributes.Contains("class")
						   &&
						   d.Attributes["class"].Value.Contains("listing_info"));

			IEnumerable<HtmlNode> nameNode;
			IEnumerable<HtmlNode> hrefNode;
			IEnumerable<HtmlNode> ratingsNode;
			IEnumerable<HtmlNode> spotNode;

			string name = string.Empty;
			string rating = string.Empty;
			string spot = string.Empty;
			string href = string.Empty;

			foreach (HtmlNode topPlace in topPlaces)
			{
				// Create new Attraction Row
				DataRow drNew = dtItinerary.NewRow();
				// Get Attraction Name
				nameNode = topPlace.Descendants("div")
					.Where(d => d.Attributes["class"].Value.Contains("listing_title"));
				name = (ElementFound(nameNode)) ? nameNode.First().InnerText : "No Name Found";
				drNew["name"] = name;

				// Get Attraction TA Link
				hrefNode = topPlace.Descendants("a");
				href = (ElementFound(hrefNode)) ? hrefNode.First().GetAttributeValue("href", null) : "No href Found";
				drNew["url"] = href;

				// Get Attraction Rating
				ratingsNode = topPlace.Descendants("span")
					.Where(d => d.Attributes["class"].Value.Contains("ui_bubble_rating"));
				rating = (ElementFound(ratingsNode)) ? ratingsNode.First().GetAttributeValue("alt", null) : "No Rating Found";
				drNew["rating"] = rating;

				// Get Attraction Ranking
				spotNode = topPlace.Descendants("div")
					.Where(d => d.Attributes["class"].Value.Contains("popRanking wrap") && d.InnerText != "\n");
				spot = (ElementFound(spotNode)) ? spotNode.First().InnerText : "No Ranking Found";
				drNew["ranking"] = spot;

				// Add Attraction Row to Attraction Table
				dtItinerary.Rows.Add(drNew);
			}

			return dsItinerary;
		}

		private bool ElementFound(IEnumerable<HtmlNode> childNodes)
		{
			if (childNodes.Count() > 0)
				return true;

			return false;
		}

		public DataSet GetReviewsData(string attraction, string reviewsUrl)
		{
			CreateReviewsData();

			var reviewLink = GetCompleteLink(reviewsUrl);

			// Get Reviews
			HtmlDocument document = GetDocument(reviewLink);
			IEnumerable<HtmlNode> reviewTitleNode;
			IEnumerable<HtmlNode> reviewContentNode;

			// Get full Review
			IEnumerable<HtmlNode> newReviewNode = GetUserReviews(document);
			if (newReviewNode.Count() > 0) // if there are user reviews
			{
				string freviewLink = newReviewNode.First().GetAttributeValue("href", null); //get link of first review
				string fullreviewLink = GetCompleteLink(freviewLink);
				document = GetDocument(fullreviewLink);

				IEnumerable<HtmlNode> fullReviewNodes = GetFullReviews(document);

				string reviewContent = string.Empty;
				string reviewTitle = string.Empty;

				foreach (HtmlNode review in fullReviewNodes)
				{
					DataRow drNew = dtReviews.NewRow();
					drNew["name"] = attraction;

					reviewTitleNode = review.Descendants("div").Where(d => d.Attributes["class"].Value.Contains("quote"));
					reviewTitle = (ElementFound(reviewTitleNode)) ? reviewTitleNode.First().InnerText : "No Review Title Found";
					drNew["title"] = reviewTitle;

					reviewContentNode = review.Descendants("p").Where(d => d.Attributes.Contains("id"));
					reviewContent = (ElementFound(reviewContentNode)) ? reviewContentNode.First().InnerText : "No Review Content Found";
					drNew["review"] = reviewContent;

					dtReviews.Rows.Add(drNew);
				}
			}

			return dsReviews;
		}

		public DataSet GetReviewsData()
		{
			CreateReviewsData();

			string thingsToDoURL = GetThingsToDoURL(CityUrl);

			HtmlDocument document = GetDocument(thingsToDoURL);

			// Get Top Things To Do list
			IEnumerable<HtmlNode> topPlacesNode = GetTopPlaces(document);
			foreach (HtmlNode topPlace in topPlacesNode)
			{
				var link = topPlace.Descendants("a").First().GetAttributeValue("href", null); //Get url
				var reviewLink = GetCompleteLink(link);

				// Get Reviews
				document = GetDocument(reviewLink);

				// Get full Review
				IEnumerable<HtmlNode> newReviewNode = GetUserReviews(document);
				if (newReviewNode.Count() > 0) // if there are user reviews
				{
					string freviewLink = newReviewNode.First().GetAttributeValue("href", null); //get link of first review
					string fullreviewLink = GetCompleteLink(freviewLink);
					document = GetDocument(fullreviewLink);

					IEnumerable<HtmlNode> fullReviewNodes = GetFullReviews(document);
					foreach (HtmlNode review in fullReviewNodes)
					{
						var p = review.Descendants("p").Where(d => d.Attributes.Contains("id"));
						DataRow drNew = dtItinerary.NewRow();
						drNew["name"] = topPlace.InnerText;
						drNew["url"] = reviewLink;
						drNew["review"] = p.First().InnerText;
						dtItinerary.Rows.Add(drNew);
					}
				}
			}

			return dsItinerary;
		}

		private IEnumerable<HtmlNode> GetTopPlaces(HtmlDocument doc)
		{
			IEnumerable<HtmlNode> topPlaces = doc.DocumentNode
						.Descendants("div")
						.Where(d =>
						   d.Attributes.Contains("class")
						   &&
						   d.Attributes["class"].Value.Contains("listing_title"));

			return topPlaces;
		}
		private IEnumerable<HtmlNode> GetFullReviews(HtmlDocument doc)
		{
			IEnumerable<HtmlNode> reviews = doc.DocumentNode
				.Descendants("div")
				.Where(d =>
					d.Attributes.Contains("class")
					&& d.Attributes["class"].Value.Contains("innerBubble"));

			return reviews;
		}

		// Used to get reviews link
		private IEnumerable<HtmlNode> GetUserReviews(HtmlDocument doc)
		{
			IEnumerable<HtmlNode> reviews = doc.DocumentNode.Descendants("a").Where(d => d.Attributes.Contains("id") && d.Attributes["href"].Value.Contains("ShowUserReviews"));

			return reviews;
		}

		private string GetThingsToDoURL(string cityUrl)
		{
			string thingsToDoUrl = string.Empty;

			HtmlDocument document = GetDocument(cityUrl);

			IEnumerable<HtmlNode> attractionsNode = document.DocumentNode.Descendants("li").Where(d => d.Attributes.Contains("data-element") && d.Attributes["data-element"].Value.Contains(".masthead-dropdown-attractions"));
			if (attractionsNode.Count() > 0)
			{
				string attractionLink = attractionsNode.First().FirstChild.GetAttributeValue("href", null);
				thingsToDoUrl = GetCompleteLink(attractionLink); ;
			}

			return thingsToDoUrl;
		}
	}
}
