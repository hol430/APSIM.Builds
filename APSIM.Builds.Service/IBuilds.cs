using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace APSIM.Builds.Service
{
    /// <summary>
    /// Web service that provides access to the ApsimX builds system.
    /// </summary>
    [ServiceContract]
    public interface IBuilds
    {
        /// <summary>Add a build to the build database.</summary>
        /// <param name="pullRequestNumber">The GitHub pull request number.</param>
        /// <param name="issueID">The issue ID.</param>
        /// <param name="issueTitle">The issue title.</param>
        [OperationContract]
        [WebGet(UriTemplate = "/AddBuild?pullRequestNumber={pullRequestNumber}&issueID={issueID}&issueTitle={issueTitle}&ChangeDBPassword={ChangeDBPassword}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void AddBuild(int pullRequestNumber, int issueID, string issueTitle, string ChangeDBPassword);

        /// <summary>
        /// Gets a list of possible upgrades since the specified issue number.
        /// </summary>
        /// <param name="issueNumber">The issue number.</param>
        /// <returns>The list of possible upgrades.</returns>
        [OperationContract]
        [WebGet(UriTemplate = "/GetUpgradesSinceIssue?issueID={issueID}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<Upgrade> GetUpgradesSinceIssue(int issueID);

        /// <summary>
        /// Gets the URL of the latest version.
        /// </summary>
        /// <returns>The URL of the latest version of APSIM Next Generation.</returns>
        [OperationContract]
        [WebGet(UriTemplate = "/GetURLOfLatestVersion", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetURLOfLatestVersion();

        /// <summary>
        /// Get a GitHub issue ID from a pull request ID.
        /// </summary>
        [OperationContract]
        [WebGet(UriTemplate = "/GetIssueID?pullRequestID={pullRequestID}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        int GetIssueID(int pullRequestID);

        /// <summary>
        /// Get latest documentation HTML.
        /// </summary>
        [OperationContract]
        [WebGet(UriTemplate = "/GetDocumentationHTML", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        Stream GetDocumentationHTML();

    }

    /// <summary>
    /// An class encapsulating an upgrade 
    /// </summary>
    public class Upgrade
    {
        public DateTime ReleaseDate { get; set; }
        public int issueNumber { get; set; }
        public string IssueTitle { get; set; }
        public string IssueURL { get; set; }
        public string ReleaseURL { get; set; }
    }

}
