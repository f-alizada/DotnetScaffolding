// Copyright (c) Microsoft Corporation. All rights reserved.

using System;
using System.Linq;
using System.Reflection;
using Microsoft.DotNet.Scaffolding.Helpers.Services;
using Microsoft.DotNet.Tools.Scaffold.AppBuilder;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Microsoft.DotNet.Tools.Scaffold;

public class ScaffoldCommandAppBuilder(string[] args)
{
    private readonly string[] _args = args;
    private readonly string _backupDotNetScaffoldVersion = "0.1.0-dev";

    public ScaffoldCommandApp Build()
    {
        var serviceRegistrations = GetDefaultServices();
        var commandApp = new CommandApp(serviceRegistrations);
        commandApp.Configure(config =>
        {
            config
                .SetApplicationName("dotnet-scaffold")
                .SetApplicationVersion(GetToolVersion())
                .AddCommand<ScaffoldCommand>("scaffold")
                    .WithDescription("blah blah scaffold description")
                    .WithExample(["scaffold", "<PROJECT_PATH>"]);
        });

        return new ScaffoldCommandApp(commandApp, _args);
    }

    private ITypeRegistrar? GetDefaultServices()
    {
        var registrar = new TypeRegistrar();
        registrar.Register(typeof(IFileSystem), typeof(FileSystem));
        registrar.Register(typeof(IEnvironmentService), typeof(EnvironmentService));
        registrar.Register(typeof(ILogger), typeof(ConsoleLogger));
        return registrar;
    }

    private string GetToolVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var assemblyAttr = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        return assemblyAttr?.InformationalVersion ?? _backupDotNetScaffoldVersion;
    }
}
