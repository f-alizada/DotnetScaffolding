using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Web.CodeGeneration;

namespace Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Common
{
    internal static class CodeGeneratorHelper
    {
        internal static async Task AddFileHelper(IFileSystem fileSystem, string outputPath, Stream sourceStream)
        {
            fileSystem.CreateDirectory(Path.GetDirectoryName(outputPath));

            if (fileSystem.FileExists(outputPath))
            {
                fileSystem.MakeFileWritable(outputPath);
            }

            await fileSystem.AddFileAsync(outputPath, sourceStream);
        }
    }
}
