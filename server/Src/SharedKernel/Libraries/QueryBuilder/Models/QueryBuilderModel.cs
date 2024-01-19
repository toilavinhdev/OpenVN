using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Libraries.QueryBuilder
{
    public class QueryBuilderModel
    {
        public BuildType BuildType { get; set; }

        public object MasterId { get; set; }

        public List<JoinBuilderModel> Joins { get; set; } = new List<JoinBuilderModel>();

        public WhereBuilderModel Where { get; set; } = new WhereBuilderModel();

        public List<SortBuilderModel> Sorts { get; set; } = new List<SortBuilderModel>();
    }
}
