using System.Collections.Generic;

namespace Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templating
{
    /// <summary>
    /// Services for invoking T4 text templates for code generation.
    /// </summary>
    public interface ITemplateInvoker
    {
        /// <summary>
        /// Invokes a T4 text template and returns the result.
        /// </summary>
        /// <param name="templatePath">Full path of T4 text template file.</param>
        /// <param name="templateParameters">Parameters for template execution.
        /// These parameters can be accessed in text template using a parameter directive.
        /// The values passed in must be either serializable or 
        /// extend <see cref="System.MarshalByRefObject"/> type.</param>
        /// <returns>Generated code if there were no processing errors. Throws 
        /// <see cref="System.InvalidOperationException" /> otherwise.
        /// </returns>
        string InvokeTemplate(
            string templatePath,
            IDictionary<string, object> templateParameters);
    }
}
