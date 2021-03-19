using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetLibraryAdmin.Attributes;
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
    }
}