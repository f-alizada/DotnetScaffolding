
namespace Microsoft.DotNet.Scaffolding.Shared
{
    public class ScaffoldingTemplatingModel
    {
        /// <summary>
        ///     Gets or sets the namespace of the project.
        /// </summary>
        /// <value>The namespace of the project.</value>
        public virtual string RootNamespace { get; set; }

        /// <summary>
        ///     Gets or sets the namespace for model classes.
        /// </summary>
        /// <value> The namespace for model classes. </value>
        public virtual string ModelNamespace { get; set; }

        /// <summary>
        ///     Gets or sets the namespace for current class
        /// </summary>
        /// <value> The namespace for current class. </value>
        public virtual string ClassNamespace { get; set; }

        /// <summary>
        ///     Gets or sets the namespace for context class.
        /// </summary>
        /// <value>The namespace for context class.</value>
        public virtual string ContextNamespace { get; set; }

        /// <summary>
        /// Gets or sets the root project directory.
        /// </summary>
        /// <value>The directory.</value>
        public virtual string ProjectDir { get; set; }

        /// <summary>
        /// Gets or sets the class name property.
        /// </summary>
        public virtual string ClassName { get; set; }
    }
}
