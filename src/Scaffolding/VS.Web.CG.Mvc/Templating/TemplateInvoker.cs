using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.VisualStudio.TextTemplating;
using System.Reflection;
using Mono.TextTemplating;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.T4.RazorPages;
using System.CodeDom.Compiler;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.T4;

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

            var host = new TextTemplatingEngineHost(_serviceProvider)
            {
                TemplateFile = templatePath
            };

            var contextTemplate = new RazorPageEmptyGenerator
            {
                Host = host,
                Session = host.CreateSession()
            };

            foreach (var param in templateParameters)
            {
                contextTemplate.Session.Add(param.Key, param.Value);
            }

            string generatedCode = string.Empty;
            if (contextTemplate != null)
            {
                contextTemplate.Initialize();
                generatedCode = ProcessTemplate(contextTemplate);
            }
            return generatedCode;
        }

        private string ProcessTemplate(ITextTransformation transformation)
        {
            var output = transformation.TransformText();

            foreach (CompilerError error in transformation.Errors)
            {
                //_reporter.Write(error);
            }

            if (transformation.Errors.HasErrors)
            {
                //throw new OperationException(DesignStrings.ErrorGeneratingOutput(transformation.GetType().Name));
            }

            return output;
        }

    }
}
