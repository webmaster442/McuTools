
namespace McuTools.Interfaces.Controls.Syntax
{
    /// <summary>
    /// Code Token
    /// </summary>
    public class CodeToken
    {
        /// <summary>
        /// Type of token (Keyword, Comment, String literal, etc.) 
        /// </summary>
        public CodeTokenType TokenType
        {
            get;
            set;
        }

        /// <summary>
        /// Start index in text
        /// </summary>
        public int Start
        {
            get;
            set;
        }

        /// <summary>
        /// End index in text
        /// </summary>
        public int End
        {
            get;
            set;
        }

        /// <summary>
        /// Lenght of token
        /// </summary>
        public int Length
        {
            get
            {
                return End - Start;
            }
        }
    }
}
