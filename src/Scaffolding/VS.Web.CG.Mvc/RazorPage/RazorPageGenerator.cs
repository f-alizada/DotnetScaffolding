// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.DotNet.Scaffolding.Shared;
using Microsoft.VisualStudio.Web.CodeGeneration;
using Microsoft.VisualStudio.Web.CodeGeneration.DotNet;
using Microsoft.VisualStudio.Web.CodeGeneration.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.ComponentModel.Design;
using System.Collections.Generic;
using Microsoft.Extensions.Internal;
using Microsoft.DotNet.MSIdentity.Shared;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Resources;
using System.Linq;

namespace Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Razor
{
    [Alias("razorpage")]
    public class RazorPageGenerator : CommonGeneratorBase, ICodeGenerator
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly IApplicationInfo _applicationInfo;

        public RazorPageGenerator(
            IApplicationInfo applicationInfo,
            IServiceProvider serviceProvider,
            ILogger logger)
            : base(applicationInfo)
        {
            if (applicationInfo == null)
            {
                throw new ArgumentNullException(nameof(applicationInfo));
            }

            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _applicationInfo = applicationInfo ?? throw new ArgumentNullException(nameof(applicationInfo));
        }

        public async Task GenerateCode(RazorPageGeneratorModel razorPageGeneratorModel)
        {
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
                System.Diagnostics.Debugger.Launch();
                _logger.LogMessage("Testing empty dotnet page");
                EmptyDotNetPage(razorPageGeneratorModel);
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

        internal void EmptyDotNetPage(RazorPageGeneratorModel razorPageGeneratorModel)
        {
            var errors = new List<string>();
            var output = new List<string>();
            var args = new List<string>();

            var outputPath = ValidateAndGetOutputPath(razorPageGeneratorModel, outputFileName: razorPageGeneratorModel.RazorPageName + Constants.ViewExtension);
            var outputFolder = Path.GetDirectoryName(outputPath);
            var pageName = Path.GetFileNameWithoutExtension(outputPath);
            var namespaceName = string.IsNullOrEmpty(razorPageGeneratorModel.NamespaceName)
                ? NameSpaceUtilities.GetSafeNameSpaceFromPath(razorPageGeneratorModel.RelativeFolderPath, _applicationInfo.ApplicationName)
                : razorPageGeneratorModel.NamespaceName;
            
            args.Add("page");
            args.Add("--name");
            args.Add(pageName);
            args.Add("--output");
            args.Add(outputFolder);
            args.Add("--force=");
            args.Add(razorPageGeneratorModel.Force.ToString());
            args.Add("--namespace");
            args.Add(namespaceName);
            args.Add("--no-pagemodel=");
            args.Add(razorPageGeneratorModel.NoPageModel.ToString());

            //Create an empty razor page using `dotnet new page`
            var result = Command.CreateDotNet(
                "new",
                args.ToArray())
                .OnErrorLine(e => errors.Add(e))
                .OnOutputLine(o => output.Add(o))
                .Execute();

            if (result.ExitCode != 0)
            {
                _logger.LogMessage("Error creating empty razor page.\n", LogMessageLevel.Error);
                throw new Exception(string.Join(Environment.NewLine, errors));
            }
            else
            {
               _logger.LogMessage($"Successfully created razor page:\n{outputPath}", LogMessageLevel.Information);
            }
            _logger.LogMessage("Doooone");
        }
    }
}
