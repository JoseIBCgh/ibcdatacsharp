﻿using System.Drawing;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ibcdatacsharp.UI.FileBrowser.Enums;


// Devuelve los iconos de las carpetas
namespace ibcdatacsharp.UI.FileBrowser
{
    public static class FolderManager
    {
        // Devuelve el icono de una carpeta
        public static ImageSource GetImageSource(string directory, ItemState folderType)
        {
            return GetImageSource(directory, new Size(16, 16), folderType);
        }
        // Devuelve el icono de una carpeta de tamaño size
        public static ImageSource GetImageSource(string directory, Size size, ItemState folderType)
        {
            using (var icon = ShellManager.GetIcon(directory, ItemType.Folder, IconSize.Large, folderType))
            {
                return Imaging.CreateBitmapSourceFromHIcon(icon.Handle,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromWidthAndHeight(size.Width, size.Height));
            }
        }
    }
}
