using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

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

        /// <summary>
        /// Get a list of all packages that have bad files deployed.
        /// </summary>
        /// <returns>List of packages.</returns>
        public static async Task<List<Package>> GetBadPackagesAsync()
        {
            // TODO
            return new List<Package>();
        }

        #endregion
    }
}