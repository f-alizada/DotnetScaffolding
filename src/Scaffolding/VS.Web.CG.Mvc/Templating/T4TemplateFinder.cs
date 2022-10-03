using Microsoft.VisualStudio.Web.CodeGeneration;
using System.Collections.Generic;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templating
{
    internal static class T4TemplateFinder
    {
        public static IEnumerable<string> GetTemplateFoldersT4(string appBasePath, string baseFolder, IProjectContext projectContext)
        {
            return TemplateFoldersUtilities.GetTemplateFolders(
                containingProject: Constants.ThisAssemblyName,
                applicationBasePath: appBasePath,
                baseFolders: new[] { baseFolder },
                projectContext: projectContext);
        }

        public static IDictionary<string, List<string>> GetAllRazorPagesT4(string appBasePath, IProjectContext projectContext)
        {
            var razorPages = new Dictionary<string, List<string>>();
            var razorPageTemplatesFolder = GetTemplateFoldersT4(appBasePath, Path.Combine("T4", "RazorPages"), projectContext)?.FirstOrDefault();
            if (Directory.Exists(razorPageTemplatesFolder))
            {
                var templateFiles = Directory.EnumerateFiles(razorPageTemplatesFolder);
                foreach (var razorPageTemplateType in RazorPageTemplates)
                {
                    razorPages.Add(razorPageTemplateType, templateFiles.Where(x => x.Contains(razorPageTemplateType, System.StringComparison.OrdinalIgnoreCase)).ToList());
                }
            }
            return razorPages;
        }

        public static IList<string> RazorPageTemplates = new List<string>()
        {
            "Empty", "Create","List", "Details", "Delete", "Edit" 
        };
    }
}
