
namespace APSIM.Builds.Portal
{
    using Shared.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Xml.Serialization;

    public partial class Bob : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string baseURL = GetBaseURL(Request.Url);
            string files = baseURL + "/files/";

            GridView.DataSource = null;
            string url = "http://www.apsim.info/APSIM.Builds.Service/BuildsClassic.svc/GetJobs?NumRows=" + NumRowsTextBox.Text + "&PassOnly=" + Passes.Checked;
            BuildJob[] buildJobs = WebUtilities.CallRESTService<BuildJob[]>(url);

            DataTable data = new DataTable();
            data.Columns.Add("User", typeof(string));
            data.Columns.Add("PatchFile", typeof(string));
            data.Columns.Add("Description", typeof(string));
            data.Columns.Add("Task", typeof(string));
            data.Columns.Add("Status", typeof(string));
            data.Columns.Add("StartTime", typeof(string));
            data.Columns.Add("Duration", typeof(string));
            data.Columns.Add("Revision", typeof(string));
            data.Columns.Add("Links", typeof(string));

            foreach (BuildJob buildJob in buildJobs)
            {
                DataRow row = data.NewRow();
                row["User"] = buildJob.UserName;

                row["PatchFile"] = HTMLLink(buildJob.PatchFileNameShort, buildJob.PatchFileURL);

                row["Description"] = buildJob.Description;

                if (buildJob.BuiltOnJenkins)
                    row["Task"] = HTMLLink("#" + buildJob.TaskID, "https://github.com/APSIMInitiative/APSIMClassic/issues/" + buildJob.TaskID);
                else
                    row["Task"] = HTMLLink("T" + buildJob.TaskID, "http://www.apsim.info/BugTracker/edit_bug.aspx?id=" + buildJob.TaskID);

                string statusText = "Win32:" + buildJob.WindowsStatus;
                if (buildJob.WindowsNumDiffs > 0)
                    statusText += " (" + buildJob.WindowsNumDiffs + ")";
                row["Status"] = HTMLLink(statusText, buildJob.WindowsDetailsURL) + " " +
                                HTMLLink("(xml)", buildJob.XmlUrl);

                row["StartTime"] = ((DateTime)buildJob.StartTime).ToString("dd MMM yyyy hh:mm tt");

                if (buildJob.Revision > 0)
                    row["Revision"] = HTMLLink("R" + buildJob.Revision, "http://apsrunet.apsim.info/websvn/revision.php?repname=apsim&path=%2Ftrunk%2F&rev=" + buildJob.Revision);
                else if (buildJob.BuiltOnJenkins)
                    row["Revision"] = HTMLLink($"#{buildJob.PatchFileName}", $"https://github.com/APSIMInitiative/APSIMClassic/pull/{buildJob.PatchFileName}.diff");

                if (statusText.Contains("Win32:Pass") || statusText.Contains("Win32:Fail"))
                {
                    row["Duration"] = buildJob.Duration + "min";
                    row["Links"] = HTMLLink("Win32 Diffs", buildJob.WindowsDiffsURL) + " " +
                                   HTMLLink("Binaries", buildJob.WindowsBinariesURL) + " " +
                                   HTMLLink("BuildTree", buildJob.WindowsBuildTreeURL) + " " +
                                   HTMLLink("WindowsInstaller", buildJob.WindowsInstallerURL) + " " +
                                   HTMLLink("WindowsInstallerFull", buildJob.WindowsInstallerFullURL) + " " +
                                   HTMLLink("Win32 SFX", buildJob.Win32SFXURL) + " " +
                                   HTMLLink("Win64 SFX", buildJob.Win64SFXURL);
                }
                data.Rows.Add(row);
            }

            GridView.DataSource = data;
            GridView.DataBind();

            // Colour pass / fail cells.
            foreach (GridViewRow Row in GridView.Rows)
            {
                if (Row.Cells[4].Text.Contains("Win32:Pass"))
                    Row.Cells[4].BackColor = Color.PaleGreen;
                else if (Row.Cells[4].Text.Contains("Win32:Fail"))
                    Row.Cells[4].BackColor = Color.LightSalmon;
            }

            PopulateChart();
        }

        public static string GetBaseURL(Uri uri)
        {
            string url = uri.ToString();
            int posLastSlash = url.LastIndexOf('/');
            if (posLastSlash == -1)
                return url;
            else
                return url.Substring(0, posLastSlash);

        }


        /// <summary>Return the valid password for this web service.</summary>
        public static string GetValidPassword()
        {
            string connectionString = File.ReadAllText(@"D:\WebSites\ChangeDBPassword.txt");
            int posPassword = connectionString.IndexOf("Password=");
            return connectionString.Substring(posPassword + "Password=".Length);
        }

        private string GetShortPatchFile(string patchFileURL)
        {
            int posLastSlash = patchFileURL.LastIndexOf('/');
            int posOpenBracket = patchFileURL.IndexOf('(');
            if (posLastSlash != -1 && posOpenBracket != -1)
                return patchFileURL.Substring(posLastSlash + 1, posOpenBracket - posLastSlash - 1);
            else
                return patchFileURL;
        }

        private string HTMLLink(string description, string url)
        {
            if (url == null || url == string.Empty)
                return "";
            else
                return "<a href=" + url + ">" + description + "</a>";
        }

        protected void UploadButton_Click(object sender, EventArgs e)
        {

            Response.Redirect("Upload.aspx");
        }

        protected void NumRowsTextBox_TextChanged(object sender, EventArgs e)
        {
            Page_Load(null, null);
        }

        protected void Passes_CheckedChanged(object sender, EventArgs e)
        {
            Page_Load(null, null);
        }

        class Commiter
        {
            public string Author;
            public int NumPasses;
            public int NumFailures;
            public int NumPatches;
            public double PercentagePass;
        }
        private void PopulateChart()
        {


            List<Commiter> Commiters = new List<Commiter>();
            foreach (GridViewRow Row in GridView.Rows)
            {
                string Author = Row.Cells[0].Text;
                Commiter Commiter = Commiters.Find(delegate (Commiter C) { return C.Author == Author; });
                if (Commiter == null)
                {
                    Commiter = new Commiter() { Author = Author };
                    Commiters.Add(Commiter);
                }
                if (Row.Cells[4].Text.Contains("Pass"))
                    Commiter.NumPasses++;
                else
                    Commiter.NumFailures++;
                Commiter.NumPatches++;
            }
            foreach (Commiter Commiter in Commiters)
            {
                int TotalCommits = Commiter.NumPasses + Commiter.NumFailures;
                if (TotalCommits > 0)
                    Commiter.PercentagePass = Convert.ToDouble(Commiter.NumPasses) / TotalCommits * 100;
            }

            DataTable GoodCommiterData = new DataTable();
            GoodCommiterData.Columns.Add("Author", typeof(string));
            GoodCommiterData.Columns.Add("NumPasses", typeof(int));
            GoodCommiterData.Columns.Add("NumFailures", typeof(int));
            GoodCommiterData.Columns.Add("PercentagePass", typeof(int));
            FillDataTable(Commiters.OrderByDescending(Commiter => Commiter.NumPatches),
                          GoodCommiterData);

            DataTable BadCommiterData = new DataTable();
            BadCommiterData.Columns.Add("Author", typeof(string));
            BadCommiterData.Columns.Add("NumPasses", typeof(int));
            BadCommiterData.Columns.Add("NumFailures", typeof(int));
            BadCommiterData.Columns.Add("PercentagePass", typeof(int));
            FillDataTable(Commiters.OrderByDescending(Commiter => Commiter.PercentagePass),
                          BadCommiterData,
                          AddNumPatchesToAuthor: true, MinNumPatches: 10);

            Chart1.DataSource = GoodCommiterData;
            Chart1.Series[0].XValueMember = "Author";
            Chart1.Series[0].YValueMembers = "NumFailures";
            Chart1.Series[1].XValueMember = "Author";
            Chart1.Series[1].YValueMembers = "NumPasses";

            Chart2.DataSource = BadCommiterData;
            Chart2.Series[0].XValueMember = "Author";
            Chart2.Series[0].YValueMembers = "PercentagePass";
        }

        private static void FillDataTable(IEnumerable<Commiter> Commiters, DataTable CommiterData, bool AddNumPatchesToAuthor = false, int MinNumPatches = 0)
        {
            int i = 0;
            foreach (Commiter Commiter in Commiters)
            {
                if (i < 8)
                {
                    int NumPatches = Commiter.NumPasses + Commiter.NumFailures;
                    if (NumPatches >= MinNumPatches)
                    {
                        DataRow NewRow = CommiterData.NewRow();
                        if (AddNumPatchesToAuthor)
                            NewRow["Author"] = Commiter.Author + "(" + NumPatches.ToString() + ")";
                        else
                            NewRow["Author"] = Commiter.Author;
                        NewRow["NumPasses"] = Commiter.NumPasses;
                        NewRow["NumFailures"] = Commiter.NumFailures;
                        NewRow["PercentagePass"] = Commiter.PercentagePass;
                        CommiterData.Rows.Add(NewRow);
                    }
                }
                i++;
            }
        }
    }

    /// <summary>A class for holding info about an APSIM classic build.</summary>
    public class BuildJob
    {
        public int ID;
        public string UserName;
        public string PatchFileName;
        public string PatchFileNameShort;
        public string PatchFileURL;
        public string Description;
        public int TaskID;
        public DateTime StartTime;
        public int Duration;
        public int Revision;
        public string XmlUrl;

        public string WindowsStatus;
        public int WindowsNumDiffs;
        public string WindowsBinariesURL;
        public string WindowsBuildTreeURL;
        public string WindowsDiffsURL;
        public string WindowsDetailsURL;
        public string WindowsInstallerURL;
        public string WindowsInstallerFullURL;
        public string Win32SFXURL;
        public string Win64SFXURL;

        public string LinuxStatus;
        public int LinuxNumDiffs;
        public string LinuxBinariesURL;
        public string LinuxDiffsURL;
        public string LinuxDetailsURL;

        public bool BuiltOnJenkins;
        public int JenkinsID;
    }

}