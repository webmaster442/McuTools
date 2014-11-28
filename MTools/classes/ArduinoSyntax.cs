using McuTools.Interfaces.Controls.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTools.classes
{
    class ArduinoSyntax : SyntaxLexer
    {
        public override void Parse(string text, int caret_position)
        {
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
