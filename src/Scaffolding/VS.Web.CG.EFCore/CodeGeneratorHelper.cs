using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore
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
