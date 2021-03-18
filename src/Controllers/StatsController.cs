using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetLibraryAdmin.Attributes;
using DotNetLibraryAdmin.Database;
using DotNetLibraryAdmin.Database.Tables;
using Microsoft.EntityFrameworkCore;

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
                },
                {
                    "badPackages",
                    (await Package.GetBadPackagesAsync()).Count
                }
            };

            // Output.
            return this.Ok(dict);
        }
    }
}