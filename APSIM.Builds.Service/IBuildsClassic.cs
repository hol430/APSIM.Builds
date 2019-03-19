namespace APSIM.Builds.Service
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    [ServiceContract(Namespace = "")]
    public interface IBuildsClassic
    {
        /// <summary>Add a new entry to the builds database.</summary>
        [OperationContract]
        [WebGet(UriTemplate = "/Add?UserName={UserName}&Password={Password}&PatchFileName={PatchFileName}&Description={Description}&BugID={BugID}&DoCommit={DoCommit}&JenkinsID={JenkinsID}&DbConnectPassword={DbConnectPassword}", 
                BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        int Add(string UserName, string Password, string PatchFileName, string Description, int BugID, bool DoCommit, int JenkinsID, string DbConnectPassword);

        /// <summary>Add a new entry to the builds database.</summary>
        [OperationContract]
        [WebGet(UriTemplate = "/AddPullRequest?PullID={PullID}&JenkinsID={JenkinsID}&Password={Password}&DbConnectPassword={DbConnectPassword}",
                BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        int AddPullRequest(int PullID, int JenkinsID, string Password, string DbConnectPassword);

        /// <summary>Return details about a specific job.</summary>
        [OperationContract]
        [WebGet(UriTemplate = "/GetStatus?JobID={JobID}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetStatus(int JobID);

        /// <summary>Return details about a specific job.</summary>
        [OperationContract]
        [WebGet(UriTemplate = "/GetPatchFileName?JobID={JobID}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetPatchFileName(int JobID);

        /// <summary>Return details about a specific job.</summary>
        [OperationContract]
        [WebGet(UriTemplate = "/GetRevisionNumber?JobID={JobID}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetRevisionNumber(int JobID);

        /// <summary>Return details about a specific job.</summary>
        [OperationContract]
        [WebGet(UriTemplate = "/GetUserName?JobID={JobID}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetUserName(int JobID);

        /// <summary>Return details about a specific job.</summary>
        [OperationContract]
        [WebGet(UriTemplate = "/GetPassword?JobID={JobID}&DbConnectPassword={DbConnectPassword}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetPassword(int JobID, string DbConnectPassword);

        /// <summary>Return details about a specific job.</summary>
        [OperationContract]
        [WebGet(UriTemplate = "/GetDescription?JobID={JobID}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetDescription(int JobID);

        /// <summary>Return details about a specific job.</summary>
        [OperationContract]
        [WebGet(UriTemplate = "/GetBugID?JobID={JobID}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetBugID(int JobID);

        /// <summary>Return details about a specific job.</summary>
        [OperationContract]
        [WebGet(UriTemplate = "/GetNumDiffs?JobID={JobID}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        int GetNumDiffs(int JobID);

        /// <summary>Return details about a specific job.</summary>
        [OperationContract]
        [WebGet(UriTemplate = "/GetDoCommit?JobID={JobID}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        int GetDoCommit(int JobID);

        /// <summary>Update the status of the specified build job.</summary>
        [OperationContract]
        [WebGet(UriTemplate = "/UpdateStatus?JobID={JobID}&NewStatus={NewStatus}&DbConnectPassword={DbConnectPassword}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void UpdateStatus(int JobID, string NewStatus, string DbConnectPassword);

        /// <summary>Update the status of the specified build job.</summary>
        [OperationContract]
        [WebGet(UriTemplate = "/UpdateFieldName?JobID={JobID}&FieldName={FieldName}&FieldValue={FieldValue}&DbConnectPassword={DbConnectPassword}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void UpdateFieldName(int JobID, string FieldName, string FieldValue, string DbConnectPassword);

        /// <summary>Update the status of the specified build job.</summary>
        [OperationContract]
        [WebGet(UriTemplate = "/UpdateLinuxStatus?JobID={JobID}&Status={NewStatus}&DbConnectPassword={DbConnectPassword}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void UpdateLinuxStatus(int JobID, string NewStatus, string DbConnectPassword);

        /// <summary>Update the status of the specified build job.</summary>
        [OperationContract]
        [WebGet(UriTemplate = "/UpdateStartDateToNow?JobID={JobID}&DbConnectPassword={DbConnectPassword}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void UpdateStartDateToNow(int JobID, string DbConnectPassword);

        /// <summary>Update the status of the specified build job.</summary>
        [OperationContract]
        [WebGet(UriTemplate = "/UpdateEndDateToNow?JobID={JobID}&DbConnectPassword={DbConnectPassword}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void UpdateEndDateToNow(int JobID, string DbConnectPassword);

        /// <summary>Update the revision number for the specified build job.</summary>
        [OperationContract]
        [WebGet(UriTemplate = "/UpdateRevisionNumber?JobID={JobID}&RevisionNumber={RevisionNumber}&DbConnectPassword={DbConnectPassword}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void UpdateRevisionNumber(int JobID, int RevisionNumber, string DbConnectPassword);

        /// <summary>Update the paths for all the revision number for the specified build job.</summary>
        [OperationContract]
        [WebGet(UriTemplate = "/UpdateDiffFileName?JobID={JobID}&DiffsFileName={DiffsFileName}&DbConnectPassword={DbConnectPassword}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void UpdateDiffFileName(int JobID, string DiffsFileName, string DbConnectPassword);

        /// <summary>Set the number of diffs.</summary>
        [OperationContract]
        [WebGet(UriTemplate = "/UpdateNumDiffs?JobID={JobID}&NumDiffs={NumDiffs}&DbConnectPassword={DbConnectPassword}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void UpdateNumDiffs(int JobID, int NumDiffs, string DbConnectPassword);

        /// <summary>Updates a field in the database for the specified job with the specified value.</summary>
        [OperationContract]
        [WebGet(UriTemplate = "/UpdateField?JobID={JobID}&FieldName={FieldName}&FieldValue={FieldValue}&DbConnectPassword={DbConnectPassword}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void UpdateField(int JobID, string FieldName, string FieldValue, string DbConnectPassword);

        /// <summary>Find the next job to run.</summary>
        [OperationContract]
        [WebGet(UriTemplate = "/FindNextJob", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        int FindNextJob();

        /// <summary>Find the next job to run.</summary>
        [WebGet(UriTemplate = "/FindNextLinuxJob", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        int FindNextLinuxJob();

        /// <summary>Delete the specified job.</summary>
        [OperationContract]
        [WebGet(UriTemplate = "/DeleteJob?JobID={JobID}&DbConnectPassword={DbConnectPassword}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void DeleteJob(int JobID, string DbConnectPassword);

        /// <summary>Return a list of build jobs.</summary>
        [OperationContract]
        [WebGet(UriTemplate = "/GetJobs?NumRows={NumRows}&PassOnly={PassOnly}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        BuildJob[] GetJobs(int NumRows, bool PassOnly);

        /// <summary>Return a list of open bugs</summary>
        [OperationContract]
        [WebGet(UriTemplate = "/GetIssueList")]
        BugTracker[] GetIssueList();

        /// <summary>
        /// Gets the ID of the issue referenced by a pull request.
        /// </summary>
        /// <param name="pullRequestID">ID of the pull request.</param>
        [OperationContract]
        [WebGet(UriTemplate = "/GetIssueID?pullRequestID={pullRequestID}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        int GetIssueID(int pullRequestID);
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

    /// <summary>A bug</summary>
    public class BugTracker
    {
        public int bugID;
        public string description;
    }
}
