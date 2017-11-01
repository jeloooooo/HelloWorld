using HelloWorld.Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HelloWorld.Explore
{
	public partial class Explore : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (IsPostBack)
			{
			}
		}

		public void ResetDataBinding(GridView gv)
		{
			gv.DataSource = null;
			//rebind to gridview
			gv.DataBind();
		}

		public async Task LoadAttractionsData(string url)
		{
			Itinerary objExp = new Itinerary();

			TripAdvisorScraper tas = new TripAdvisorScraper(url);
			DataSet ds = await tas.GetTopAttractions();

			gvExplore.DataSource = ds;
			gvExplore.DataBind();
		}

		public async Task LoadReviewsData(string attraction, string reviewsUrl)
		{
			Itinerary objExp = new Itinerary();

			TripAdvisorScraper tas = new TripAdvisorScraper();
			DataSet ds = await tas.GetTopAttractionsReviews(attraction, reviewsUrl);

			gvReviews.DataSource = ds;
			gvReviews.DataBind();

			DataSet dsPhoto = await tas.GetTopAttractionsReviewsPhotos();

			rpPhotos.DataSource = dsPhoto;
			rpPhotos.DataBind();
		}

		protected void btnExplore_Click(object sender, EventArgs e)
		{
			string url = "https://www.tripadvisor.com/Tourism-g1758900-Taguig_City_Metro_Manila_Luzon-Vacations.html";
			RegisterAsyncTask(new PageAsyncTask(() => LoadAttractionsData(url)));
		}

		protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(gvExplore, "Select$" + e.Row.RowIndex);
				e.Row.Attributes["style"] = "cursor:pointer";
			}
		}

		protected void OnSelectedIndexChanged(object sender, EventArgs e)
		{
			//int index = gvExplore.SelectedRow.RowIndex;
			string name = gvExplore.SelectedRow.Cells[0].Text;
			string reviewUrl = gvExplore.SelectedRow.Cells[1].Text;
			//string message = "Row Index: " + index + " Name: " + name + " Url: " + country;
			//txtExplore.Text = message;
			RegisterAsyncTask(new PageAsyncTask(() => LoadReviewsData(name, reviewUrl)));
		}

		protected void btnExploreCustom_Click(object sender, EventArgs e)
		{
			if (Utility.IsValidUrl(txtExplore.Text))
			{
				ResetDataBinding(gvReviews);
				string url = txtExplore.Text;
				RegisterAsyncTask(new PageAsyncTask(() => LoadAttractionsData(url)));
			}
			else
				lblInfo.Text = "Invalid TripAdvisor url.";

		}
	}
}
