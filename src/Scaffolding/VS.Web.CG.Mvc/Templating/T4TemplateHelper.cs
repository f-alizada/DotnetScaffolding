using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using Microsoft.VisualStudio.Web.CodeGeneration;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.T4;

namespace Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templating
{
    internal static class  T4TemplateHelper
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
                var templateFiles = Directory.EnumerateFiles(razorPageTemplatesFolder, "*.tt", SearchOption.AllDirectories);
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

        public static ITextTransformation CreateT4Generator(IServiceProvider serviceProvider, string templatePath)
        {
            if (string.IsNullOrEmpty(templatePath))
            {
                return null;
            }
            var host = new TextTemplatingEngineHost(serviceProvider)
            {
                TemplateFile = templatePath
            };
            ITextTransformation contextTemplate = null;

            string templateName = Path.GetFileNameWithoutExtension(templatePath);
            if (string.IsNullOrEmpty(templateName))
            {
                return null;
            }

            if (templateName.StartsWith("razorpage", StringComparison.OrdinalIgnoreCase))
            {
                contextTemplate = CreateT4RazorPageTemplate(host, templateName);
            }
            
            contextTemplate.Session = host.CreateSession();

            return contextTemplate;
        }

        private static ITextTransformation CreateT4RazorPageTemplate(TextTemplatingEngineHost host, string templateName)
        {
            ITextTransformation contextTemplate = null;
            if (templateName.Equals(nameof(RazorPageEmptyCshtmlGenerator), StringComparison.OrdinalIgnoreCase))
            {
                contextTemplate = new RazorPageEmptyCshtmlGenerator { Host = host };
            }
            else if (templateName.Equals(nameof(RazorPageEmptyGenerator), StringComparison.OrdinalIgnoreCase))
            {
                contextTemplate = new RazorPageEmptyGenerator { Host = host };
            }
            //Create.cshtml
            else if (templateName.Equals(nameof(RazorPageCreateCshtmlGenerator), StringComparison.OrdinalIgnoreCase))
            {
                contextTemplate = new RazorPageCreateCshtmlGenerator { Host = host };
            }
            //Create.cs
            else if (templateName.Equals(nameof(RazorPageCreateGenerator), StringComparison.OrdinalIgnoreCase))
            {
                contextTemplate = new RazorPageCreateGenerator { Host = host };
            }
            //Delete.cshtml
            else if (templateName.Equals(nameof(RazorPageCreateCshtmlGenerator), StringComparison.OrdinalIgnoreCase))
            {
                contextTemplate = new RazorPageCreateCshtmlGenerator { Host = host };
            }
            //Delete.cs
            else if (templateName.Equals(nameof(RazorPageCreateCshtmlGenerator), StringComparison.OrdinalIgnoreCase))
            {
                contextTemplate = new RazorPageCreateCshtmlGenerator { Host = host };
            }
            //Details.cshtml
            else if (templateName.Equals(nameof(RazorPageCreateCshtmlGenerator), StringComparison.OrdinalIgnoreCase))
            {
                contextTemplate = new RazorPageCreateCshtmlGenerator { Host = host };
            }
            //Details.cs
            else if (templateName.Equals(nameof(RazorPageCreateCshtmlGenerator), StringComparison.OrdinalIgnoreCase))
            {
                contextTemplate = new RazorPageCreateCshtmlGenerator { Host = host };
            }
            //Edit.cshtml
            else if (templateName.Equals(nameof(RazorPageCreateCshtmlGenerator), StringComparison.OrdinalIgnoreCase))
            {
                contextTemplate = new RazorPageCreateCshtmlGenerator { Host = host };
            }
            //Edit.cs
            else if (templateName.Equals(nameof(RazorPageCreateCshtmlGenerator), StringComparison.OrdinalIgnoreCase))
            {
                contextTemplate = new RazorPageCreateCshtmlGenerator { Host = host };
            }
            //List.cs
            else if (templateName.Equals(nameof(RazorPageCreateCshtmlGenerator), StringComparison.OrdinalIgnoreCase))
            {
                contextTemplate = new RazorPageCreateCshtmlGenerator { Host = host };
            }
            //List.cshtml
            else if (templateName.Equals(nameof(RazorPageCreateCshtmlGenerator), StringComparison.OrdinalIgnoreCase))
            {
                contextTemplate = new RazorPageCreateCshtmlGenerator { Host = host };
            }

            return contextTemplate;
        }
    }
}
