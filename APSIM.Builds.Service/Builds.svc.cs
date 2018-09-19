
namespace APSIM.Builds.Service
{
    using Shared.Utilities;
    using Octokit;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Text;
    using System.Threading.Tasks;
    using System.Globalization;


    /// <summary>
    /// Web service that provides access to the ApsimX builds system.
    /// </summary>
    public class Builds : IBuilds
    {
        /// <summary>Add a build to the build database.</summary>
        /// <param name="pullRequestNumber">The GitHub pull request number.</param>
        /// <param name="changeDBPassword">The password</param>
        public void AddBuild(int pullRequestNumber, string changeDBPassword)
        {
            if (changeDBPassword == BuildsClassic.GetValidPassword())
            {
                using (SqlConnection connection = BuildsClassic.Open())
                {
                    string sql = "INSERT INTO ApsimX (Date, PullRequestID, IssueNumber, IssueTitle, Released) " +
                                 "VALUES (@Date, @PullRequestID, @IssueNumber, @IssueTitle, @Released)";

                    DateTime date = DateTime.Now;
                    int issueID;
                    string issueTitle;
                    GetIssueDetails(pullRequestNumber, out issueID, out issueTitle);
                    bool released = PullResolvesIssue(pullRequestNumber, issueID);
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@Date", date));
                        command.Parameters.Add(new SqlParameter("@PullRequestID", pullRequestNumber));
                        command.Parameters.Add(new SqlParameter("@IssueNumber", issueID));
                        command.Parameters.Add(new SqlParameter("@IssueTitle", issueTitle));
                        command.Parameters.Add(new SqlParameter("@Released", released));
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>Add a green build to the build database.</summary>
        /// <param name="pullRequestNumber">The GitHub pull request number.</param>
        /// <param name="buildTimeStamp">The build time stamp</param>
        /// <param name="changeDBPassword">The password</param>
        public void AddGreenBuild(int pullRequestNumber, string buildTimeStamp, string changeDBPassword)
        {
            if (changeDBPassword == BuildsClassic.GetValidPassword())
            {
                using (SqlConnection connection = BuildsClassic.Open())
                {
                    string sql = "INSERT INTO ApsimX (Date, PullRequestID, IssueNumber, IssueTitle, Released) " +
                                 "VALUES (@Date, @PullRequestID, @IssueNumber, @IssueTitle, @Released)";

                    DateTime date = DateTime.ParseExact(buildTimeStamp, "yyyy.MM.dd-HH:mm", null);
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@Date", date));
                        command.Parameters.Add(new SqlParameter("@PullRequestID", pullRequestNumber));
                        command.Parameters.Add(new SqlParameter("@IssueNumber", string.Empty));
                        command.Parameters.Add(new SqlParameter("@IssueTitle", string.Empty));
                        command.Parameters.Add(new SqlParameter("@Released", false));
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Gets a list of possible upgrades since the specified Apsim version.
        /// </summary>
        /// <param name="version">Fully qualified (a.b.c.d) version number.</param>
        /// <returns>List of possible upgrades.</returns>
        public List<Upgrade> GetUpgradesSinceVersion(string version)
        {
            int issueNumber = 0;

            int lastDotPosition = version.LastIndexOf(".");
            int.TryParse(version.Substring(lastDotPosition + 1), out issueNumber);

            string dateFromVersion = version.Substring(0, lastDotPosition);
            string[] formats = new string[]
            {
                "yyyy.mm.dd",
                "yyyy.m.dd",
                "yyyy.mm.d",
                "yyyy.m.d"
            };
            DateTime issueResolvedDate;
            if (!DateTime.TryParseExact(dateFromVersion, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out issueResolvedDate))
                throw new Exception(string.Format("Date is not in a valid format: {0}.", dateFromVersion));
            return GetUpgradesSinceDate(issueResolvedDate).Where(u => u.issueNumber != issueNumber).ToList();
        }

        /// <summary>
        /// Gets a list of possible upgrades since the specified issue number.
        /// </summary>
        /// <param name="issueNumber">The issue number.</param>
        /// <returns>The list of possible upgrades.</returns>
        public List<Upgrade> GetUpgradesSinceIssue(int issueNumber)
        {
            return GetUpgradesSinceDate(GetIssueResolvedDate(issueNumber));
        }

        /// <summary>
        /// Gets all list of released upgrades since a given date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>List of possible upgrades.</returns>
        private List<Upgrade> GetUpgradesSinceDate(DateTime date)
        {
            List<Upgrade> upgrades = new List<Upgrade>();

            string sql = "SELECT * FROM ApsimX " +
                         "WHERE Date >= " + string.Format("'{0:yyyy-MM-ddThh:mm:ss tt}'", date) +
                         " ORDER BY Date DESC";

            using (SqlConnection connection = BuildsClassic.Open())
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int buildIssueNumber = (int)reader["IssueNumber"];
                            int released = (int)reader["Released"];
                            if (buildIssueNumber > 0 && released == 1)
                            {
                                if (upgrades.Find(u => u.issueNumber == buildIssueNumber) == null)
                                {
                                    Upgrade upgrade = new Upgrade();
                                    upgrade.ReleaseDate = (DateTime)reader["Date"];
                                    upgrade.issueNumber = buildIssueNumber;
                                    upgrade.IssueTitle = (string)reader["IssueTitle"];
                                    upgrade.IssueURL = @"https://github.com/APSIMInitiative/ApsimX/issues/" + buildIssueNumber;
                                    upgrade.ReleaseURL = @"http://www.apsim.info/ApsimXFiles/ApsimSetup" + buildIssueNumber + ".exe";
                                    upgrades.Add(upgrade);
                                }
                            }
                        }
                    }
                }
            }
            return upgrades;
        }

        /// <summary>
        /// Gets a URL for a version that resolves the specified issue
        /// </summary>
        /// <param name="issueNumber">The issue number.</param>
        public string GetURLOfVersionForIssue(int issueNumber)
        {
            List<Upgrade> upgrades = new List<Upgrade>();

            DateTime issueResolvedDate = GetIssueResolvedDate(issueNumber);

            string sql = "SELECT * FROM ApsimX " +
                         "WHERE IssueNumber = " + issueNumber;

            using (SqlConnection connection = BuildsClassic.Open())
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            return @"http://www.apsim.info/ApsimXFiles/ApsimSetup" + issueNumber + ".exe";
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the URL of the latest version.
        /// </summary>
        /// <param name="operatingSystem">Operating system to get url for.</param>
        /// <returns>The URL of the latest version of APSIM Next Generation.</returns>
        public string GetURLOfLatestVersion(string operatingSystem)
        {
            Build latestBuild = GetLatestBuild();
            if (operatingSystem == "Debian")
                return Path.ChangeExtension(latestBuild.url, ".deb");
            else if (operatingSystem == "Mac")
                return Path.ChangeExtension(latestBuild.url, ".dmg");
            else
                return latestBuild.url;
        }

        /// <summary>
        /// Return the date the specified issue was resolved.
        /// </summary>
        /// <param name="issueNumber">The issue number</param>
        /// <returns>The date.</returns>
        private DateTime GetIssueResolvedDate(int issueNumber)
        {
            DateTime resolvedDate = new DateTime(2015, 1, 1);

            string sql = "SELECT * FROM ApsimX " +
                         "WHERE IssueNumber = " + issueNumber +
                         "ORDER BY Date DESC";
            using (SqlConnection connection = BuildsClassic.Open())
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            resolvedDate = (DateTime)reader["Date"];
                    }
                }
            }

            return resolvedDate;
        }

        /// <summary>
        /// Get details about a GitHub pull request ID. Called from Jenkins.
        /// </summary>
        /// <param name="pullRequestID"></param>
        /// <returns>Format of return string is yyyy-MM-dd hh:mm tt,ID</returns>
        public string GetPullRequestDetails(int pullRequestID)
        {
            int issueID;
            string issueTitle;
            GetIssueDetails(pullRequestID, out issueID, out issueTitle);

            if (issueID == 0)
                throw new Exception("Cannot find issue number in pull request: " + pullRequestID);

            return DateTime.Now.ToString("yyyy.MM.dd-HH:mm") + "," + issueID;
        }
     
        /// <summary>
        /// Try and get a issue number and title for the specified pull request id. If not 
        /// a valid release then will return issueID = 0;
        /// </summary>
        /// <remarks>
        /// A valid release is one that is merged and has 'Resolves #xxx" in the body
        /// of the pull request.
        /// </remarks>
        /// <param name="pullID"></param>
        /// <param name="issueID">The issue ID found.</param>
        /// <param name="issueTitle">The issue title found.</param>
        private static void GetIssueDetails(int pullID, out int issueID, out string issueTitle)
        {
            issueID = 0;
            issueTitle = null;

            GitHubClient github = new GitHubClient(new ProductHeaderValue("ApsimX"));
            string token = File.ReadAllText(@"D:\Websites\GitHubToken.txt");
            github.Credentials = new Credentials(token);
            Task<PullRequest> pullRequestTask = github.PullRequest.Get("APSIMInitiative", "ApsimX", pullID);
            pullRequestTask.Wait();
            PullRequest pullRequest = pullRequestTask.Result;
            issueID = GetIssueID(pullRequest.Body);
            if (issueID != -1)
            {
                Task<Issue> issueTask = github.Issue.Get("APSIMInitiative", "ApsimX", issueID);
                issueTask.Wait();
                issueTitle = issueTask.Result.Title;
            }
        }

        private static bool PullResolvesIssue(int pullID, int issueID)
        {
            GitHubClient github = new GitHubClient(new ProductHeaderValue("ApsimX"));
            string token = File.ReadAllText(@"D:\Websites\GitHubToken.txt");
            github.Credentials = new Credentials(token);
            Task<PullRequest> pullRequestTask = github.PullRequest.Get("APSIMInitiative", "ApsimX", pullID);
            pullRequestTask.Wait();
            PullRequest pullRequest = pullRequestTask.Result;
            return pullRequest.Body.IndexOf(string.Format("Resolves #{0}", issueID), StringComparison.OrdinalIgnoreCase) > 0;
        }

        /// <summary>
        /// Returns a resolved issue id or -1 if not found.
        /// </summary>
        /// <param name="pullRequestBody">The text of the pull request body.</param>
        /// <returns>The issue ID or -1 if not found.</returns>
        private static int GetIssueID(string pullRequestBody)
        {
            int posResolves = pullRequestBody.IndexOf("Resolves", StringComparison.InvariantCultureIgnoreCase);
            if (posResolves == -1)
                posResolves = pullRequestBody.IndexOf("Working on", StringComparison.InvariantCultureIgnoreCase);

            if (posResolves != -1)
            {
                int posHash = pullRequestBody.IndexOf("#", posResolves);
                if (posHash != -1)
                {
                    int issueID = 0;

                    int posSpace = pullRequestBody.IndexOfAny(new char[] { ' ', '\r', '\n',
                                                                           '\t', '.', ';',
                                                                           ':', '+', '&' }, posHash);
                    if (posSpace == -1)
                        posSpace = pullRequestBody.Length;
                    if (posSpace != -1)
                        if (Int32.TryParse(pullRequestBody.Substring(posHash + 1, posSpace - posHash - 1), out issueID))
                            return issueID;
                }
            }
            return -1;
        }

        /// <summary>Get the latest build.</summary>
        private static Build GetLatestBuild()
        {
            string sql = "SELECT TOP 1 * FROM ApsimX " +
                         " WHERE Released=1" +
                         " ORDER BY Date DESC";

            using (SqlConnection connection = BuildsClassic.Open())
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Build build = new Build();
                            build.date = (DateTime)reader["Date"];
                            build.pullRequestID = (int)reader["PullRequestID"]; ;
                            build.issueNumber = (int)reader["IssueNumber"];
                            build.issueTitle = (string)reader["IssueTitle"];
                            build.url = @"http://www.apsim.info/ApsimXFiles/ApsimSetup" + build.issueNumber + ".exe";
                            return build;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>Get latest documentation HTML.</summary>
        public Stream GetDocumentationHTML()
        {
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html; charset=utf-8";
            string html = "<html><head>" + Environment.NewLine;
            html += "<link rel=\"shortcut icon\" href=\"https://apsimnextgeneration.netlify.com/images/favicon.png\" type=\"image/x-icon\" />" +
                    "<link href=\"https://apsimnextgeneration.netlify.com/css/nucleus.css\" rel=\"stylesheet\" >" +
                    "<link href=\"https://apsimnextgeneration.netlify.com/css/font-awesome.min.css\" rel=\"stylesheet\">" +
                    "<link href=\"https://apsimnextgeneration.netlify.com/css/hybrid.css\" rel=\"stylesheet\">" +
                    "<link href=\"https://apsimnextgeneration.netlify.com/css/featherlight.min.css\" rel=\"stylesheet\">" +
                    "<link href=\"https://apsimnextgeneration.netlify.com/css/auto-complete.css\" rel=\"stylesheet\" >" +
                    "<link href=\"https://apsimnextgeneration.netlify.com/theme-original/style.css\" rel=\"stylesheet\">" +
                    "<link href=\"https://apsimnextgeneration.netlify.com/theme-original/variant-darkgreen.css\" rel=\"stylesheet\">" +
                    "<link rel=\"stylesheet\" href=\"https://apsimnextgeneration.netlify.com/css/bootstrap.min.css\" >";

            html += "</head><body>";
            Build latestBuild = GetLatestBuild();

            html += "<h2>Documentation for build <a href=\"https://github.com/APSIMInitiative/ApsimX/issues/" + latestBuild.issueNumber + "\">" +
                     latestBuild.issueNumber + "</a> " + latestBuild.date.ToLongDateString() + "</h2>";

            string pattern = "*" + latestBuild.issueNumber + ".pdf";
            foreach (string file in Directory.GetFiles(@"D:\WebSites\APSIM\ApsimxFiles", pattern))
            {
                string docURL = file.Replace(@"D:\WebSites\APSIM", "http://www.apsim.info");
                docURL = docURL.Replace('\\', '/');

                string modelName = Path.GetFileNameWithoutExtension(file);
                modelName = modelName.Replace(latestBuild.issueNumber.ToString(), "");
                html += "<a href=\"" + docURL + "\" target=\"_blank\">" + modelName + "</a><br/>" + Environment.NewLine;
            }

            // Add in extra docs.
            html += "<a href=\"https://www.apsim.info/Report.aspx\" target=\"_blank\">Report</a><br/>" + Environment.NewLine;
            html += "<a href=\"https://www.apsim.info/Documentation/APSIM(nextgeneration)/Memo.aspx\" target=\"_blank\"> Memo</a><br/>" + Environment.NewLine;

            html += "</body></html>";

            return new MemoryStream(Encoding.UTF8.GetBytes(html));
        }


        private class Build
        {
            public DateTime date;
            public int pullRequestID;
            public int issueNumber;
            public string issueTitle;
            public string url;
        }
    }
}
