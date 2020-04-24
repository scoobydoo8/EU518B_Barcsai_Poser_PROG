// <copyright file="ShortcutExtension.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.View.Admin
{
    using Shell32;

    /// <summary>
    /// Shortcut extension class.
    /// </summary>
    public static class ShortcutExtension
    {
        /// <summary>
        /// Get shortcut target file.
        /// </summary>
        /// <param name="shortcutPath">Shortcut path.</param>
        /// <returns>Exe path.</returns>
        public static string GetShortcutTargetFile(this string shortcutPath)
        {
            string pathOnly = System.IO.Path.GetDirectoryName(shortcutPath);
            string filenameOnly = System.IO.Path.GetFileName(shortcutPath);

            Shell shell = new Shell();
            Folder folder = shell.NameSpace(pathOnly);
            FolderItem folderItem = folder.ParseName(filenameOnly);
            if (folderItem != null)
            {
                ShellLinkObject link = (ShellLinkObject)folderItem.GetLink;
                return link.Path;
            }

            return string.Empty;
        }
    }
}
