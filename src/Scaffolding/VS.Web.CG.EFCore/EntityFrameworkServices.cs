// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.DotNet.Scaffolding.Shared;
using Microsoft.DotNet.Scaffolding.Shared.Project;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using Microsoft.VisualStudio.Web.CodeGeneration.DotNet;

namespace Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore
{
    public class EntityFrameworkServices : IEntityFrameworkService
    {
        private readonly IDbContextEditorServices _dbContextEditorServices;
        private readonly IApplicationInfo _applicationInfo;
        private readonly ICodeGenAssemblyLoadContext _loader;
        private readonly IModelTypesLocator _modelTypesLocator;
        private readonly IPackageInstaller _packageInstaller;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private const string EFSqlServerPackageName = "Microsoft.EntityFrameworkCore.SqlServer";
        private const string NewDbContextFolderName = "Data";
        private readonly Workspace _workspace;
        private readonly IProjectContext _projectContext;
        private readonly IFileSystem _fileSystem;


        public EntityFrameworkServices(
            IProjectContext projectContext,
            IApplicationInfo applicationInfo,
            ICodeGenAssemblyLoadContext loader,
            IModelTypesLocator modelTypesLocator,
            IDbContextEditorServices dbContextEditorServices,
            IPackageInstaller packageInstaller,
            IServiceProvider serviceProvider,
            Workspace workspace,
            IFileSystem fileSystem,
            ILogger logger)
        {
            _projectContext = projectContext ?? throw new ArgumentNullException(nameof(projectContext));
            _applicationInfo = applicationInfo ?? throw new ArgumentNullException(nameof(applicationInfo));
            _loader = loader ?? throw new ArgumentNullException(nameof(loader));
            _modelTypesLocator = modelTypesLocator ?? throw new ArgumentNullException(nameof(modelTypesLocator));
            _dbContextEditorServices = dbContextEditorServices ?? throw new ArgumentNullException(nameof(dbContextEditorServices));
            _packageInstaller = packageInstaller ?? throw new ArgumentNullException(nameof(packageInstaller));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem)); ;
        }

        public async Task<ContextProcessingResult> GetModelMetadata(string dbContextFullTypeName, ModelType modelTypeSymbol, string areaName, bool useSqlite, bool useT4 = false)
        {
            return await GetModelMetadata(dbContextFullTypeName, modelTypeSymbol, areaName, useSqlite ? DbProvider.SQLite : DbProvider.SqlServer);
        }

        public async Task<ContextProcessingResult> GetModelMetadata(string dbContextFullTypeName, ModelType modelTypeSymbol, string areaName, DbProvider databaseProvider)
        {
            if (string.IsNullOrEmpty(dbContextFullTypeName))
            {
                throw new ArgumentException(nameof(dbContextFullTypeName));
            }

            var processor = new EntityFrameworkModelProcessor(dbContextFullTypeName,
                modelTypeSymbol,
                areaName,
                databaseProvider,
                _loader,
                _dbContextEditorServices,
                _modelTypesLocator,
                _workspace,
                _projectContext,
                _applicationInfo,
                _fileSystem,
                _logger);

            if (useT4)
            {
                await processor.ProcessT4();
            }
            else
            {
                await processor.Process();
            }
            
            return new ContextProcessingResult()
            {
                ContextProcessingStatus = processor.ContextProcessingStatus,
                ModelMetadata = processor.ModelMetadata
            };
        }
    }
}
