using System;
using System.Collections.Generic;
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
        [OperationContract]
        [WebGet(UriTemplate = "/RegisterUpgrade?firstName={firstName}&lastName={lastName}&organisation={organisation}" +
                                               "&address1={address1}&address2={address2}&city={city}&state={state}&postcode={postcode}" +
                                               "&country={country}&email={email}&product={product}" +
                                               "&ChangeDBPassword={ChangeDBPassword}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void RegisterUpgrade(string firstName, string lastName, string organisation, string address1, string address2,
                             string city, string state, string postcode, string country, string email, string product,
                             string ChangeDBPassword);

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
