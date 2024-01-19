using SharedKernel.Application;
using System.Collections.Generic;

namespace SharedKernel.Libraries.QueryBuilder
{
    public class WhereBuilderModel
    {
        public List<WhereCondition> Conditions { get; set; } = new List<WhereCondition>();

        private string _formula = "(1)";
        public string Formula
        {
            get
            {
                var chars = new List<string>();
                for (int i = 0; i < _formula.Length; i++)
                {
                    if (_formula[i].IsNumber())
                    {
                        chars.Add($"[{_formula[i]}]");
                    }
                    else
                    {
                        chars.Add($"{_formula[i]}");
                    }
                }

                return string.Join("", chars);
            }
            set { _formula = value; }
        }
    }

    public class WhereCondition
    {
        private string _table;

        public string Table
        {
            get { return _table?.ToLower(); }
            set { _table = value; }
        }

        public string Column { get; set; }

        public WhereType WhereType { get; set; } = WhereType.E;

        public string WhereValue { get; set; }
    }
}
