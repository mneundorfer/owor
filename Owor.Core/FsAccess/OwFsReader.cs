using System.IO.Abstractions;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Owor.Core.Configuration;

namespace Owor.Core.FsAccess
{

    internal sealed class OwFsReader : IOwFsReader
    {
        private readonly ILogger<OwFsReader> _logger;
        private readonly IFileSystem _fileSystem;
        private readonly string _basePath;

        public OwFsReader(ILogger<OwFsReader> logger, IFileSystem fileSystem, IOptions<OwConfig> owConfig)
        {
            _logger = logger;
            _fileSystem = fileSystem;
            _basePath = owConfig.Value.BasePath;
        }

        public string GetFileContent(string deviceId, string fileName)
        {
            var fullPath = _fileSystem.Path.Combine(_basePath, deviceId, fileName);

            _logger.LogDebug($"Trying to read file contents of file {fullPath}");

            var rawContent = _fileSystem.File.ReadAllText(fullPath);
            return rawContent.TrimEnd('\u0000');
        }

        public string[] GetOwDeviceIds()
        {
            _logger.LogDebug($"Trying to read files in directory {_basePath}");

            var filePaths = _fileSystem.Directory.GetFileSystemEntries(@_basePath);

            var fileNames = new List<string>();
            filePaths.ToList().ForEach(fn => fileNames.Add(_fileSystem.FileInfo.FromFileName(fn).Name));

            _logger.LogDebug("Found {0} in directory {1}", fileNames, _basePath);

            return fileNames.ToArray();
        }

    }

}