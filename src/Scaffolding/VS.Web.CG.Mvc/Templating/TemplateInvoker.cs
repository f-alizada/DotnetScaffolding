using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.VisualStudio.TextTemplating;
using System.Reflection;
using Mono.TextTemplating;

namespace Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templating
{
    /// <summary>
    /// Contains useful helper functions for running visual studio text transformation.
    /// For internal microsoft use only. Use <see cref="ITemplateInvoker"/>
    /// in custom code generators.
    /// </summary>
    internal class TemplateInvoker : ITemplateInvoker
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="serviceProvider"></param>
        public TemplateInvoker(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Executes a code generator template to generate the code.
        /// </summary>
        /// <param name="templatePath">Full path of the template file.</param>
        /// <param name="templateParameters">Parameters for the template.
        /// These parameters can be accessed in text template using a parameter directive.
        /// The values passed in must be either serializable or 
        /// extend <see cref="System.MarshalByRefObject"/> type.</param>
        /// <returns>Generated code if there were no processing errors. Throws 
        /// <see cref="System.InvalidOperationException" /> otherwise.
        /// </returns>
        public string InvokeTemplate(string templatePath,
            IDictionary<string, object> templateParameters)
        {
            if (string.IsNullOrEmpty(templatePath))
            {
                //ExceptionUtil.ThrowStringEmptyArgumentException(nameof(templatePath));
            }

            if (templateParameters == null)
            {
                throw new ArgumentNullException(nameof(templateParameters));
            }

            /*            TemplateProcessingResult result = new TemplateProcessingResult();

                        TextTemplatingEngineHost engineHost = new TextTemplatingEngineHost(_serviceProvider);
                        foreach (KeyValuePair<string, object> entry in templateParameters)
                        {
                            if (entry.Value == null)
                            {
                                throw new InvalidOperationException(
                                    string.Format(CultureInfo.CurrentCulture,
                                        "ABCDE",
                                        entry.Key,
                                        templatePath));
                            }

                            engineHost.Session.Add(entry.Key, entry.Value);
                        }

                        var vsTextTemplating = new TemplatingEngine();
                        result.GeneratedText = vsTextTemplating.ProcessTemplate(
                            File.ReadAllText(templatePath),
                            engineHost);

                        return result;*/
            var host = new TextTemplatingEngineHost(_serviceProvider)
            {
                Session =
                {
                    { "RazorPageClassName" , "ClassName" },
                    { "Namespace", "TestNamespace" }
                }
            };

            //var contextTemplate = Path.Combine(options.ProjectDir!, TemplatesDirectory, DbContextTemplate);
            var contextTemplate = @"D:\Stuff\scaffolding\src\Scaffolding\VS.Web.CG.Mvc\Templates\T4\RazorPages\RazorPage_Empty.tt";

            string generatedCode = string.Empty;
            if (File.Exists(contextTemplate))
            {
                host.TemplateFile = contextTemplate;
                host.Initialize();
                host.Session.Add("Namespace", "TestNamespace");
                host.Session.Add("RazorPageClassName", "TestClass");
                CompiledTemplate compiledEntityTypeTemplate = null;
                //generatedCode = new TemplatingEngine().ProcessTemplate(File.ReadAllText(contextTemplate), host);
                //CheckEncoding(host.OutputEncoding);
                //HandleErrors(host);
                compiledEntityTypeTemplate = new TemplatingEngine().CompileTemplate(File.ReadAllText(contextTemplate), host);
                generatedCode = compiledEntityTypeTemplate.Process();
            }
            return generatedCode;
        }

    }
}
