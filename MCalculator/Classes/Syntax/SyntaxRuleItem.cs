using System.Windows.Media;


namespace MCalculator.Classes.Syntax
{
    /// <summary>
    /// Highlight rule
    /// </summary>
    public class SyntaxRuleItem
    {
        /// <summary>
        /// Category of highlight rule
        /// </summary>
        public CodeTokenType RuleType
        {
            get;
            set;
        }

        /// <summary>
        /// Foreground brush
        /// </summary>
        public Brush Foreground
        {
            get;
            set;
        }
    }
}
