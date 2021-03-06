﻿
namespace APSIM.Builds.Service
{
    using APSIM.Shared.Web;
    using Octokit;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.Text;

    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, 
                     Namespace = "http://apsimdev.apsim.info/services")]
    public class BuildsClassic : IBuildsClassic
    {
        /// <summary>
        /// Owner of the APSIM repository on GitHub.
        /// </summary>
        private const string repoOwner = "APSIMInitiative";

        /// <summary>
        /// Name of the APSIM repository on GitHub.
        /// </summary>
        private const string repoName = "APSIMClassic";

        /// <summary>Add a new entry to the builds database.</summary>
        public int Add(string UserName, string Password, string PatchFileName, string Description, int BugID, bool DoCommit, int JenkinsID, int PullID, string DbConnectPassword)
        {
            if (DbConnectPassword == GetValidPassword())
            {
                string SQL = "INSERT INTO Classic (UserName, Password, PatchFileName, Description, BugID, DoCommit, Status, StartTime, linuxStatus, JenkinsID, PullRequestID) " +
                             "Output Inserted.ID " +
                             "VALUES (@UserName, @Password, @PatchFileName, @Description, @BugID, @DoCommit, @Status, @StartTime, @LinuxStatus, @JenkinsID, @PullID)";

                string NowString = DateTime.Now.ToString("yyyy-MM-dd hh:mm tt");

                using (SqlConnection connection = Open())
                {
                    using (SqlCommand command = new SqlCommand(SQL, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@UserName", UserName));
                        command.Parameters.Add(new SqlParameter("@Password", Password));
                        command.Parameters.Add(new SqlParameter("@PatchFileName", PatchFileName));
                        command.Parameters.Add(new SqlParameter("@Description", Description));
                        command.Parameters.Add(new SqlParameter("@BugID", BugID));
                        command.Parameters.Add(new SqlParameter("@Status", "Running"));
                        command.Parameters.Add(new SqlParameter("@LinuxStatus", "Queued"));
                        command.Parameters.Add(new SqlParameter("@StartTime", NowString));
                        command.Parameters.Add(new SqlParameter("@JenkinsID", JenkinsID));
                        command.Parameters.Add(new SqlParameter("@PullID", PullID));
                        if (DoCommit)
                            command.Parameters.Add(new SqlParameter("@DoCommit", "1"));
                        else
                            command.Parameters.Add(new SqlParameter("@DoCommit", "0"));
                        return (int)command.ExecuteScalar();
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Add a new entry to the builds database.
        /// </summary>
        public int AddPullRequest(int PullID, int JenkinsID, string Password, string DbConnectPassword)
        {
            PullRequest pull = GitHubUtilities.GetPullRequest(PullID, repoOwner, repoName);

            string author = pull.User.Login;
            string patchFileName = PullID.ToString(); // Use Pull Request ID as patch file names.
            string description = pull.Title;
            int issueId = pull.GetIssueID();
            bool doCommit = false; // Legacy option.

            return Add(author, Password, patchFileName, description, issueId, doCommit, JenkinsID, PullID, DbConnectPassword);
        }

        /// <summary>Return details about a specific job.</summary>
        public string GetStatus(int JobID)
        {
            string SQL = "SELECT * FROM Classic WHERE ID = @JobID";

            using (SqlConnection connection = Open())
            {
                using (SqlCommand command = new SqlCommand(SQL, connection))
                {
                    command.Parameters.AddWithValue("@JobID", JobID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            return reader["Status"].ToString();
                        else
                            return null;
                    }
                }
            }
        }

        /// <summary>Return details about a specific job.</summary>
        public string GetPatchFileName(int JobID)
        {
            string SQL = "SELECT * FROM Classic WHERE ID = @JobID";

            using (SqlConnection connection = Open())
            {
                using (SqlCommand command = new SqlCommand(SQL, connection))
                {
                    command.Parameters.AddWithValue("@JobID", JobID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            return "http://apsimdev.apsim.info/APSIM.Builds.Portal/Files/" + reader["PatchFileName"].ToString();
                        else
                            return null;
                    }
                }
            }
        }

        /// <summary>Return details about a specific job.</summary>
        public string GetRevisionNumber(int JobID)
        {
            string SQL = "SELECT * FROM Classic WHERE ID = @JobID";

            using (SqlConnection connection = Open())
            {
                using (SqlCommand command = new SqlCommand(SQL, connection))
                {
                    command.Parameters.AddWithValue("@JobID", JobID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            return reader["RevisionNumber"].ToString();
                        else
                            return null;
                    }
                }
            }
        }

        /// <summary>Return details about a specific job.</summary>
        public string GetUserName(int JobID)
        {
            string SQL = "SELECT * FROM Classic WHERE ID = @JobID";

            using (SqlConnection connection = Open())
            {
                using (SqlCommand command = new SqlCommand(SQL, connection))
                {
                    command.Parameters.AddWithValue("@JobID", JobID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            return reader["UserName"].ToString();
                        else
                            return null;
                    }
                }
            }
        }

        /// <summary>Return details about a specific job.</summary>
        public string GetPassword(int JobID, string DbConnectPassword)
        {
            if (DbConnectPassword == GetValidPassword())
            {
                string SQL = "SELECT * FROM Classic WHERE ID = @JobID";

                using (SqlConnection connection = Open())
                {
                    using (SqlCommand command = new SqlCommand(SQL, connection))
                    {
                        command.Parameters.AddWithValue("@JobID", JobID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                return reader["Password"].ToString();
                            else
                                return null;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>Return details about a specific job.</summary>
        public string GetDescription(int JobID)
        {
            string SQL = "SELECT * FROM Classic WHERE ID = @JobID";

            using (SqlConnection connection = Open())
            {
                using (SqlCommand command = new SqlCommand(SQL, connection))
                {
                    command.Parameters.AddWithValue("@JobID", JobID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            return reader["Description"].ToString();
                        else
                            return null;
                    }
                }
            }
        }

        /// <summary>Return details about a specific job.</summary>
        public string GetBugID(int JobID)
        {
            string SQL = "SELECT * FROM Classic WHERE ID = @JobID";

            using (SqlConnection connection = Open())
            {
                using (SqlCommand command = new SqlCommand(SQL, connection))
                {
                    command.Parameters.AddWithValue("@JobID", JobID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            return reader["BugID"].ToString();
                        else
                            return null;
                    }
                }
            }
        }

        /// <summary>Return details about a specific job.</summary>
        public int GetNumDiffs(int JobID)
        {
            string SQL = "SELECT * FROM Classic WHERE ID = @JobID";

            using (SqlConnection connection = Open())
            {
                using (SqlCommand command = new SqlCommand(SQL, connection))
                {
                    command.Parameters.AddWithValue("@JobID", JobID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (!Convert.IsDBNull(reader["NumDiffs"]))
                                return Convert.ToInt32(reader["NumDiffs"]);
                        }
                        return 0;
                    }
                }
            }
        }

        /// <summary>Return details about a specific job.</summary>
        public int GetDoCommit(int JobID)
        {
            string SQL = "SELECT * FROM Classic WHERE ID = @JobID";

            using (SqlConnection connection = Open())
            {
                using (SqlCommand command = new SqlCommand(SQL, connection))
                {
                    command.Parameters.AddWithValue("@JobID", JobID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            return Convert.ToInt32(reader["DoCommit"]);
                        else
                            return 0;
                    }
                }
            }
        }

        /// <summary>Update the status of the specified build job.</summary>
        public void UpdateStatus(int JobID, string NewStatus, string DbConnectPassword)
        {
            if (DbConnectPassword == GetValidPassword())
            {
                string SQL = "UPDATE Classic SET Status = @Status WHERE ID = @JobID";
                using (SqlConnection connection = Open())
                {
                    using (SqlCommand command = new SqlCommand(SQL, connection))
                    {
                        command.Parameters.AddWithValue("@Status", NewStatus);
                        command.Parameters.AddWithValue("@JobID", JobID);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>Update the status of the specified build job.</summary>
        public void UpdateFieldName(int JobID, string FieldName, string FieldValue, string DbConnectPassword)
        {
            if (DbConnectPassword == GetValidPassword())
            {
                string SQL = "UPDATE Classic SET @FieldName = @FieldValue WHERE ID = @JobID";
                using (SqlConnection connection = Open())
                {
                    using (SqlCommand command = new SqlCommand(SQL, connection))
                    {
                        command.Parameters.AddWithValue("@FieldName", FieldName);
                        command.Parameters.AddWithValue("@FieldValue", FieldValue);
                        command.Parameters.AddWithValue("@JobID", JobID);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>Update the status of the specified build job.</summary>
        public void UpdateLinuxStatus(int JobID, string NewStatus, string DbConnectPassword)
        {
            if (DbConnectPassword == GetValidPassword())
            {
                string SQL = "UPDATE Classic SET LinuxStatus = @NewStatus WHERE ID = @JobID";
                using (SqlConnection connection = Open())
                {
                    using (SqlCommand command = new SqlCommand(SQL, connection))
                    {
                        command.Parameters.AddWithValue("@NewStatus", NewStatus);
                        command.Parameters.AddWithValue("@JobID", JobID);
                        
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>Update the status of the specified build job.</summary>
        public void UpdateStartDateToNow(int JobID, string DbConnectPassword)
        {
            if (DbConnectPassword == GetValidPassword())
            {
                string nowString = DateTime.Now.ToString("yyyy-MM-dd hh:mm tt");
                string SQL = "UPDATE Classic SET StartTime = @Now WHERE ID = @JobID";

                using (SqlConnection connection = Open())
                {
                    using (SqlCommand command = new SqlCommand(SQL, connection))
                    {
                        command.Parameters.AddWithValue("@Now", nowString);
                        command.Parameters.AddWithValue("@JobID", JobID);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>Update the status of the specified build job.</summary>
        public void UpdateEndDateToNow(int JobID, string DbConnectPassword)
        {
            if (DbConnectPassword == GetValidPassword())
            {
                string nowString = DateTime.Now.ToString("yyyy-MM-dd hh:mm tt");
                string SQL = "UPDATE Classic SET FinishTime = @Now WHERE ID = @JobID";

                using (SqlConnection connection = Open())
                {
                    using (SqlCommand command = new SqlCommand(SQL, connection))
                    {
                        command.Parameters.AddWithValue("@Now", nowString);
                        command.Parameters.AddWithValue("@JobID", JobID);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>Update the revision number for the specified build job.</summary>
        public void UpdateRevisionNumber(int JobID, int RevisionNumber, string DbConnectPassword)
        {
            if (DbConnectPassword == GetValidPassword())
            {
                string SQL = "UPDATE Classic SET RevisionNumber = @RevisionNumber WHERE ID = @JobID";

                using (SqlConnection connection = Open())
                {
                    using (SqlCommand command = new SqlCommand(SQL, connection))
                    {
                        command.Parameters.AddWithValue("@RevisionNumber", RevisionNumber);
                        command.Parameters.AddWithValue("@JobID", JobID);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>Update the revision number for the specified pull request.</summary>
        public void UpdateRevisionNumberForPR(int pullRequestID, int revisionNumber, string DbConnectPassword)
        {
            if (DbConnectPassword == GetValidPassword())
            {
                int jobID = GetIDOfLatestBuild(pullRequestID);
                UpdateRevisionNumber(jobID, revisionNumber, DbConnectPassword);
            }
        }

        private int GetIDOfLatestBuild(int pullID)
        {
            using (SqlConnection connection = Open())
            {
                string sql = "SELECT TOP (1) ID FROM Classic WHERE PullRequestID = @PullID ORDER BY ID DESC;";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@PullID", pullID);
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        /// <summary>Update the paths for all the revision number for the specified build job.</summary>
        public void UpdateDiffFileName(int JobID, string DiffsFileName, string DbConnectPassword)
        {
            if (DbConnectPassword == GetValidPassword())
            {
                string SQL = "UPDATE Classic SET DiffsFileName = @DiffsFileName WHERE ID = @JobID";

                using (SqlConnection connection = Open())
                {
                    using (SqlCommand command = new SqlCommand(SQL, connection))
                    {
                        command.Parameters.AddWithValue("@DiffsFileName", DiffsFileName);
                        command.Parameters.AddWithValue("@JobID", JobID);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>Set the number of diffs.</summary>
        public void UpdateNumDiffs(int JobID, int NumDiffs, string DbConnectPassword)
        {
            if (DbConnectPassword == GetValidPassword())
            {
                string SQL = "UPDATE Classic SET NumDiffs = @NumDiffs WHERE ID = @JobID";

                using (SqlConnection connection = Open())
                {
                    using (SqlCommand command = new SqlCommand(SQL, connection))
                    {
                        command.Parameters.AddWithValue("@NumDiffs", NumDiffs);
                        command.Parameters.AddWithValue("@JobID", JobID);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>Updates a field in the database for the specified job with the specified value.</summary>
        public void UpdateField(int JobID, string FieldName, string FieldValue, string DbConnectPassword)
        {
            if (DbConnectPassword == GetValidPassword())
            {
                string SQL = "UPDATE Classic SET @FieldName = @FieldValue WHERE ID = @JobID";

                using (SqlConnection connection = Open())
                {
                    using (SqlCommand command = new SqlCommand(SQL, connection))
                    {
                        command.Parameters.AddWithValue("@FieldName", FieldName);
                        command.Parameters.AddWithValue("@FieldValue", FieldValue);
                        command.Parameters.AddWithValue("@JobID", JobID);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>Update the patch file name for the given pull request.</summary>
        public void UpdatePatchFileName(int pullRequestID, string patchFileName, string DbConnectPassword)
        {
            if (DbConnectPassword == GetValidPassword())
            {
                int jobID = GetIDOfLatestBuild(pullRequestID);
                string sql = "UPDATE Classic SET PatchFileName = @PatchFileName WHERE ID = @JobID";
                using (SqlConnection connection = Open())
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@PatchFileName", patchFileName);
                        command.Parameters.AddWithValue("@JobID", jobID);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>Find the next job to run.</summary>
        public int FindNextJob()
        {
            int JobID = -1;
            string SQL = "SELECT ID FROM Classic WHERE Status = 'Queued' ORDER BY ID";

            using (SqlConnection connection = Open())
            {
                using (SqlCommand command = new SqlCommand(SQL, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            JobID = Convert.ToInt32(reader["ID"]);
                        reader.Close();
                        return JobID;
                    }
                }
            }
        }

        /// <summary>Find the next job to run.</summary>
        public int FindNextLinuxJob()
        {
            int JobID = -1;
            string sql = "SELECT ID FROM Classic WHERE Status = 'Pass' AND LinuxStatus = 'Queued' ORDER BY ID";
            using (SqlConnection connection = Open())
            {
                MarkFailedJobs(connection);
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            JobID = Convert.ToInt32(reader["ID"]);
                        reader.Close();
                        return JobID;
                    }
                }
            }
        }

        /// <summary>Mark "failed" jobs</summary>
        private static void MarkFailedJobs(SqlConnection Connection)
        {
            string SQL = "SELECT ID FROM Classic WHERE (Status = 'Fail' OR Status = 'Aborted') AND LinuxStatus = 'Queued' ORDER BY ID";
            List<int> ignoredJobs = new List<int>();
            using (SqlCommand command = new SqlCommand(SQL, Connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        ignoredJobs.Add(Convert.ToInt32(reader[0]));
                }

            BuildsClassic builds = new BuildsClassic();
            foreach (int jobID in ignoredJobs)
                builds.UpdateField(jobID, "linuxStatus", "Ignored", GetValidPassword());
        }

        /// <summary>Delete the specified job.</summary>
        public void DeleteJob(int JobNumber, string DbConnectPassword)
        {
            if (DbConnectPassword == GetValidPassword())
            {
                string SQL = "DELETE FROM Classic WHERE ID = @JobID";
                using (SqlConnection connection = Open())
                {
                    using (SqlCommand command = new SqlCommand(SQL, connection))
                    {
                        command.Parameters.AddWithValue("@JobID", JobNumber);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>Return a list of build jobs.</summary>
        public BuildJob[] GetJobs(int NumRows, bool PassOnly)
        {
            string sql;
            if (PassOnly)
                sql = "SELECT TOP (@NumRows) * " +
                         " FROM Classic " +
                         " WHERE Status = 'Pass'" +
                         " ORDER BY ID DESC";
            else
                sql = "SELECT TOP (@NumRows) * " +
                         " FROM Classic " +
                         " ORDER BY ID DESC";

            using (SqlConnection connection = Open())
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@NumRows", NumRows));
                    using (SqlDataReader reader = command.ExecuteReader())
                        return GetBuildJobs(reader).ToArray();
                }
            }
        }

        /// <summary>Return a list of build jobs which have been released.</summary>
        /// <param name="numRows">Maximum number of results.</param>
        public BuildJob[] GetReleases(int numRows)
        {
            using (SqlConnection connection = Open())
            {
                string sql = "SELECT TOP (@NumRows) * FROM Classic WHERE RevisionNumber IS NOT NULL ORDER BY ID DESC;";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@NumRows", numRows);
                    using (SqlDataReader reader = command.ExecuteReader())
                        return GetBuildJobs(reader).ToArray();
                }
            }
        }

        private IEnumerable<BuildJob> GetBuildJobs(SqlDataReader reader)
        {
            string filesURL = "https://apsimdev.apsim.info/APSIMClassicFiles/";

            while (reader.Read())
            {
                string baseFileName = Path.GetFileNameWithoutExtension((string)reader["PatchFileName"]);

                BuildJob buildJob = new BuildJob();
                buildJob.ID = (int)reader["ID"];
                buildJob.PatchFileName = reader["PatchFileName"]?.ToString();
                buildJob.UserName = (string)reader["UserName"];
                if (!Convert.IsDBNull(reader["RevisionNumber"]))
                    buildJob.Revision = (int)reader["RevisionNumber"];
                buildJob.StartTime = (DateTime)reader["StartTime"];
                buildJob.TaskID = (int)reader["BugID"];
                buildJob.Description = reader["Description"].ToString();
                buildJob.JenkinsID = (int)reader["JenkinsID"];
                buildJob.BuiltOnJenkins = buildJob.JenkinsID >= 0;
                buildJob.WindowsStatus = (string)reader["Status"];
                // PullRequestID will be null for the old Bob builds.
                if (!Convert.IsDBNull(reader["PullRequestID"]))
                    buildJob.PullRequestID = Convert.ToInt32(reader["PullRequestID"]);

                if (!Convert.IsDBNull(reader["FinishTime"]))
                {
                    buildJob.Duration = 0;
                    DateTime finishTime = (DateTime)reader["FinishTime"];
                    buildJob.Duration = Convert.ToInt32((finishTime - buildJob.StartTime).TotalMinutes);
                }

                if (buildJob.BuiltOnJenkins /*&& buildJob.Revision != 0*/)
                {
                    buildJob.PatchFileURL = filesURL + buildJob.PullRequestID + ".zip";
                    buildJob.PatchFileNameShort = buildJob.PatchFileName;
                    buildJob.WindowsDetailsURL = $"http://apsimdev.apsim.info:8080/jenkins/job/PullRequestClassic/{buildJob.JenkinsID}/consoleText";
                    buildJob.XmlUrl = filesURL + buildJob.PullRequestID + ".xml";
                    buildJob.PatchFileURL = $"https://github.com/APSIMInitiative/APSIMClassic/pull/{buildJob.PullRequestID}";
                    buildJob.IssueURL = $"https://github.com/APSIMInitiative/APSIMClassic/issues/{buildJob.TaskID}";
                    buildJob.WindowsBinariesURL = filesURL + buildJob.PullRequestID + ".binaries.zip";
                    buildJob.WindowsBuildTreeURL = filesURL + buildJob.PullRequestID + ".buildtree.zip";
                }
                else
                {
                    buildJob.PatchFileURL = filesURL + buildJob.PatchFileName;
                    buildJob.PatchFileNameShort = GetShortPatchFileName((string)reader["PatchFileName"]);
                    buildJob.WindowsDetailsURL = filesURL + baseFileName + ".txt";
                    buildJob.XmlUrl = Path.ChangeExtension(buildJob.PatchFileURL, ".xml");
                    buildJob.IssueURL = $"http://apsimdev.apsim.info/BugTracker/edit_bug.aspx?id={buildJob.TaskID}";
                    buildJob.WindowsBinariesURL = filesURL + baseFileName + ".binaries.zip";
                    buildJob.WindowsBuildTreeURL = filesURL + baseFileName + ".buildtree.zip";
                }

                if (!Convert.IsDBNull(reader["NumDiffs"]))
                    buildJob.WindowsNumDiffs = (int)reader["NumDiffs"];

                if (buildJob.WindowsNumDiffs > 0)
                    buildJob.WindowsDiffsURL = filesURL + buildJob.PullRequestID + ".diffs.zip";

                string versionString;
                if (buildJob.Revision < 3855)
                    versionString = "Apsim7.7-r";
                else if (buildJob.Revision < 4035)
                    versionString = "Apsim7.8-r";
                else if (buildJob.Revision < 4133)
                    versionString = "Apsim7.9-r";
                else
                    versionString = "Apsim7.10-r";

                buildJob.VersionString = versionString + buildJob.Revision;

                if (buildJob.WindowsNumDiffs == 0 && buildJob.WindowsStatus == "Pass")
                {
                    if (buildJob.BuiltOnJenkins)
                    {
                        if (buildJob.Revision != 0)
                        {
                            // we no longer provide Win32 SFX.
                            buildJob.Win32SFXURL = null;
                            buildJob.Win64SFXURL = filesURL + buildJob.PatchFileName + ".binaries.WINDOWS.X86_64.exe";
                            buildJob.WindowsInstallerFullURL = filesURL + buildJob.PatchFileName + ".bootleg.exe";
                            buildJob.WindowsInstallerURL = filesURL + buildJob.PatchFileName + ".apsimsetup.exe";
                        }
                    }
                    else
                    {
                        buildJob.Win32SFXURL = filesURL + versionString + buildJob.Revision + ".binaries.WINDOWS.INTEL.exe";
                        buildJob.Win64SFXURL = filesURL + versionString + buildJob.Revision + ".binaries.WINDOWS.X86_64.exe";
                        buildJob.WindowsInstallerFullURL = filesURL + baseFileName + ".bootleg.exe";
                        buildJob.WindowsInstallerURL = filesURL + baseFileName + ".apsimsetup.exe";
                    }
                }

                if (buildJob.LinuxStatus == "Pass")
                {
                    buildJob.LinuxBinariesURL = filesURL + versionString + buildJob.Revision + ".LINUX.X86_64.exe";
                    buildJob.LinuxDetailsURL = filesURL + versionString + buildJob.Revision + ".linux.txt";
                    buildJob.LinuxDiffsURL = filesURL + versionString + buildJob.Revision + ".linux.txt";
                }

                if (!Convert.IsDBNull(reader["LinuxNumDiffs"]))
                    buildJob.LinuxNumDiffs = (int)reader["LinuxNumDiffs"];
                buildJob.LinuxStatus = (string)reader["LinuxStatus"];

                yield return buildJob;
            }
        }

        private string GetShortPatchFileName(string patchFileURL)
        {
            int posOpenBracket = patchFileURL.IndexOf('(');
            if (posOpenBracket != -1)
                return patchFileURL.Substring(0, posOpenBracket);
            else
                return patchFileURL;
        }

        /// <summary>Return a list of open bugs</summary>
        public BugTracker[] GetIssueList()
        {
            List<BugTracker> bugs = new List<BugTracker>();

            string connectionString = File.ReadAllText(@"D:\Websites\bugTrackerConnect.txt");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT bg_id, bg_short_desc FROM bugs WHERE (bg_status <> 5) ORDER BY bg_id DESC";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            BugTracker bug = new BugTracker();
                            bug.bugID = (int)reader["bg_id"];
                            bug.description = (string)reader["bg_short_desc"];
                            bugs.Add(bug);
                        }
                    }
                }

                connection.Close();
            }

            return bugs.ToArray();
        }

        /// <summary>Open the SoilsDB ready for use.</summary>
        public static SqlConnection Open(string databaseName = "APSIM.Builds")
        {
            string connectionString = File.ReadAllText(@"D:\Websites\dbConnect.txt") + ";Database=\"" + databaseName +"\""; ;
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }

        /// <summary>Return the valid password for this web service.</summary>
        public static string GetValidPassword()
        {
            string connectionString = File.ReadAllText(@"D:\Websites\ChangeDBPassword.txt");
            int posPassword = connectionString.IndexOf("Password=");
            return connectionString.Substring(posPassword + "Password=".Length);
        }

        /// <summary>
        /// Gets the ID of the issue referenced by a pull request.
        /// </summary>
        /// <param name="pullRequestID">ID of the pull request.</param>
        public int GetIssueID(int pullRequestID)
        {
            PullRequest pull = GitHubUtilities.GetPullRequest(pullRequestID, repoOwner, repoName);
            return pull.GetIssueID();
        }

        /// <summary>
        /// Get the latest revision number.
        /// </summary>
        /// <returns></returns>
        public int GetLatestRevisionNo()
        {
            string sql = "SELECT TOP 1 RevisionNumber FROM Classic ORDER BY RevisionNumber DESC;";

            using (SqlConnection connection = Open())
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                        using (SqlDataReader reader = command.ExecuteReader())
                            if (reader.Read())
                                return (int)reader["RevisionNumber"];
                }
                finally
                {
                    connection.Close();
                }
            }

            throw new Exception("Unable to get latest revision number.");
        }
    }
}

