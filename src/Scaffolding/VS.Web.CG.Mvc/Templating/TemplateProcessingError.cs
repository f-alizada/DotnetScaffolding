using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templating
{
    /// <summary>
    /// Contains information about errors resulted in running a
    /// visual studio T4 text template transformation.
    /// </summary>
    public class TemplateProcessingError
    {
        /// <summary>
        /// Error message.
        /// </summary>
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// Line number within the T4 template.
        /// </summary>
        public int LineNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Column number within the line.
        /// </summary>
        public int ColumnNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Overriding base implementation.
        /// </summary>
        public override string ToString()
        {
            return String.Format(CultureInfo.CurrentCulture, "ABCD", Message, LineNumber, ColumnNumber);
        }
    }
}
