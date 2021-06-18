using DotNetLibraryAdmin.Attributes;
using DotNetLibraryAdmin.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetLibraryAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        /// <summary>
        /// Get stats.
        /// </summary>
        /// <returns>Stats.</returns>
        [HttpGet]
        [VerifyAuthorization]
        public async Task<ActionResult> GetStats()
        {
            await using var db = new DatabaseContext();

            var dict = new Dictionary<string, int>
            {
                {
                    "fileEntries",
                    await db.FileEntries.CountAsync()
                },
                {
                    "fileEntriesWithoutPackage",
                    await db.FileEntries.CountAsync(n => !n.PackageId.HasValue)
                },
                {
                    "packages",
                    await db.Packages.CountAsync()
                }
            };

            // Output.
            return this.Ok(dict);
        }
    }
}