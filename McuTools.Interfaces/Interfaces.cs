using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shell;

namespace McuTools.Interfaces
{
    /// <summary>
    /// Tool Category enumeration
    /// </summary>
    public enum ToolCategory
    {
        /// <summary>
        /// Indicates that a tool supposed to be put in the analog category
        /// </summary>
        Analog,
        /// <summary>
        /// Indicates that a tool supposed to be put in the digital category
        /// </summary>
        Digital,
        /// <summary>
        /// Indicates that a tool supposed to be put in the other category
        /// </summary>
        Other,
        /// <summary>
        /// Indicates that a tool supposed to be put in the web category
        /// </summary>
        Web,
        /// <summary>
        /// Indicates that a tool supposed to be put in the external category
        /// </summary>
        External,
    }

    /// <summary>
    /// Loadable attribute
    /// </summary>
    public class Loadable : Attribute { }

    /// <summary>
    /// Tool Host Interface;
    /// </summary>
    public interface IToolHost
    {
        void OpenUrl(string url, string description = null);
        void SetProgressValue(TaskbarItemProgressState state, double percent);
        void OpenUserControlAsPopup(UserControl control, string Header);

        string ReadSubconfKey(string key);
        void WriteSubconfKey(string key, string value);
        void DeleteSubconfKey(string key);
    }

    public interface IFixedTool { }

    /// <summary>
    /// Tool base class
    /// </summary>
    [Loadable]
    public abstract class ToolBase
    {
        /// <summary>
        /// Link to host interface
        /// </summary>
        public static IToolHost Host { get; set; }
        /// <summary>
        /// Tool Description
        /// </summary>
        public abstract string Description { get; }
        /// <summary>
        /// Tool Category
        /// </summary>
        public abstract ToolCategory Category { get; }
        /// <summary>
        /// Tool Icon
        /// </summary>
        public virtual ImageSource Icon
        {
            get { return null; }
        }

        /// <summary>
        /// Tool ID for jumplist switching
        /// </summary>
        public string ID
        {
            get { return ComputeID(Description); }
        }

        /// <summary>
        /// Tracking ID
        /// </summary>
        public virtual string TrackId
        {
            get { return Category.ToString() + "/" + Description.Replace(" ", ""); }
        }

        public static string ComputeID(string input)
        {
            int val = -1024;
            if (string.IsNullOrEmpty(input)) return val.ToString();
            val += input[0];
            for (int i = 1; i < input.Length; i++) val += i * (int)input[i];
            return val.ToString();
        }
    }
    
    /// <summary>
    /// Generic tool class
    /// </summary>
    public abstract class Tool: ToolBase
    {
        /// <summary>
        /// Override this method to return the usercontrol associated to the class
        /// </summary>
        /// <returns>a usercontrol associated to the class</returns>
        public abstract UserControl GetControl();
    }

    /// <summary>
    /// A tool that is displayed in a popup window
    /// </summary>
    public abstract class PopupTool : ToolBase
    {
        /// <summary>
        /// Override this method to return the usercontrol associated to the class
        /// </summary>
        /// <returns>a usercontrol associated to the class</returns>
        public abstract UserControl GetControl();
    }

    /// <summary>
    /// External tool class
    /// </summary>
    public abstract class ExternalTool: ToolBase
    {
        /// <summary>
        /// Tool logic is implemented in this overriden method
        /// </summary>
        public abstract void RunTool();

        public abstract bool IsVisible
        {
            get;
        }

        /// <summary>
        /// Tool Category
        /// </summary>
        public override ToolCategory Category
        {
            get { return ToolCategory.External; }
        }
    }

    /// <summary>
    /// Web tool class
    /// </summary>
    public abstract class WebTool : ToolBase
    {
        /// <summary>
        /// URL of the webpage
        /// </summary>
        public abstract string URL { get; }

        /// <summary>
        /// Tool category
        /// </summary>
        public override ToolCategory Category
        {
            get { return ToolCategory.Web; }
        }
    }
}
