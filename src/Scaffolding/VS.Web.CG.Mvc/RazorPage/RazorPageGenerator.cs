// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DotNet.Scaffolding.Shared;
using Microsoft.VisualStudio.Web.CodeGeneration;
using Microsoft.VisualStudio.Web.CodeGeneration.DotNet;
using Microsoft.VisualStudio.Web.CodeGeneration.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templating;
using Microsoft.VisualStudio.Web.CodeGeneration.Templating;

namespace Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Razor
{
    [Alias("razorpage")]
    public class RazorPageGenerator : CommonGeneratorBase, ICodeGenerator
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;

        public RazorPageGenerator(
            IApplicationInfo applicationInfo,
            IServiceProvider serviceProvider,
            ILogger logger,
            IFileSystem fileSystem)
            : base(applicationInfo)
        {
            if (applicationInfo == null)
            {
                throw new ArgumentNullException(nameof(applicationInfo));
            }

            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public async Task GenerateCode(RazorPageGeneratorModel razorPageGeneratorModel)
        {
            System.Diagnostics.Debugger.Launch();
            await Task.Delay(0);
            if (razorPageGeneratorModel == null)
            {
                throw new ArgumentNullException(nameof(razorPageGeneratorModel));
            }

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
                TemplateInvoker ti = new TemplateInvoker(_serviceProvider);
                //var templateModel = GetRazorPageViewGeneratorTemplateModel(razorPageGeneratorModel);
                var dictParams = new Dictionary<string, object>()
                {
                    { "RazorPageClassName" , "ClassName" },
                    { "Namespace", "TestNamespace" }
                };
                var result = ti.InvokeTemplate(@"D:\Stuff\scaffolding\src\Scaffolding\VS.Web.CG.Mvc\Templates\T4\RazorPages\RazorPage_Empty.tt", dictParams);
                var outputPath = ValidateAndGetOutputPath(razorPageGeneratorModel, outputFileName: razorPageGeneratorModel.RazorPageName + Constants.CodeFileExtension);
                //var text = result.GeneratedText;
                using (var sourceStream = new MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    await AddFileHelper(outputPath, sourceStream);
                }
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

        private async Task AddFileHelper(string outputPath, Stream sourceStream)
        {
            _fileSystem.CreateDirectory(Path.GetDirectoryName(outputPath));

            if (_fileSystem.FileExists(outputPath))
            {
                _fileSystem.MakeFileWritable(outputPath);
            }

            await _fileSystem.AddFileAsync(outputPath, sourceStream);
        }
    }
}
