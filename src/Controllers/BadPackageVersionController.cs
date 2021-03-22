using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetLibraryAdmin.Attributes;
using DotNetLibraryAdmin.Database;
using DotNetLibraryAdmin.Database.Tables;
using DotNetLibraryAdmin.Controllers.Payloads;
using Microsoft.EntityFrameworkCore;

namespace DotNetLibraryAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BadPackageVersionController : ControllerBase
    {
        /// <summary>
        /// Get a list of all bad package versions.
        /// </summary>
        /// <param name="packageId">Package to filter by.</param>
        /// <returns>List of bad package versions.</returns>
        [HttpGet]
        [VerifyAuthorization]
        public async Task<ActionResult> GetAll([FromQuery] long? packageId)
        {
            List<PackageBadVersion> list;

            try
            {
                var query = new DatabaseContext()
                    .PackageBadVersions
                    .Where(n => !n.Deleted.HasValue)
                    .AsQueryable();

                if (packageId.HasValue)
                {
                    query = query.Where(n => n.PackageId == packageId);
                }

                list = await query.ToListAsync();
            }
            catch (Exception ex)
            {
                return this.BadRequest(new
                {
                    message = ex.Message
                });
            }

            return this.Ok(list);
        }

        /// <summary>
        /// Create a new bad version entry.
        /// </summary>
        /// <param name="payload">Info about the new bad version.</param>
        /// <returns>Success.</returns>
        [HttpPost]
        [VerifyAuthorization]
        public async Task<ActionResult> CreateNew(BadPackageVersionCreateNewRequestPayload payload)
        {
            try
            {
                await using var db = new DatabaseContext();

                var entry = new PackageBadVersion
                {
                    Created = DateTimeOffset.Now,
                    Updated = DateTimeOffset.Now,
                    PackageId = payload.PackageId,
                    FileVersionFrom = payload.FileVersionFrom,
                    FileVersionTo = payload.FileVersionTo,
                    ProductVersionFrom = payload.ProductVersionFrom,
                    ProductVersionTo = payload.ProductVersionTo
                };

                await db.PackageBadVersions.AddAsync(entry);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return this.BadRequest(new
                {
                    message = ex.Message
                });
            }

            return this.Ok(new { });
        }

        /// <summary>
        /// Mark an entry as deleted.
        /// </summary>
        /// <param name="id">Id of entry.</param>
        /// <returns>Success.</returns>
        [HttpDelete]
        [Route("{id}")]
        [VerifyAuthorization]
        public async Task<ActionResult> Delete([FromRoute] long id)
        {
            try
            {
                await using var db = new DatabaseContext();

                var entry = await db.PackageBadVersions
                    .FirstOrDefaultAsync(n => n.Id == id);

                if (entry == null)
                {
                    throw new Exception($"Entry not found: {id}");
                }

                entry.Updated = DateTimeOffset.Now;
                entry.Deleted = DateTimeOffset.Now;

                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return this.BadRequest(new
                {
                    message = ex.Message
                });
            }

            return this.Ok(new { });
        }

        /// <summary>
        /// Get a list of all active bad packages and their metadata.
        /// </summary>
        /// <returns>A list.</returns>
        [HttpGet]
        [Route("list")]
        [VerifyAuthorization]
        public async Task<ActionResult> GetList()
        {
            await using var db = new DatabaseContext();

            var packages = await db.Packages
                .Where(n => !n.Deleted.HasValue)
                .OrderBy(n => n.Name)
                .ToListAsync();

            var badVersions = await db.PackageBadVersions
                .Where(n => !n.Deleted.HasValue)
                .OrderBy(n => n.FileVersionFrom)
                .ThenBy(n => n.FileVersionTo)
                .ThenBy(n => n.ProductVersionFrom)
                .ThenBy(n => n.ProductVersionTo)
                .ToListAsync();

            var fileEntries = await db.FileEntries
                .Where(n => n.PackageId.HasValue)
                .OrderBy(n => n.FileName)
                .ToListAsync();

            var list = packages
                .Select(n => BadPackageVersionGetListResponsePayload.Check(
                    n,
                    badVersions.Where(m => m.PackageId == n.Id).ToList(),
                    fileEntries.Where(m => m.PackageId == n.Id).ToList()))
                .Where(n => n != null)
                .ToList();

            return this.Ok(list);
        }
    }
}