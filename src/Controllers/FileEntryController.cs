using DotNetLibraryAdmin.Attributes;
using DotNetLibraryAdmin.Controllers.Payloads;
using DotNetLibraryAdmin.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetLibraryAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileEntryController : ControllerBase
    {
        /// <summary>
        /// Get a list of all file entries, grouped by servers.
        /// </summary>
        /// <returns>List of file entries.</returns>
        [HttpGet]
        [VerifyAuthorization]
        public async Task<ActionResult> GetAll()
        {
            var res = new FileEntryGetAllResponsePayload
            {
                Servers = new List<FileEntryGetAllResponsePayload.PayloadServer>()
            };

            try
            {
                var list = await new DatabaseContext()
                    .FileEntries
                    .OrderBy(n => n.ServerName)
                    .ThenBy(n => n.FileName)
                    .ToListAsync();

                var serverNames = list
                    .Select(n => n.ServerName)
                    .Distinct()
                    .ToList();

                foreach (var sn in serverNames)
                {
                    // Setup a server entity.
                    var server = new FileEntryGetAllResponsePayload.PayloadServer
                    {
                        ServerName = sn,
                        ServerIps = new List<string>(),
                        FileEntries = list
                            .Where(n => n.ServerName == sn)
                            .ToList()
                    };

                    // Get all IPs.
                    foreach (var se in server.FileEntries.Where(n => n.ServerIps != null))
                    {
                        var ips = se.ServerIps
                            .Split(',')
                            .Select(n => n.Trim())
                            .ToList();

                        foreach (var ip in ips)
                        {
                            if (!server.ServerIps.Contains(ip))
                            {
                                server.ServerIps.Add(ip);
                            }
                        }
                    }

                    // Add to list.
                    res.Servers.Add(server);
                }
            }
            catch (Exception ex)
            {
                return this.BadRequest(new
                {
                    message = ex.Message
                });
            }

            return this.Ok(res);
        }
    }
}