using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetLibraryAdmin.Database.Tables
{
    [Table("PackageBadVersions")]
    public class PackageBadVersion
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
        public long PackageId { get; set; }

        [Column]
        [MaxLength(32)]
        public string FileVersionFrom { get; set; }

        [Column]
        [MaxLength(32)]
        public string FileVersionTo { get; set; }

        [Column]
        [MaxLength(32)]
        public string ProductVersionFrom { get; set; }

        [Column]
        [MaxLength(32)]
        public string ProductVersionTo { get; set; }

        #endregion
    }
}