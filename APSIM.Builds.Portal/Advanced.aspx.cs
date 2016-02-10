namespace APSIM.Builds.Portal
{
    using APSIM.Shared.Utilities;
    using System;

    public partial class Advanced : System.Web.UI.Page
    {
        protected void Button1_Click(object sender, EventArgs e)
        {
            string url = "http://www.apsim.info/APSIM.Builds.Service/BuildsClassic.svc/UpdateStatus" +
                         "?JobID=" + JobID.Text +
                         "&NewStatus=" + NewStatus.Text +
                         "&DbConnectPassword=" + Bob.GetValidPassword();
            WebUtilities.CallRESTService<object>(url);
        }
    }
}