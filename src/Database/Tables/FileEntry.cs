using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetLibraryAdmin.Database.Tables
{
    [Table("FileEntries")]
    public class FileEntry
    {
        #region ORM

        [Key]
        [Column]
        public long Id { get; set; }

        [Column]
        public DateTimeOffset Created { get; set; }

        [Column]
        public DateTimeOffset LastScan { get; set; }

        [Column]
        public long? PackageId { get; set; }

        [Column]
        [MaxLength(128)]
        public string ServerName { get; set; }

        [Column]
        public string ServerIps { get; set; }

        [Column]
        [MaxLength(1024)]
        public string FilePath { get; set; }

        [Column]
        [MaxLength(64)]
        public string FileName { get; set; }

        [Column]
        public long FileSize { get; set; }

        [Column]
        [MaxLength(32)]
        public string FileVersion { get; set; }

        [Column]
        public int FileVersionMajor { get; set; }

        [Column]
        public int FileVersionMinor { get; set; }

        [Column]
        public int FileVersionBuild { get; set; }

        [Column]
        public int FileVersionPrivate { get; set; }

        [Column]
        [MaxLength(32)]
        public string ProductVersion { get; set; }

        [Column]
        public int ProductVersionMajor { get; set; }

        [Column]
        public int ProductVersionMinor { get; set; }

        [Column]
        public int ProductVersionBuild { get; set; }

        [Column]
        public int ProductVersionPrivate { get; set; }

        #endregion
    }
}
