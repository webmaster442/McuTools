using System.Collections.Generic;

namespace McuTools.Interfaces.Controls.Syntax
{

    /// <summary>
    /// Abstract token parser
    /// </summary>
    public abstract class SyntaxLexer
    {
        protected List<CodeToken> _tokens;


        /// <summary>
        /// Constructor
        /// </summary>
        public SyntaxLexer()
        {
            _tokens = new List<CodeToken>();
        }

        /// <summary>
        /// Parse code 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="carret_position"></param>
        public abstract void Parse(string text, int caret_position);

        /// <summary>
        /// Key for auto show suggestion list
        /// </summary>
        public abstract System.Windows.Input.Key SuggestionListTriggerKey
        {
            get;
        }

        /// <summary>
        /// Can syntaxhiglight textbox show suggestion list?
        /// </summary>
        /// <param name="caret_position"></param>
        /// <returns></returns>
        public abstract bool CanShowSuggestionList(int caret_position);
        

        /// <summary>
        /// List of tokens - result of parsing
        /// </summary>
        public IEnumerable<CodeToken> Tokens
        {
            get
            {
                return _tokens.AsReadOnly();
            }
        }
    }
}
