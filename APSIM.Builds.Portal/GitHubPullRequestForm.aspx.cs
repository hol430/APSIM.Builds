namespace APSIM.Builds.Portal
{
    using APSIM.Shared.Utilities;
    using Octokit;
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Script.Serialization;

    public partial class GitHubPullRequestForm : System.Web.UI.Page
    {
        /// <summary>
        /// Page has been loaded - called by GitHub when a pull request is opened, merged or closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            string json = GetJsonFromInputStream();

            var serializer = new JavaScriptSerializer();
            GitHub gitHub = serializer.Deserialize<GitHub>(json);

            if (gitHub.pull_request == null)
                ShowMessage("Cannot find pull request in GitHub JSON.");
            else if (gitHub.pull_request.merged == false)
                ShowMessage("Pull request not merged - ignored");
            else if (gitHub.pull_request.IssueNumber == -1)
                ShowMessage("Pull request doesn't reference an issue.");
            else
            {
                string url = "http://www.apsim.info/APSIM.Builds.Service/Builds.svc/AddBuild" +
                             "?pullRequestNumber=" + gitHub.pull_request.number +
                             "&issueID=" + gitHub.pull_request.IssueNumber +
                             "&issueTitle=" + gitHub.pull_request.IssueTitle +
                             "&released=true" +
                             "&ChangeDBPassword=" + GetValidPassword();

                StreamWriter o = new StreamWriter(@"D:\Websites\test.txt");
                o.Write(url);
                o.Close();
                WebUtilities.CallRESTService<object>(url);

                ShowMessage("Added release for issue " + gitHub.pull_request.IssueNumber);
            }
        }

        /// <summary>Return the valid password for this web service.</summary>
        public static string GetValidPassword()
        {
            string connectionString = File.ReadAllText(@"D:\Websites\ChangeDBPassword.txt");
            int posPassword = connectionString.IndexOf("Password=");
            return connectionString.Substring(posPassword + "Password=".Length);
        }

        /// <summary>
        /// Get the JSON that GitHub has passed to us.
        /// </summary>
        /// <returns>The JSON string.</returns>
        private string GetJsonFromInputStream()
        {
            const int INPUT_BUFFER_LENGTH = 4096;
            byte[] data = new byte[INPUT_BUFFER_LENGTH];
            MemoryStream writer = new MemoryStream();
            Stream input = Request.InputStream;
            int rlen;
            while ((rlen = input.Read(data, 0, INPUT_BUFFER_LENGTH)) > 0)
                writer.Write(data, 0, rlen);

            string json = Encoding.ASCII.GetString(writer.ToArray());
            return json;
        }

        /// <summary>
        /// Write a message in the response stream.
        /// </summary>
        /// <param name="msg"></param>
        private void ShowMessage(string msg)
        {
            //File.WriteAllText(@"C:\inetpub\wwwroot\github.txt", msg);
            Response.Clear();
            Response.ContentType = "text/plain";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Write(msg);
            Response.End();
        }
    }

    /// <summary>
    /// A class for deserialising the top level JSON from GitHub
    /// </summary>
    public class GitHub
    {
        public PullRequest pull_request;

    }

    /// <summary>
    /// A class for deserialising the pull request JSON object.
    /// </summary>
    public class PullRequest
    {
        public int number;
        public bool merged;
        public string body;
        public string issue_url;

        /// <summary>
        /// Returns a resolved issue number or -1 if not found.
        /// </summary>
        public int IssueNumber
        {
            get
            {
                int posResolves = body.IndexOf("Resolves", StringComparison.InvariantCultureIgnoreCase);
                if (posResolves == -1)
                    posResolves = body.IndexOf("Working on", StringComparison.InvariantCultureIgnoreCase);

                if (posResolves != -1)
                {
                    int posHash = body.IndexOf("#", posResolves);
                    if (posHash != -1)
                    {
                        int issueID = 0;

                        int posSpace = body.IndexOfAny(new char[] { ' ', '\r', '\n',
                                                                           '\t', '.', ';',
                                                                           ':', '+', '&' }, posHash);
                        if (posSpace == -1)
                            posSpace = body.Length;
                        if (posSpace != -1)
                            if (Int32.TryParse(body.Substring(posHash + 1, posSpace - posHash - 1), out issueID))
                                return issueID;
                    }
                }

                return -1;
            }
        }

        /// <summary>
        /// Returns true if pull request resolves an issue
        /// </summary>
        public bool ResolvesIssue
        {
            get
            {
                string stringToMatch = "Resolves #" + IssueNumber;
                return body.IndexOf(stringToMatch, StringComparison.CurrentCultureIgnoreCase) != -1;
            }
        }

        /// <summary>
        /// Returns the issue title.
        /// </summary>
        public string IssueTitle
        {
            get
            {
                if (IssueNumber != -1)
                {
                    GitHubClient github = new GitHubClient(new ProductHeaderValue("ApsimX"));
                    string token = File.ReadAllText(@"D:\Websites\GitHubToken.txt");
                    github.Credentials = new Credentials(token);
                    Task<Issue> issueTask = github.Issue.Get("APSIMInitiative", "ApsimX", IssueNumber);
                    issueTask.Wait();
                    return issueTask.Result.Title;
                }

                return null;
            }
        }
    }
}