using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace APSIM.Builds.Portal
{
    public partial class Validation : System.Web.UI.Page
    {
        private string validationFolder;

        /// <summary>
        /// Page is loading - populate controls.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            validationFolder = Path.Combine(Request.PhysicalApplicationPath, "ValidationGraphs");
            if (versionList.Items.Count == 0)
            {
                PopulateVersionList();
                PopulateModuleList();
                PopulateLiteral();
            }
        }

        /// <summary>User has selected a version.</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnVersionListChanged(object sender, EventArgs e)
        {
            PopulateModuleList();
        }

        /// <summary>User has selected a module.</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnModuleListChanged(object sender, EventArgs e)
        {
            PopulateLiteral();
        }

        /// <summary>Get a list of all modules for a version.</summary>
        private void PopulateModuleList()
        {
            moduleList.Items.Clear();
            foreach (string moduleName in Directory.GetDirectories(Path.Combine(validationFolder, versionList.Text)))
                moduleList.Items.Add(Path.GetFileName(moduleName));
        }

        /// <summary>Get a list of APSIM versions.</summary>
        private void PopulateVersionList()
        {
            var folders = Directory.GetDirectories(validationFolder).ToList();
            versionList.Items.Clear();
            foreach (string version in folders.Select(p => Path.GetFileName(p)))
                versionList.Items.Add(version);
        }

        /// <summary>Populate the HTML literal control at the bottom of the page.</summary>
        private void PopulateLiteral()
        {
            literal.Text = GenerateHTML();
        }

        /// <summary>
        /// Generate HTML for showing validation for a module.
        /// </summary>
        private string GenerateHTML()
        {
            string html = string.Empty;

            string moduleFolder = Path.Combine(validationFolder, versionList.Text, moduleList.Text);

            string baseURL = Bob.GetBaseURL(Request.Url);

            foreach (string imageFileName in Directory.GetFiles(moduleFolder, "*.jpg", SearchOption.AllDirectories))
            {
                string title = imageFileName.Replace(validationFolder + "\\", "").Replace("\\", "/");
                html += "<h2>" + title.Replace(".jpg", "") + "</h2>";
                string imageURL = baseURL + "/ValidationGraphs/" + title;
                html += "<img src=\"" + imageURL + "\"/>";
            }

            return html;
        }


    }
}