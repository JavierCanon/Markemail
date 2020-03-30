using System;
using System.IO;
using EmailClient.Core.Model;

namespace EmailClient.Core.Service
{
    /// <summary>
    /// Static utility class to handle backup file path, and removing serialized backup files
    /// </summary>
    public static class BackupHandler
    {
        public static string root = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// If exists, it deletes the user's backup file from the Assembile's root directory.
        /// </summary>
        /// <param name="user">The user to delete</param>
        public static void DeleteBackup(User user)
        {
            var file = new FileInfo(Path.Combine(root, user.Name + ".dat"));
            if (file.Exists) file.Delete();
        }
    }
}
