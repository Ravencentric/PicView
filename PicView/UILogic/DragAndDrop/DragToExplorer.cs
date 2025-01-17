﻿using PicView.ChangeTitlebar;
using PicView.FileHandling;
using PicView.PicGallery;
using PicView.Properties;
using PicView.UILogic.TransformImage;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;
using static PicView.ChangeImage.Navigation;

namespace PicView.UILogic.DragAndDrop
{
    internal static class DragToExplorer
    {
        internal static async void DragFile(object sender, MouseButtonEventArgs e)
        {
            if (ConfigureWindows.GetMainWindow.MainImage.Source == null
                || Keyboard.Modifiers != ModifierKeys.Control
                || Keyboard.Modifiers == ModifierKeys.Shift
                || Keyboard.Modifiers == ModifierKeys.Alt
                || GalleryFunctions.IsHorizontalOpen
                || Settings.Default.Fullscreen
                || Scroll.IsAutoScrolling
                || ZoomLogic.IsZoomed)
            {
                return;
            }

            if (ConfigureWindows.GetMainWindow.TitleText.IsFocused)
            {
                EditTitleBar.Refocus();
                return;
            }

            string file;

            if (Pics.Count == 0)
            {
                try
                {
                    // Download from internet
                    // TODO check file exceptions and errors
                    string url = FileFunctions.GetURL(ConfigureWindows.GetMainWindow.TitleText.Text);
                    if (Uri.IsWellFormedUriString(url, UriKind.Absolute)) // Check if from web
                    {
                        file = await HttpFunctions.DownloadData(url, false).ConfigureAwait(false);
                    }
                    else
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Tooltip.ShowTooltipMessage(ex);
                    return;
                }
            }
            else if (Pics.Count > FolderIndex)
            {
                file = Pics[FolderIndex];
            }
            else
            {
                return;
            }

            FrameworkElement? senderElement = sender as FrameworkElement;
            DataObject dragObj = new DataObject();
            dragObj.SetFileDropList(new StringCollection { file });
            DragDrop.DoDragDrop(senderElement, dragObj, DragDropEffects.Copy);

            e.Handled = true;
        }
    }
}