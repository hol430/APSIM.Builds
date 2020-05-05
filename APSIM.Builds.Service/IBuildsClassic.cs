namespace APSIM.Builds.Service
{
    using APSIM.Shared.Web;
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    [ServiceContract(Namespace = "")]
    public interface IBuildsClassic
    {
        /// <summary>Add a new entry to the builds database.</summary>
        [OperationContract]
        [WebGet(UriTemplate = "/Add?UserName={UserName}&Password={Password}&PatchFileName={PatchFileName}&Description={Description}&BugID={BugID}&DoCommit={DoCommit}&JenkinsID={JenkinsID}&PullID={PullID}&DbConnectPassword={DbConnectPassword}", 
                BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        int Add(string UserName, string Password, string PatchFileName, string Description, int BugID, bool DoCommit, int JenkinsID, int PullID, string DbConnectPassword);

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

        /// <summary>Update the patch file name for the given pull request.</summary>
        [OperationContract]
        [WebGet(UriTemplate = "/UpdatePatchFileName?pullRequestID={pullRequestID}&patchFileName={patchFileName}&DbConnectPassword={DbConnectPassword}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void UpdatePatchFileName(int pullRequestID, string patchFileName, string DbConnectPassword);

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

        /// <summary>Update the revision number for the specified pull request.</summary>
        [OperationContract]
        [WebGet(UriTemplate = "/UpdateRevisionNumberForPR?pullRequestID={pullRequestID}&revisionNumber={revisionNumber}&DbConnectPassword={DbConnectPassword}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void UpdateRevisionNumberForPR(int pullRequestID, int revisionNumber, string DbConnectPassword);

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

        /// <summary>Return a list of build jobs which have been released.</summary>
        /// <param name="numRows">Maximum number of results.</param>
        [OperationContract]
        [WebGet(UriTemplate = "/GetReleases?numRows={numRows}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        BuildJob[] GetReleases(int numRows);

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

        /// <summary>Get the latest revision number.</summary>
        [OperationContract]
        [WebGet(UriTemplate = "/GetLatestRevisionNo", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        int GetLatestRevisionNo();
    }

    /// <summary>A bug</summary>
    public class BugTracker
    {
        public int bugID;
        public string description;
    }
}
