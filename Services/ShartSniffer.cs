using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Roshart.Services
{
    sealed class ShartSnifferOptions
    {
        public const string SectionName = nameof(ShartSniffer);

        public string? RootDirectory { get; set; }
    }

    sealed class ShartSniffer
    {
        readonly Dictionary<string, ShartContext> shartContexts
            = new Dictionary<string, ShartContext>();

        public ShartSniffer(
            IOptions<ShartSnifferOptions> options,
            ILogger<ShartSniffer> logger)
        {
            var rootDirectory = options.Value.RootDirectory
                ?? throw new ArgumentException(
                    $"{ShartSnifferOptions.SectionName}.{nameof(ShartSnifferOptions.RootDirectory)} " +
                    "must be set in appsettings.json");

            logger.LogInformation("Root directory: {rootDirectory}", rootDirectory);

            foreach (var vhostDirectory in Directory.EnumerateDirectories(rootDirectory))
            {
                var vhostName = Path.GetFileName(vhostDirectory)?.ToLowerInvariant();
                if (vhostName is null)
                    continue;

                logger.LogInformation(
                    "Sniffing out sharts in {vhostDirectory}", vhostDirectory);

                var vhostSharts = Directory.EnumerateFiles(vhostDirectory, "*.gif")
                    .Select(filePath => Shart.FromFileSystem(filePath))
                    .OrderByDescending(shart => shart.Created)
                    .ToShartCollection();

                shartContexts.Add(vhostName, new ShartContext(
                    vhostName,
                    vhostSharts));

                logger.LogInformation(
                    "Found {shartCount} sharts in {vhostDirectory}",
                    vhostSharts.Count,
                    vhostDirectory);
            }
        }

        public bool TryGetShartContext(string vhostName, out ShartContext shartContext)
        {
            if (string.IsNullOrEmpty(vhostName) ||
                !this.shartContexts.TryGetValue(
                    vhostName.ToLowerInvariant(), 
                    out var shartsList))
            {
                #nullable disable
                shartContext = null;
                #nullable restore
                return false;
            }

            shartContext = shartsList;
            return true;
        }
    }
}
