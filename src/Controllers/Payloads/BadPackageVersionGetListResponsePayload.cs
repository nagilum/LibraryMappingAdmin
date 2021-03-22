using System;
using System.Collections.Generic;
using DotNetLibraryAdmin.Database.Tables;

namespace DotNetLibraryAdmin.Controllers.Payloads
{
    public class BadPackageVersionGetListResponsePayload
    {
        public Package Package { get; set; }
        public List<PayloadBadVersion> BadVersions { get; set; }

        #region Structure classes

        public class PayloadBadVersion
        {
            public PackageBadVersion BadVersion { get; set; }
            public List<FileEntry> FileEntries { get; set; }
        }

        #endregion

        #region Helper functions

        /// <summary>
        /// Check if a package has any bad file entries.
        /// </summary>
        /// <param name="package">Package.</param>
        /// <param name="badVersions">List of bad versions.</param>
        /// <param name="fileEntries">List of file entries.</param>
        /// <returns>Entry.</returns>
        public static BadPackageVersionGetListResponsePayload Check(
            Package package,
            List<PackageBadVersion> badVersions,
            List<FileEntry> fileEntries)
        {
            var pe = new BadPackageVersionGetListResponsePayload
            {
                Package = package,
                BadVersions = new List<PayloadBadVersion>()
            };

            foreach (var badVersion in badVersions)
            {
                var bv = new PayloadBadVersion
                {
                    BadVersion = badVersion,
                    FileEntries = GetBadFileEntries(
                        badVersion,
                        fileEntries)
                };

                if (bv.FileEntries.Count == 0)
                {
                    continue;
                }

                pe.BadVersions.Add(bv);
            }

            return pe.BadVersions.Count == 0
                ? null
                : pe;
        }

        /// <summary>
        /// Get a list of file entries that are bad versions.
        /// </summary>
        /// <param name="badVersion">Bad version.</param>
        /// <param name="fileEntries">File entries.</param>
        /// <returns>List.</returns>
        private static List<FileEntry> GetBadFileEntries(
            PackageBadVersion badVersion,
            List<FileEntry> fileEntries)
        {
            var list = new List<FileEntry>();

            foreach (var fileEntry in fileEntries)
            {
                bool? ibv1 = null;
                bool? ibv2 = null;
                bool? ibv3 = null;
                bool? ibv4 = null;

                Version v1;
                Version v2;
                int vc;

                // Check FileVersionFrom.
                if (!string.IsNullOrWhiteSpace(badVersion.FileVersionFrom))
                {
                    ibv1 = true;

                    try
                    {
                        v1 = new Version(fileEntry.FileVersion);
                        v2 = new Version(badVersion.FileVersionFrom);

                        // > 0 = v1 is greater.
                        // < 0 = v1 is lesser.
                        // = 0 = v1 and v2 is equal.
                        vc = v1.CompareTo(v2);

                        // if > 0 = TRUE
                        // if < 0 = FALSE
                        // if = 0 = TRUE

                        if (vc < 0)
                        {
                            ibv1 = false;
                        }
                    }
                    catch
                    {
                        //
                    }
                }

                // Check FileVersionTo.
                if (!string.IsNullOrWhiteSpace(badVersion.FileVersionTo))
                {
                    ibv2 = true;

                    try
                    {
                        v1 = new Version(fileEntry.FileVersion);
                        v2 = new Version(badVersion.FileVersionTo);

                        // > 0 = v1 is greater.
                        // < 0 = v1 is lesser.
                        // = 0 = v1 and v2 is equal.
                        vc = v1.CompareTo(v2);

                        // if > 0 = FALSE
                        // if < 0 = TRUE
                        // if = 0 = TRUE

                        if (vc > 0)
                        {
                            ibv2 = false;
                        }
                    }
                    catch
                    {
                        //
                    }
                }

                // Check ProductVersionFrom.
                if (!string.IsNullOrWhiteSpace(badVersion.ProductVersionFrom))
                {
                    ibv3 = true;

                    try
                    {
                        v1 = new Version(fileEntry.ProductVersion);
                        v2 = new Version(badVersion.ProductVersionFrom);

                        // > 0 = v1 is greater.
                        // < 0 = v1 is lesser.
                        // = 0 = v1 and v2 is equal.
                        vc = v1.CompareTo(v2);

                        // if > 0 = TRUE
                        // if < 0 = FALSE
                        // if = 0 = TRUE

                        if (vc < 0)
                        {
                            ibv3 = false;
                        }
                    }
                    catch
                    {
                        //
                    }
                }

                // Check ProductVersionTo.
                if (!string.IsNullOrWhiteSpace(badVersion.ProductVersionTo))
                {
                    ibv4 = true;

                    try
                    {
                        v1 = new Version(fileEntry.ProductVersion);
                        v2 = new Version(badVersion.ProductVersionTo);

                        // > 0 = v1 is greater.
                        // < 0 = v1 is lesser.
                        // = 0 = v1 and v2 is equal.
                        vc = v1.CompareTo(v2);

                        // if > 0 = FALSE
                        // if < 0 = TRUE
                        // if = 0 = TRUE

                        if (vc > 0)
                        {
                            ibv4 = false;
                        }
                    }
                    catch
                    {
                        //
                    }
                }

                // If either the file or product version is within the limits, return true.
                var retval = false;

                /*
                 * FILE VERSION
                 */

                // From is lesser than v, no upper limit.
                if (ibv1.HasValue &&
                    ibv1.Value &&
                    !ibv2.HasValue)
                {
                    retval = true;
                }

                // From is lesser than v, to is larger.
                else if (ibv1.HasValue &&
                         ibv1.Value &&
                         ibv2.HasValue &&
                         ibv2.Value)
                {
                    retval = true;
                }

                // From has not limit, to is larger than v.
                else if (!ibv1.HasValue &&
                         ibv2.HasValue &&
                         ibv2.Value)
                {
                    retval = true;
                }

                /*
                 * PRODUCT VERSION
                 */

                // From is lesser than v, no upper limit.
                if (ibv3.HasValue &&
                    ibv3.Value &&
                    !ibv4.HasValue)
                {
                    retval = true;
                }

                // From is lesser than v, to is larger.
                else if (ibv3.HasValue &&
                         ibv3.Value &&
                         ibv4.HasValue &&
                         ibv4.Value)
                {
                    retval = true;
                }

                // From has not limit, to is larger than v.
                else if (!ibv3.HasValue &&
                         ibv4.HasValue &&
                         ibv4.Value)
                {
                    retval = true;
                }

                if (!retval)
                {
                    continue;
                }

                list.Add(fileEntry);
            }

            return list;
        }

        #endregion
    }
}