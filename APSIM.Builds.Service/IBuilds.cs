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
        /// <param name="changeDBPassword">The passowrd.</param>
        [OperationContract]
        [WebGet(UriTemplate = "/AddBuild?pullRequestNumber={pullRequestNumber}&ChangeDBPassword={changeDBPassword}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void AddBuild(int pullRequestNumber, string changeDBPassword);

        /// <summary>Add a green build to the build database.</summary>
        /// <param name="pullRequestNumber">The GitHub pull request number.</param>
        /// <param name="buildTimeStamp">The build time stamp</param>
        /// <param name="changeDBPassword">The password</param>
        [OperationContract]
        [WebGet(UriTemplate = "/AddGreenBuild?pullRequestNumber={pullRequestNumber}&buildTimeStamp={buildTimeStamp}&changeDBPassword={changeDBPassword}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void AddGreenBuild(int pullRequestNumber, string buildTimeStamp, string changeDBPassword);

        /// <summary>
        /// Gets a list of possible upgrades since the specified issue number.
        /// </summary>
        /// <param name="issueNumber">The issue number.</param>
        /// <returns>The list of possible upgrades.</returns>
        [OperationContract]
        [WebGet(UriTemplate = "/GetUpgradesSinceIssue?issueID={issueID}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<Upgrade> GetUpgradesSinceIssue(int issueID);

        /// <summary>
        /// Gets a list of possible upgrades since the specified Apsim version.
        /// </summary>
        /// <param name="version">Fully qualified (a.b.c.d) version number.</param>
        /// <returns>List of possible upgrades.</returns>
        [OperationContract]
        [WebGet(UriTemplate = "/GetUpgradesSinceVersion?version={version}", BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<Upgrade> GetUpgradesSinceVersion(string version);

        /// <summary>
        /// Gets the URL of the latest version.
        /// </summary>
        /// <returns>The URL of the latest version of APSIM Next Generation.</returns>
        [OperationContract]
        [WebGet(UriTemplate = "/GetURLOfLatestVersion?operatingSystem={operatingSystem}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetURLOfLatestVersion(string operatingSystem);

        /// <summary>
        /// Gets a URL for a version that resolves the specified issue
        /// </summary>
        /// <param name="issueNumber">The issue number.</param>
        [OperationContract]
        [WebGet(UriTemplate = "/GetURLOfVersionForIssue?issueID={issueID}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetURLOfVersionForIssue(int issueID);

        /// <summary>
        /// Get a GitHub issue ID from a pull request ID.
        /// </summary>
        [OperationContract]
        [WebGet(UriTemplate = "/GetPullRequestDetails?pullRequestID={pullRequestID}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetPullRequestDetails(int pullRequestID);

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
        public int IssueNumber { get; set; }
        public string IssueTitle { get; set; }
        public string IssueURL { get; set; }
        public string ReleaseURL { get; set; }

        // Leaving this here for compatibility reasons.
        [Obsolete("Deprecated in favour of IssueNumber.")]
        public int issueNumber
        {
            get
            {
                return IssueNumber;
            }
            set
            {
                IssueNumber = value;
            }
        }
    }

}
