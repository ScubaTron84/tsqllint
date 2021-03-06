using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Parser.Interfaces;
using TSQLLint.Lib.Utility;

namespace TSQLLint.Lib.Parser
{
    public class FragmentBuilder : IFragmentBuilder
    {
        private readonly TSqlParser parser;

        public FragmentBuilder(int compatabilityLevel)
        {
            parser = GetSqlParser(CompatabilityLevelUtility.ValidateCompatabilityLevel(compatabilityLevel));
        }

        public TSqlFragment GetFragment(TextReader txtRdr, out IList<ParseError> errors)
        {
            var fragment = parser.Parse(txtRdr, out errors);
            return fragment?.FirstTokenIndex != -1 ? fragment : null;
        }

        private static TSqlParser GetSqlParser(int compatabilityLevel)
        {
            compatabilityLevel = CompatabilityLevelUtility.ValidateCompatabilityLevel(compatabilityLevel);

            var fullyQualifiedName = string.Format("Microsoft.SqlServer.TransactSql.ScriptDom.TSql{0}Parser", compatabilityLevel);
            var type = Type.GetType(fullyQualifiedName);

            TSqlParser parser = new TSql120Parser(true);

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = asm.GetType(fullyQualifiedName);
                if (type != null)
                {
                    parser = (TSqlParser)Activator.CreateInstance(type, new object[] { true });
                    break;
                }
            }

            return parser;
        }
    }
}
