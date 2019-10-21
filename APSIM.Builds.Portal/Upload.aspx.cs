using APSIM.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace APSIM.Builds.Portal
{
    public partial class Upload : System.Web.UI.Page
    {
        private BugTracker[] bugs;

        protected void Page_Load(object sender, EventArgs e)
        {
            string url = "http://apsimdev.apsim.info/APSIM.Builds.Service/BuildsClassic.svc/GetIssueList";
            bugs = WebUtilities.CallRESTService<BugTracker[]>(url);

            foreach (BugTracker bug in bugs)
                BugList.Items.Add(bug.bugID + " - " + bug.description);
        }


        protected void UploadButton_Click(object sender, EventArgs e)
        {
            bool Ok = AuthenticateSVNUser(UserNameTextBox.Text, PasswordTextBox.Text);
            InvalidLabel.Visible = !Ok;
            DescriptionLabel.Visible = DescriptionTextBox.Text == "";
            PatchLabel.Visible = FileUpload.FileName == "";
            if (Ok && DescriptionTextBox.Text != "" && FileUpload.FileName != "")
            {
                // Check the bug id.
                int PosHyphen = BugList.Text.IndexOf(" - ");
                if (PosHyphen == -1)
                    throw new Exception("Bad BugID description: " + BugList.Text);
                int BugID = Convert.ToInt32(BugList.Text.Substring(0, PosHyphen));

                // Write the file to the upload directory.
                string FileNameToWrite = @"D:\Websites\APSIM.Builds.Portal\Files\" + Path.GetFileNameWithoutExtension(FileUpload.FileName).Replace(" ", "") + "(" + DateTime.Now.ToString("dd-MM-yyyy_HH.mm.ss") + ").zip";

                // Write the zip file.
                FileStream ZipFile = new FileStream(FileNameToWrite, FileMode.CreateNew);
                FileUpload.FileContent.CopyTo(ZipFile);
                ZipFile.Close();

                string url = "http://apsimdev.apsim.info/APSIM.Builds.Service/BuildsClassic.svc/Add" +
                             "?UserName=" + UserNameTextBox.Text +
                             "&Password=" + PasswordTextBox.Text +
                             "&PatchFileName=" + Path.GetFileName(FileNameToWrite) +
                             "&Description=" + DescriptionTextBox.Text +
                             "&BugID=" + BugID +
                             "&DoCommit=" + CheckBox.Checked +
                             "&DbConnectPassword=" + Bob.GetValidPassword();
                WebUtilities.CallRESTService<object>(url);

                string authority = Request.Url.GetLeftPart(UriPartial.Authority);

                Response.Redirect(Bob.GetBaseURL(Request.Url) + "/Bob.aspx");
            }

        }

        /// <summary>
        /// Perorms an authorisation check on the given username nad password. Returns true
        /// if user and password is OK.
        /// </summary>
        private bool AuthenticateSVNUser(string UserName, string Password)
        {
            bool Ok = false;
            try
            {
                string uri = "http://apsrunet.apsim.info/svn/apsim/";

                HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(uri);
                request.Method = "MKACTIVITY";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Credentials = new NetworkCredential(UserName, Password); ;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {
                Ok = !ex.Message.Contains("Unauthorized");
            }
            return Ok;
        }

        /// <summary>A bug</summary>
        public class BugTracker
        {
            public int bugID;
            public string description;
        }

    }
}