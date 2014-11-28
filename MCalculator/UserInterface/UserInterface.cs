using MCalculator.Maths;
using System.Windows;

namespace MCalculator.UserInterface
{
    /// <summary>
    /// User Interface related commands
    /// </summary>
    public static class UserInterface
    {
        /// <summary>
        /// Creates a basic dialog with yes and no buttons. Returns true, if the user pressed the yes button, otherwise false
        /// </summary>
        /// <param name="caption">Dialog caption</param>
        /// <param name="text">Dialog text</param>
        public static bool YesNoDialog(string caption, string text)
        {
            var result = MessageBox.Show(text, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) return true;
            else return false;
        }

        /// <summary>
        /// Creates a basic dialog with ok and cancel buttons. Returns true, if the user pressed the ok button, otherwise false
        /// </summary>
        /// <param name="caption">Dialog caption</param>
        /// <param name="text">Dialog text</param>
        public static bool ConfirmDialog(string caption, string text)
        {
            var result = MessageBox.Show(text, caption, MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK) return true;
            else return false;
        }

        /// <summary>
        /// Displays a generic error message
        /// </summary>
        /// <param name="caption">dialog caption</param>
        /// <param name="text">dialog text</param>
        public static void ErrorDialog(string caption, string text)
        {
            MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Creates a dialog that allows the modification of an existing set's items
        /// </summary>
        /// <param name="input">Set to modifiy</param>
        public static void EditSetDialog(Set input)
        {
            EditSetDlg dialog = new EditSetDlg();
            dialog.Items = input;
            if (dialog.ShowDialog() == true)
            {
                input.Clear();
                input.AddRange(dialog.Items);
            }
        }

        /// <summary>
        /// Creates a dialog that allows graphical input of set items into a new set
        /// </summary>
        public static Set CreateSetDialog()
        {
            EditSetDlg dialog = new EditSetDlg();
            if (dialog.ShowDialog() == true)
            {
                return dialog.Items;
            }
            else return null;
        }

        /// <summary>
        /// Shows a custom dialog. Returns true, if the user clicked the OK button, otherwise false
        /// </summary>
        /// <param name="form">Custom dialog to show</param>
        public static bool ShowDialog(InputForm form)
        {
            return (bool)form.ShowDialog();
        }
    }
}
