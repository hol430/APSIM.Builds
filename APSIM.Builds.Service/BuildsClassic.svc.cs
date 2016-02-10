
namespace APSIM.Builds.Service
{
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
                     Namespace = "http://www.apsim.info/services")]
    public class BuildsClassic : IBuildsClassic
    {
        /// <summary>Add a new entry to the builds database.</summary>
        public void Add(string UserName, string Password, string PatchFileName, string Description, int BugID, bool DoCommit, string DbConnectPassword)
        {
            if (DbConnectPassword == GetValidPassword())
            {
                string SQL = "INSERT INTO Classic (UserName, Password, PatchFileName, Description, BugID, DoCommit, Status, StartTime, linuxStatus) " +
                             "VALUES (@UserName, @Password, @PatchFileName, @Description, @BugID, @DoCommit, @Status, @StartTime, @LinuxStatus)";

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
                        command.Parameters.Add(new SqlParameter("@Status", "Queued"));
                        command.Parameters.Add(new SqlParameter("@LinuxStatus", "Queued"));
                        command.Parameters.Add(new SqlParameter("@StartTime", NowString));
                        if (DoCommit)
                            command.Parameters.Add(new SqlParameter("@DoCommit", "1"));
                        else
                            command.Parameters.Add(new SqlParameter("@DoCommit", "0"));
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>Return details about a specific job.</summary>
        public string GetStatus(int JobID)
        {
            string SQL = "SELECT * FROM Classic WHERE ID = " + JobID.ToString();

            using (SqlConnection connection = Open())
            {
                using (SqlCommand command = new SqlCommand(SQL, connection))
                {
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
            string SQL = "SELECT * FROM Classic WHERE ID = " + JobID.ToString();

            using (SqlConnection connection = Open())
            {
                using (SqlCommand command = new SqlCommand(SQL, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            return "http://www.apsim.info/APSIM.Builds.Portal/Files/" + reader["PatchFileName"].ToString();
                        else
                            return null;
                    }
                }
            }
        }

        /// <summary>Return details about a specific job.</summary>
        public string GetRevisionNumber(int JobID)
        {
            string SQL = "SELECT * FROM Classic WHERE ID = " + JobID.ToString();

            using (SqlConnection connection = Open())
            {
                using (SqlCommand command = new SqlCommand(SQL, connection))
                {
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
            string SQL = "SELECT * FROM Classic WHERE ID = " + JobID.ToString();

            using (SqlConnection connection = Open())
            {
                using (SqlCommand command = new SqlCommand(SQL, connection))
                {
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
                string SQL = "SELECT * FROM Classic WHERE ID = " + JobID.ToString();

                using (SqlConnection connection = Open())
                {
                    using (SqlCommand command = new SqlCommand(SQL, connection))
                    {
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
            string SQL = "SELECT * FROM Classic WHERE ID = " + JobID.ToString();

            using (SqlConnection connection = Open())
            {
                using (SqlCommand command = new SqlCommand(SQL, connection))
                {
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
            string SQL = "SELECT * FROM Classic WHERE ID = " + JobID.ToString();

            using (SqlConnection connection = Open())
            {
                using (SqlCommand command = new SqlCommand(SQL, connection))
                {
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
            string SQL = "SELECT * FROM Classic WHERE ID = " + JobID.ToString();

            using (SqlConnection connection = Open())
            {
                using (SqlCommand command = new SqlCommand(SQL, connection))
                {
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
            string SQL = "SELECT * FROM Classic WHERE ID = " + JobID.ToString();

            using (SqlConnection connection = Open())
            {
                using (SqlCommand command = new SqlCommand(SQL, connection))
                {
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
                string SQL = "UPDATE Classic SET Status = '" + NewStatus + "' WHERE ID = " + JobID.ToString();
                using (SqlConnection connection = Open())
                {
                    using (SqlCommand command = new SqlCommand(SQL, connection))
                    {
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
                string SQL = "UPDATE Classic SET " + FieldName + " = '" + FieldValue + "' WHERE ID = " + JobID.ToString();
                using (SqlConnection connection = Open())
                {
                    using (SqlCommand command = new SqlCommand(SQL, connection))
                    {
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
                string SQL = "UPDATE Classic SET LinuxStatus = '" + NewStatus + "' WHERE ID = " + JobID.ToString();
                using (SqlConnection connection = Open())
                {
                    using (SqlCommand command = new SqlCommand(SQL, connection))
                    {
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
                string SQL = "UPDATE Classic SET StartTime = '" + nowString + "' WHERE ID = " + JobID.ToString();

                using (SqlConnection connection = Open())
                {
                    using (SqlCommand command = new SqlCommand(SQL, connection))
                    {
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
                string SQL = "UPDATE Classic SET FinishTime = '" + nowString + "' WHERE ID = " + JobID.ToString();

                using (SqlConnection connection = Open())
                {
                    using (SqlCommand command = new SqlCommand(SQL, connection))
                    {
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
                string SQL = "UPDATE Classic SET RevisionNumber = " + RevisionNumber.ToString() + " WHERE ID = " + JobID.ToString();

                using (SqlConnection connection = Open())
                {
                    using (SqlCommand command = new SqlCommand(SQL, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>Update the paths for all the revision number for the specified build job.</summary>
        public void UpdateDiffFileName(int JobID, string DiffsFileName, string DbConnectPassword)
        {
            if (DbConnectPassword == GetValidPassword())
            {
                string SQL = "UPDATE Classic SET DiffsFileName = '" + DiffsFileName + "'" +
                                                " WHERE ID = " + JobID.ToString();

                using (SqlConnection connection = Open())
                {
                    using (SqlCommand command = new SqlCommand(SQL, connection))
                    {
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
                string SQL = "UPDATE Classic SET NumDiffs = " + NumDiffs.ToString() + " WHERE ID = " + JobID.ToString();

                using (SqlConnection connection = Open())
                {
                    using (SqlCommand command = new SqlCommand(SQL, connection))
                    {
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
                string SQL = "UPDATE Classic SET " + FieldName + " = '" + FieldValue + "'" +
                                                " WHERE ID = " + JobID.ToString();

                using (SqlConnection connection = Open())
                {
                    using (SqlCommand command = new SqlCommand(SQL, connection))
                    {
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

        /// <summary>Delete the specified job.</summary>
        public void DeleteJob(int JobNumber, string DbConnectPassword)
        {
            if (DbConnectPassword == GetValidPassword())
            {
                string SQL = "DELETE FROM Classic WHERE ID = " + JobNumber.ToString();
                using (SqlConnection connection = Open())
                {
                    using (SqlCommand command = new SqlCommand(SQL, connection))
                    {
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
                sql = "SELECT TOP (" + NumRows.ToString() + ") * " +
                         " FROM Classic " +
                         " WHERE Status = 'Pass'" +
                         " ORDER BY ID DESC";
            else
                sql = "SELECT TOP (" + NumRows.ToString() + ") * " +
                         " FROM Classic " +
                         " ORDER BY ID DESC";

            string filesURL = "http://bob.apsim.info/files/";

            List<BuildJob> buildJobs = new List<BuildJob>();
            using (SqlConnection connection = Open())
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string baseFileName = Path.GetFileNameWithoutExtension((string)reader["PatchFileName"]);

                            BuildJob buildJob = new BuildJob();
                            buildJob.ID = (int)reader["ID"];
                            buildJob.UserName = (string)reader["UserName"];
                            if (!Convert.IsDBNull(reader["RevisionNumber"]))
                                buildJob.Revision = (int)reader["RevisionNumber"];
                            buildJob.StartTime = (DateTime)reader["StartTime"];
                            buildJob.TaskID = (int)reader["BugID"];
                            buildJob.Description = reader["Description"].ToString();

                            if (!Convert.IsDBNull(reader["FinishTime"]))
                            {
                                buildJob.Duration = 0;
                                DateTime finishTime = (DateTime)reader["FinishTime"];
                                buildJob.Duration = Convert.ToInt32((finishTime - buildJob.StartTime).TotalMinutes);
                            }

                            buildJob.PatchFileURL = "http://www.apsim.info/APSIM.Builds.Portal/Files/" + baseFileName + ".zip";


                            if (!Convert.IsDBNull(reader["NumDiffs"]))
                                buildJob.WindowsNumDiffs = (int)reader["NumDiffs"];

                            if (buildJob.WindowsNumDiffs > 0)
                                buildJob.WindowsDiffsURL = filesURL + baseFileName + ".diffs.zip";

                            buildJob.WindowsBinariesURL = filesURL + baseFileName + ".binaries.zip";
                            buildJob.WindowsBuildTreeURL = filesURL + baseFileName + ".buildtree.zip";
                            buildJob.WindowsDetailsURL = filesURL + baseFileName + ".txt";

                            if (buildJob.WindowsNumDiffs == 0)
                            {
                                buildJob.WindowsInstallerFullURL = filesURL + baseFileName + ".bootleg.exe"; ;
                                buildJob.WindowsInstallerURL = filesURL + baseFileName + ".apsimsetup.zip";
                                buildJob.Win32SFXURL = filesURL + "Apsim7.7-r" + buildJob.Revision + ".binaries.WINDOWS.INTEL.exe";
                                buildJob.Win64SFXURL = filesURL + "Apsim7.7-r" + buildJob.Revision + ".binaries.WINDOWS.X86_64.exe";
                            }

                            buildJob.WindowsStatus = (string)reader["Status"];

                            buildJob.LinuxBinariesURL = filesURL + "Apsim7.7-r" + buildJob.Revision + ".LINUX.X86_64.exe";
                            buildJob.LinuxDetailsURL = filesURL + "Apsim7.7-r" + buildJob.Revision + ".linux.txt";
                            buildJob.LinuxDiffsURL = filesURL + "Apsim7.7-r" + buildJob.Revision + ".linux.txt";
                            if (!Convert.IsDBNull(reader["LinuxNumDiffs"]))
                                buildJob.LinuxNumDiffs = (int)reader["LinuxNumDiffs"];
                            buildJob.LinuxStatus = (string)reader["LinuxStatus"];

                            buildJobs.Add(buildJob);
                        }
                    }
                }
            }
            return buildJobs.ToArray();
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

    }
}

