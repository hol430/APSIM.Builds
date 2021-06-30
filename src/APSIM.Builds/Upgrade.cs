using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace APSIM.Builds
{
    /// <summary>
    /// An class encapsulating an APSIM Next Gen upgrade.
    /// </summary>
    public class Upgrade
    {
        /// <summary>
        /// Release date of the upgrade.
        /// </summary>
        public DateTime ReleaseDate { get; private set; }

        /// <summary>
        /// Number/ID of the issue addressed by this upgrade.
        /// </summary>
        public int IssueNumber { get; private set; }

        /// <summary>
        /// Title of the issue addressed by this upgrade.
        /// </summary>
        public string IssueTitle { get; private set; }

        /// <summary>
        /// URL of the issue addressed by this upgrade.
        /// </summary>
        public string IssueUrl { get; private set; }

        /// <summary>
        /// Create an <see cref="Upgrade" /> instance.
        /// </summary>
        /// <param name="date">Release date of the upgrade.</param>
        /// <param name="issue">Number/ID of the issue addressed by this upgrade.</param>
        /// <param name="title">Upgrade title.</param>
        /// <param name="issueUrl">URL of the issue addressed by this upgrade.</param>
        public Upgrade(DateTime date, int issue, string title, string issueUrl)
        {
            ReleaseDate = date;
            IssueNumber = issue;
            IssueTitle = title;
            IssueUrl = issueUrl;
        }

        /// <summary>
        /// URL of the installer for this upgrade.
        /// </summary>
        public string GetURL(Platform platform)
        {
            // fixme
            string ext = GetInstallerFileExtension(platform);
            return $"https://apsimdev.apsim.info/ApsimXFiles/apsim-{IssueNumber}.{ext}";
        }

        /// <summary>
        /// Get the file path extension of the installer for the given platform.
        /// </summary>
        /// <param name="platform">The platform.</param>
        private string GetInstallerFileExtension(Platform platform)
        {
            switch (platform)
            {
                case Platform.Linux:
                    return ".deb";
                case Platform.Windows:
                    return ".exe";
                case Platform.MacOS:
                    return ".dmg";
                default:
                    throw new NotImplementedException($"Unknown platform {platform}");
            }
        }
    }
}
