﻿using System;
using System.Linq;
using System.Threading.Tasks;
using SeafClient;
using SeafClient.Types;

namespace SeafileCli
{
    /// <summary>
    /// Contains extension methods for the class <see cref="SeafSession"/>.
    /// </summary>
    public static class SeafSessionExtensions
    {
        /// <summary>
        /// Checks if the given directory exists.
        /// </summary>
        /// <param name="session">The seafile session.</param>
        /// <param name="library">The library, which will searched for the directory.</param>
        /// <param name="path">The directory to check for existence.</param>
        public static async Task<bool> ExistsDirectory(this SeafSession session, SeafLibrary library, string path)
        {
            try
            {
                var dirs = await session.ListDirectory(library, path);
                return dirs != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Creates the directory in the given library. The directory structure with all parents will be created as well.
        /// If the directory already exists, no error will be thrown.
        /// </summary>
        /// <param name="session">The seafile session.</param>
        /// <param name="library">The library, where the directory should be created in.</param>
        /// <param name="path">The directory to create.</param>
        public static async Task CreateDirectoryWithParents(this SeafSession session, SeafLibrary library, string path)
        {
            var currentPath = path;
            for (int i = 0; i <= path.Count(n => n == '/'); i++)
            {
                if (await ExistsDirectory(session, library, currentPath))
                {
                    break;
                }

                if (currentPath.Contains('/'))
                {
                    currentPath = currentPath.Substring(0, currentPath.LastIndexOf('/'));
                }
                else
                {
                    currentPath = string.Empty;
                }
            }

            var dirsToCreate = path.Substring(currentPath.Length)
                .Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
            if (dirsToCreate.Length > 0)
            {
                foreach (string dir in dirsToCreate)
                {
                    currentPath = $"{currentPath}/{dir}";
                    await session.CreateDirectory(library, currentPath);
                }
            }
        }

        /// <summary>
        /// Retrieves a library by name.
        /// </summary>
        /// <param name="session">The seafile session.</param>
        /// <param name="libraryName">The name of the library to get.</param>
        public static async Task<SeafLibrary> GetLibrary(this SeafSession session, string libraryName)
        {
            var libs = await session.ListLibraries();
            return libs.SingleOrDefault(n => n.Name == libraryName);
        }
    }
}