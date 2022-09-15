// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DotNet.Scaffolding.Shared;
using Microsoft.VisualStudio.Web.CodeGeneration;
using Microsoft.VisualStudio.Web.CodeGeneration.DotNet;
using Microsoft.VisualStudio.Web.CodeGeneration.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templating;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;

namespace Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Razor
{
    [Alias("razorpage")]
    public class RazorPageGenerator : CommonGeneratorBase, ICodeGenerator
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;
        private readonly IProjectContext _projectContext;

        public RazorPageGenerator(
            IApplicationInfo applicationInfo,
            IServiceProvider serviceProvider,
            ILogger logger,
            IFileSystem fileSystem,
            IProjectContext projectContext)
            : base(applicationInfo)
        {
            if (applicationInfo == null)
            {
                throw new ArgumentNullException(nameof(applicationInfo));
            }

            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _projectContext = projectContext ?? throw new ArgumentNullException(nameof(projectContext));
        }

        public async Task GenerateCode(RazorPageGeneratorModel razorPageGeneratorModel)
        {
            if (razorPageGeneratorModel == null)
            {
                throw new ArgumentNullException(nameof(razorPageGeneratorModel));
            }

            if (razorPageGeneratorModel.T4Templating)
            {
                await GenerateCodeT4(razorPageGeneratorModel);
            }
            //older razor templating.
            else
            {
                if (string.IsNullOrEmpty(razorPageGeneratorModel.ModelClass))
                {
                    if (string.IsNullOrEmpty(razorPageGeneratorModel.RazorPageName))
                    {
                        throw new ArgumentException(MessageStrings.RazorPageNameRequired);
                    }

                    if (string.IsNullOrEmpty(razorPageGeneratorModel.TemplateName))
                    {
                        throw new ArgumentException(MessageStrings.TemplateNameRequired);
                    }
                    RazorPageScaffolderBase scaffolder = ActivatorUtilities.CreateInstance<EmptyRazorPageScaffolder>(_serviceProvider);
                    await scaffolder.GenerateCode(razorPageGeneratorModel);
                }
                else
                {
                    EFModelBasedRazorPageScaffolder scaffolder = ActivatorUtilities.CreateInstance<EFModelBasedRazorPageScaffolder>(_serviceProvider);
                    if (!string.IsNullOrEmpty(razorPageGeneratorModel.TemplateName) && !string.IsNullOrEmpty(razorPageGeneratorModel.RazorPageName))
                    {   // Razor page using EF
                        await scaffolder.GenerateCode(razorPageGeneratorModel);
                    }
                    else
                    {   // Razor page CRUD using EF
                        await scaffolder.GenerateViews(razorPageGeneratorModel);
                    }
                }
            }    
        }

        private async Task GenerateCodeT4(RazorPageGeneratorModel razorPageGeneratorModel)
        {
            var razorPageTemplates = T4TemplateHelper.GetAllRazorPagesT4(ApplicationInfo.ApplicationBasePath, _projectContext);
            razorPageGeneratorModel.NamespaceName = string.IsNullOrEmpty(razorPageGeneratorModel.NamespaceName)
               ? NameSpaceUtilities.GetSafeNameSpaceFromPath(razorPageGeneratorModel.RelativeFolderPath, _projectContext.RootNamespace)
               : razorPageGeneratorModel.NamespaceName;

            TemplateInvoker templateInvoker = new TemplateInvoker(_serviceProvider);
            var dictParams = new Dictionary<string, object>()
            {
                { "RazorPageModel" , razorPageGeneratorModel }
            };

            List<string> razorPageTemplatesToExecute = new List<string>();
            //CRUD scenario, do all but the Empty razor page.
            if (string.IsNullOrEmpty(razorPageGeneratorModel.TemplateName))
            {
                var allButEmptyTemplates = T4TemplateHelper.RazorPageTemplates.Where(x => !x.Equals("Empty"));
                foreach (var templateName in allButEmptyTemplates)
                {
                    if (razorPageTemplates.TryGetValue(templateName, out List<string> templateFiles))
                    {
                        razorPageTemplatesToExecute.AddRange(templateFiles);
                    }
                }
            }
            //execute the specified CRUD or Empty razor page.
            else
            {
                if (razorPageTemplates.TryGetValue(razorPageGeneratorModel.TemplateName, out List<string> templateFiles))
                {
                    razorPageTemplatesToExecute.AddRange(templateFiles);
                }
            }

            foreach(var razorPageTemplateToExecute in razorPageTemplatesToExecute)
            {
                var result = templateInvoker.InvokeTemplate(razorPageTemplateToExecute, dictParams);
                string extension = Constants.CodeFileExtension;
                if (razorPageTemplateToExecute.Contains("cshtml", StringComparison.OrdinalIgnoreCase))
                {
                    extension = Constants.ViewExtension;
                }
                var outputPath = ValidateAndGetOutputPath(razorPageGeneratorModel, outputFileName: razorPageGeneratorModel.RazorPageName + extension);
                using (var sourceStream = new MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    await AddFileHelper(_fileSystem, outputPath, sourceStream);
                }
            }
        }
    }
}
