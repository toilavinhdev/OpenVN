using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Libraries.QueryBuilder
{
    public class JoinBuilderModel
    {
        private string _table;

        public string Table
        {
            get { return _table?.ToLower(); }
            set { _table = value; }
        }

        public JoinType JoinType { get; set; } = JoinType.InnerJoin;

        public string MainTableJoinOnColumn { get; set; } = "Id";

        public string JoinOnColumn { get; set; }

        public List<string> TakeColumns { get; set; } = new List<string>();
    }
}
