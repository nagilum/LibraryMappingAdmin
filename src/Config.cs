using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DotNetLibraryAdmin
{
    public class Config
    {
        #region Local storage

        /// <summary>
        /// Internal storage.
        /// </summary>
        private static Dictionary<string, object> Storage { get; set; }

        #endregion

        #region IO functions

        /// <summary>
        /// Load config from disk.
        /// </summary>
        public static void Load()
        {
            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "config.json");

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Unable to find config file: {path}");
            }

            Storage = JsonSerializer.Deserialize<Dictionary<string, object>>(
                File.ReadAllText(path),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }

        #endregion

        #region Getters

        /// <summary>
        /// Get value from config.
        /// </summary>
        /// <param name="keys">Key, with depths, to fetch for.</param>
        /// <returns>Value.</returns>
        public static string Get(params string[] keys)
        {
            if (keys.Length == 0)
            {
                return null;
            }

            var dict = Storage;

            for (var i = 0; i < keys.Length; i++)
            {
                if (!dict.ContainsKey(keys[i]))
                {
                    return null;
                }

                if (i == keys.Length - 1)
                {
                    return dict[keys[i]].ToString();
                }

                try
                {
                    var json = dict[keys[i]].ToString();

                    if (json == null)
                    {
                        throw new NullReferenceException($"Config for key {keys[i]} is empty.");
                    }

                    dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        #endregion
    }
}