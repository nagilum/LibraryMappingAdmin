using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DotNetLibraryAdmin.Attributes;
using DotNetLibraryAdmin.Controllers.Payloads;
using DotNetLibraryAdmin.Database;
using DotNetLibraryAdmin.Database.Tables;
using Microsoft.EntityFrameworkCore;

namespace DotNetLibraryAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        /// <summary>
        /// Get a list of all packages.
        /// </summary>
        /// <returns>List of packages.</returns>
        [HttpGet]
        [VerifyAuthorization]
        public async Task<ActionResult> GetAll()
        {
            List<Package> list;

            try
            {
                list = await new DatabaseContext()
                    .Packages
                    .Where(n => !n.Deleted.HasValue)
                    .OrderBy(n => n.Name)
                    .ToListAsync();
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
        /// Attach a filename to a package.
        /// </summary>
        /// <param name="id">Id of package.</param>
        /// <param name="payload">Payload containing the filename.</param>
        /// <returns>Success.</returns>
        [HttpPost]
        [Route("{id}/attach")]
        [VerifyAuthorization]
        public async Task<ActionResult> AttachPackage([FromRoute] long id, PackageAttachRequestPayload payload)
        {
            try
            {
                await using var db = new DatabaseContext();

                var package = await db.Packages
                    .FirstOrDefaultAsync(n => !n.Deleted.HasValue &&
                                              n.Id == id);

                if (package == null)
                {
                    throw new Exception($"Package not found: {id}");
                }

                var files = JsonSerializer.Deserialize<List<string>>(package.Files);

                if (!files.Contains(payload.fileName))
                {
                    files.Add(payload.fileName);
                }

                files = files
                    .OrderBy(n => n)
                    .ToList();

                package.Files = JsonSerializer.Serialize(files);
                package.Updated = DateTimeOffset.Now;

                var entries = await db.FileEntries
                    .Where(n => n.FileName == payload.fileName)
                    .ToListAsync();

                foreach (var entry in entries)
                {
                    entry.PackageId = package.Id;
                }

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
        /// Add a new package.
        /// </summary>
        /// <param name="payload">Info about the new package.</param>
        /// <returns>Success.</returns>
        [HttpPost]
        [VerifyAuthorization]
        public async Task<ActionResult> CreateNewPackage(PackageNewRequestPayload payload)
        {
            try
            {
                await using var db = new DatabaseContext();

                var package = await db.Packages
                    .FirstOrDefaultAsync(n => !n.Deleted.HasValue &&
                                              n.Name == payload.Name);

                if (package != null)
                {
                    throw new Exception("A package with that name already exists.");
                }

                package = new Package
                {
                    Created = DateTimeOffset.Now,
                    Updated = DateTimeOffset.Now,
                    Name = payload.Name,
                    Files = JsonSerializer.Serialize(payload.Files),
                    NuGetUrl = payload.NuGetUrl,
                    InfoUrl = payload.InfoUrl,
                    RepoUrl = payload.RepoUrl
                };

                await db.Packages.AddAsync(package);
                await db.SaveChangesAsync();

                var entries = await db.FileEntries
                    .Where(n => !n.PackageId.HasValue &&
                                payload.Files.Contains(n.FileName))
                    .ToListAsync();

                foreach (var entry in entries)
                {
                    entry.PackageId = package.Id;
                }

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
    }
}