using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using DotNetLibraryAdmin.Controllers.Payloads;
using Microsoft.EntityFrameworkCore;

namespace DotNetLibraryAdmin.Database.Tables
{
    [Table("Packages")]
    public class Package
    {
        #region ORM

        [Key]
        [Column]
        public long Id { get; set; }

        [Column]
        public DateTimeOffset Created { get; set; }

        [Column]
        public DateTimeOffset Updated { get; set; }

        [Column]
        public DateTimeOffset? Deleted { get; set; }

        [Column]
        [MaxLength(128)]
        public string Name { get; set; }

        [Column]
        public string Files { get; set; }

        [Column]
        public string NuGetUrl { get; set; }

        [Column]
        public string InfoUrl { get; set; }

        [Column]
        public string RepoUrl { get; set; }

        #endregion

        #region Helper functions

        /*
        /// <summary>
        /// Get a list of all packages that have bad files deployed.
        /// </summary>
        /// <returns>List of packages.</returns>
        public static async Task<List<StatsGetListOfBadPackagesResponsePayload>> GetBadPackagesAsync()
        {
            var list = new List<StatsGetListOfBadPackagesResponsePayload>();

            await using var db = new DatabaseContext();

            var packages = await db.Packages
                .Where(n => !n.Deleted.HasValue)
                .OrderBy(n => n.Name)
                .ToListAsync();

            var badVersions = await db.PackageBadVersions
                .Where(n => !n.Deleted.HasValue)
                .ToListAsync();

            var fileEntries = await db.FileEntries
                .Where(n => n.PackageId.HasValue)
                .ToListAsync();

            foreach (var package in packages)
            {
                var entry = CheckForBadVersions(
                    package,
                    badVersions.Where(n => n.PackageId == package.Id),
                    fileEntries.Where(n => n.PackageId == package.Id));

                if (entry == null)
                {
                    continue;
                }

                list.Add(entry);
            }

            return list;
        }

        private static StatsGetListOfBadPackagesResponsePayload CheckForBadVersions(
            Package package,
            IEnumerable<PackageBadVersion> badVersions,
            IEnumerable<FileEntry> fileEntries)
        {
            var pe = new StatsGetListOfBadPackagesResponsePayload
            {
                PackageId = package.Id,
                PackageName = package.Name
            };

            return pe;
        }
        */

        #endregion
    }
}