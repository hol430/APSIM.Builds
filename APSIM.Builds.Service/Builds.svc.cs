
namespace APSIM.Builds.Service
{
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


    /// <summary>
    /// Web service that provides access to the ApsimX builds system.
    /// </summary>
    public class Builds : IBuilds
    {
        /// <summary>Add a build to the build database.</summary>
        /// <param name="pullRequestNumber">The GitHub pull request number.</param>
        /// <param name="issueID">The issue ID.</param>
        /// <param name="issueTitle">The issue title.</param>
        public void AddBuild(int pullRequestNumber, int issueID, string issueTitle, string ChangeDBPassword)
        {
            if (ChangeDBPassword == BuildsClassic.GetValidPassword())
            {
                using (SqlConnection connection = BuildsClassic.Open())
                {
                    string sql = "INSERT INTO ApsimX (Date, PullRequestID, IssueNumber, IssueTitle) " +
                                 "VALUES (@Date, @PullRequestID, @IssueNumber, @IssueTitle)";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@Date", DateTime.Now.ToString("yyyy-MM-dd hh:mm tt")));
                        command.Parameters.Add(new SqlParameter("@PullRequestID", pullRequestNumber));
                        command.Parameters.Add(new SqlParameter("@IssueNumber", issueID));
                        command.Parameters.Add(new SqlParameter("@IssueTitle", issueTitle));
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Gets a list of possible upgrades since the specified issue number.
        /// </summary>
        /// <param name="issueNumber">The issue number.</param>
        /// <returns>The list of possible upgrades.</returns>
        public List<Upgrade> GetUpgradesSinceIssue(int issueNumber)
        {
            List<Upgrade> upgrades = new List<Upgrade>();

            DateTime issueResolvedDate = GetIssueResolvedDate(issueNumber);

            string sql = "SELECT * FROM ApsimX " +
                         "WHERE Date >= " + string.Format("'{0:yyyy-MM-ddThh:mm:ss tt}'", issueResolvedDate) +
                         " ORDER BY Date DESC";

            using (SqlConnection connection = BuildsClassic.Open())
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int pullID = (int)reader["PullRequestID"];
                            DateTime date = (DateTime)reader["Date"];

                            int buildIssueNumber = (int)reader["IssueNumber"];

                            string version = ((DateTime)reader["Date"]).ToString("yyyy.MM.dd") + "." + buildIssueNumber;

                            Upgrade upgrade = new Upgrade();
                            upgrade.ReleaseDate = (DateTime)reader["Date"];
                            upgrade.issueNumber = buildIssueNumber;
                            upgrade.IssueTitle = (string)reader["IssueTitle"];
                            upgrade.IssueURL = @"https://github.com/APSIMInitiative/ApsimX/issues/" + buildIssueNumber;
                            upgrade.ReleaseURL = @"http://bob.apsim.info/ApsimXFiles/" + buildIssueNumber + "/ApsimSetup.exe";

                            upgrades.Add(upgrade);
                        }
                    }
                }
            }
            return upgrades;
        }

        /// <summary>
        /// Gets the URL of the latest version.
        /// </summary>
        /// <returns>The URL of the latest version of APSIM Next Generation.</returns>
        public string GetURLOfLatestVersion()
        {
            string url = null;
            string sql = "SELECT TOP 1 * FROM ApsimX " +
                         " ORDER BY Date DESC";

            using (SqlConnection connection = BuildsClassic.Open())
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int buildIssueNumber = (int)reader["IssueNumber"];
                            url = @"http://bob.apsim.info/ApsimXFiles/" + buildIssueNumber + "/ApsimSetup.exe";
                        }
                    }
                }
            }

            return url;
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
                         "WHERE IssueNumber = " + issueNumber;
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
        /// Add a upgrade registration into the database.
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="organisation"></param>
        /// <param name="address1"></param>
        /// <param name="address2"></param>
        /// <param name="city"></param>
        /// <param name="state"></param>
        /// <param name="postcode"></param>
        /// <param name="country"></param>
        /// <param name="email"></param>
        /// <param name="product"></param>
        public void RegisterUpgrade(string firstName, string lastName, string organisation, string address1, string address2,
                    string city, string state, string postcode, string country, string email, string product, 
                    string ChangeDBPassword)
        {
            if (ChangeDBPassword == BuildsClassic.GetValidPassword())
            {
                string sql = "INSERT INTO Registrations (Date, FirstName, LastName, Organisation, Address1, Address2, City, State, Postcode, Country, Email, Product) " +
                             "VALUES (@Date, @FirstName, @LastName, @Organisation, @Address1, @Address2, @City, @State, @Postcode, @Country, @Email, @Product)";

                using (SqlConnection connection = BuildsClassic.Open("ProductRegistration"))
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@Date", DateTime.Now));
                        command.Parameters.Add(new SqlParameter("@FirstName", firstName));
                        command.Parameters.Add(new SqlParameter("@LastName", lastName));
                        command.Parameters.Add(new SqlParameter("@Organisation", organisation));
                        command.Parameters.Add(new SqlParameter("@Address1", address1));
                        command.Parameters.Add(new SqlParameter("@Address2", address2));
                        command.Parameters.Add(new SqlParameter("@City", city));
                        command.Parameters.Add(new SqlParameter("@State", state));
                        command.Parameters.Add(new SqlParameter("@Postcode", postcode));
                        command.Parameters.Add(new SqlParameter("@Country", country));
                        command.Parameters.Add(new SqlParameter("@Email", email));
                        command.Parameters.Add(new SqlParameter("@Product", product));
                        command.ExecuteNonQuery();

                    }
                }
            }
        }

        /// <summary>
        /// Get a GitHub issue ID from a pull request ID.
        /// </summary>
        /// <param name="pullRequestID"></param>
        /// <returns></returns>
        public int GetIssueID(int pullRequestID)
        {
            int issueID;
            string issueTitle;
            GetIssueDetails(pullRequestID, out issueID, out issueTitle);

            if (issueID == 0)
                throw new Exception("Cannot find issue number in pull request: " + pullRequestID);

            return issueID;
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
            string token = File.ReadAllText(@"D:Websites\GitHubToken.txt");
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
    }
}
