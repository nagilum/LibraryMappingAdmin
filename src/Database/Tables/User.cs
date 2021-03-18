using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetLibraryAdmin.Database.Tables
{
    [Table("Users")]
    public class User
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
        [MaxLength(64)]
        public string Username { get; set; }

        [Column]
        [MaxLength(1024)]
        public string Password { get; set; }

        #endregion

        #region Instance functions

        /// <summary>
        /// Get the token associated with this user.
        /// </summary>
        /// <returns>Token.</returns>
        public string GetToken()
        {
            return Tools.CreateHash(
                $"{this.Id}" +
                $"{this.Created:yyyy-MM-dd HH:mm:ss}" +
                $"{this.Username}" +
                $"{this.Password}");
        }

        #endregion
    }
}