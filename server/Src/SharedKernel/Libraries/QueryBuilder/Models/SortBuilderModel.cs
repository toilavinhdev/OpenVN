using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Libraries.QueryBuilder
{
    public class SortBuilderModel
    {
        private string _table;

        public string Table
        {
            get { return _table?.ToLower(); }
            set { _table = value; }
        }

        public string SortColumn { get; set; }

        public bool IsAscending { get; set; } = true;
    }
}
