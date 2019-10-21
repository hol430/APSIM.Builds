namespace APSIM.Builds.Portal
{
    using APSIM.Shared.Utilities;
    using System;

    public partial class DeleteJob : System.Web.UI.Page
    {
        private int JobID;

        protected void Page_Load(object sender, EventArgs e)
        {
            JobID = Convert.ToInt32(Request.QueryString["id"]);
        }

        protected void Yes_Click(object sender, EventArgs e)
        {
            string url = "http://apsimdev.apsim.info/APSIM.Builds.Service/BuildsClassic.svc/DeleteJob" +
             "?JobID=" + JobID +
             "&DbConnectPassword=" + Bob.GetValidPassword();
            WebUtilities.CallRESTService<object>(url);
            Response.Redirect(Bob.GetBaseURL(Request.Url) + "/Bob.aspx");
        }

        protected void No_Click(object sender, EventArgs e)
        {
            Response.Redirect(Bob.GetBaseURL(Request.Url) + "/Bob.aspx");
        }
    }
}