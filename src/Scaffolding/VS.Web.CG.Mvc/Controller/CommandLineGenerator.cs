// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Web.CodeGeneration;
using Microsoft.VisualStudio.Web.CodeGeneration.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Razor;

namespace Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Controller
{
    [Alias("controller")]
    public class CommandLineGenerator : ICodeGenerator
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandLineGenerator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task GenerateCode(CommandLineGeneratorModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.T4Templating)
            {
                await GenerateCodeT4(model);
            }
            //older razor templating
            else
            {
                ControllerGeneratorBase generator;
                if (string.IsNullOrEmpty(model.ModelClass))
                {
                    if (model.GenerateReadWriteActions)
                    {
                        generator = GetGenerator<MvcControllerWithReadWriteActionGenerator>();
                    }
                    else
                    {
                        generator = GetGenerator<MvcControllerEmpty>(); //This need to handle the WebAPI Empty as well...
                    }
                }
                else
                {
                    generator = GetGenerator<ControllerWithContextGenerator>();
                }

                if (generator != null)
                {
                    await generator.Generate(model);
                }
            }
        }

        private Task GenerateCodeT4(CommandLineGeneratorModel model)
        {
            throw new NotImplementedException(string.Format(MessageStrings.T4TemplatingNotSupported, nameof(MvcController)));
        }

        private ControllerGeneratorBase GetGenerator<TChild>() where TChild : ControllerGeneratorBase
        {
            return (ControllerGeneratorBase)ActivatorUtilities.CreateInstance<TChild>(_serviceProvider);
        }
    }
}
