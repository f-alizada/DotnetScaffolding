using System;

namespace Microsoft.DotNet.Scaffolding.Helpers.Services;

public interface IEnvironmentService
{
    string UserProfilePath { get; }
    string NugetCachePath { get; }
    string DotnetUserProfilePath { get; }
    string CurrentDirectory { get; }
    bool Is64BitOperatingSystem { get; }
    bool Is64BitProcess { get; }
    string? DomainName { get; }
    OperatingSystem OS { get; }
    string GetMachineName();
    string? GetEnvironmentVariable(string name);
    void SetEnvironmentVariable(string name, string value, EnvironmentVariableTarget envTarget = EnvironmentVariableTarget.Process);
    string GetFolderPath(Environment.SpecialFolder specifalFolder);
    string ExpandEnvironmentVariables(string name);
}
