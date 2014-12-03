using McuTools.Interfaces.NativeCode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace McuTools.Interfaces.WPF
{
    public static class WpfHelpers
    {
        public static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;
            T foundChild = null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }
            return foundChild;
        }

        public static void ExceptionDialog(Exception ex)
        {
#if DEBUG
            MessageBox.Show(string.Format("{0},\r\nSource: {1}\r\nTrace: {2}", ex.Message, ex.Source, ex.StackTrace), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
#else
             MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
#endif
        }

        public static void ExceptionDialog(string text, Exception ex = null)
        {
            if (ex == null)  MessageBox.Show(text, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
#if DEBUG
                MessageBox.Show(string.Format("{0}\r\n{1},\r\nSource: {2}\r\nTrace: {3}", text, ex.Message, ex.Source, ex.StackTrace), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
#else
                MessageBox.Show(text+"\r\n"+ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
#endif
            }
        }


        public static IEnumerable<T> FindChildren<T>(this DependencyObject source) where T : DependencyObject
        {
            if (source != null)
            {
                var childs = GetChildObjects(source);
                foreach (DependencyObject child in childs)
                {
                    //analyze if children match the requested type
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    //recurse tree
                    foreach (T descendant in FindChildren<T>(child))
                    {
                        yield return descendant;
                    }
                }
            }
        }

        public static IEnumerable<DependencyObject> GetChildObjects(this DependencyObject parent)
        {
            if (parent == null) yield break;

            if (parent is ContentElement || parent is FrameworkElement)
            {
                //use the logical tree for content / framework elements
                foreach (object obj in LogicalTreeHelper.GetChildren(parent))
                {
                    var depObj = obj as DependencyObject;
                    if (depObj != null) yield return (DependencyObject)obj;
                }
            }
            else
            {
                //use the visual tree per default
                int count = VisualTreeHelper.GetChildrenCount(parent);
                for (int i = 0; i < count; i++)
                {
                    yield return VisualTreeHelper.GetChild(parent, i);
                }
            }
        }

        public static Storyboard FindStoryBoard(ContentControl w, string name)
        {
            Storyboard sb = w.FindResource(name) as Storyboard;
            return sb;
        }

        private static ImageSource ToImageSource(this Icon icon)
        {
            Bitmap bitmap = icon.ToBitmap();
            IntPtr hBitmap = bitmap.GetHbitmap();

            ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            if (!NativeMethods.DeleteObject(hBitmap))
            {
                throw new Win32Exception();
            }

            return wpfBitmap;
        }

        public static ImageSource GetExeIcon(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            return Icon.ExtractAssociatedIcon(path).ToImageSource();
        }
    }
}
