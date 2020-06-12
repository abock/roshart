using System;
using System.IO;

using Microsoft.Extensions.FileProviders;

namespace Roshart.Services
{
    readonly struct Shart : IFileInfo
    {
        public string Name { get; }
        public string PhysicalPath { get; }
        public long Length { get; }
        public DateTimeOffset Created { get; }
        public DateTimeOffset LastModified { get; }
        
        bool IFileInfo.Exists => true;
        bool IFileInfo.IsDirectory => false;

        public Stream CreateReadStream()
            => File.OpenRead(PhysicalPath);

        public Shart (
            string name,
            string physicalPath,
            long length,
            DateTimeOffset created,
            DateTimeOffset lastModified)
        {
            Name = name;
            PhysicalPath = physicalPath;
            Length = length;
            Created = created;
            LastModified = lastModified;
        }

        public static Shart FromFileSystem(
            string filePath,
            string? slug = null)
        {
            var file = new FileInfo(filePath);
            if (!file.Exists)
                throw new FileNotFoundException(filePath);
               
            return new Shart(
                slug ?? Path.GetFileNameWithoutExtension(file.FullName),
                Path.GetFullPath(file.FullName),
                file.Length,
                file.CreationTimeUtc,
                file.LastWriteTimeUtc);
        }
    }
}
