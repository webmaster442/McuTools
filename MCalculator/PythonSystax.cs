using McuTools.Interfaces.Controls.Syntax;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace MCalculator
{
    class PythonSystax : SyntaxLexer
    {
        public ScriptEngine Engine { get; set; }

        public override void Parse(string text, int caret_position)
        {
            _tokens.Clear();
            var tokenizer = Engine.GetService<TokenCategorizer>();
            if (tokenizer == null) return;

            // init with whole text
            tokenizer.Initialize(null, Engine.CreateScriptSourceFromString(text), SourceLocation.MinValue);

            var t = tokenizer.ReadToken();
            while (t.Category != Microsoft.Scripting.TokenCategory.EndOfStream)
            {
                CodeTokenType type = CodeTokenType.None;

                switch (t.Category)
                {
                    case TokenCategory.Comment:
                    case TokenCategory.LineComment:
                    case TokenCategory.DocComment:
                        type = CodeTokenType.Comment;
                        break;

                    case TokenCategory.Keyword:
                        type = CodeTokenType.Keyword;
                        break;

                    case TokenCategory.StringLiteral:
                    case TokenCategory.CharacterLiteral:
                        type = CodeTokenType.String;
                        break;

                    case TokenCategory.NumericLiteral:
                        type = CodeTokenType.Number;
                        break;

                    case TokenCategory.Operator:
                        type = CodeTokenType.Operator;
                        break;

                    case TokenCategory.Identifier:
                        type = CodeTokenType.Indentifier;
                        break;
                }

                _tokens.Add(new CodeToken() { TokenType = type, Start = t.SourceSpan.Start.Index, End = t.SourceSpan.End.Index });
                t = tokenizer.ReadToken();
            }
        }

        public override System.Windows.Input.Key SuggestionListTriggerKey
        {
            get { return System.Windows.Input.Key.OemPeriod; }
        }

        public override bool CanShowSuggestionList(int caret_position)
        {
            return false;
        }
    }
}
