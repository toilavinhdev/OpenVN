using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SharedKernel.Libraries.QueryBuilder
{
    public enum RelationType
    {
        [Description("1 - 1")]
        OO,
        [Description("1 - N")]
        OM,
        [Description("N - N")]
        MM,
        [Description("N - 1")]
        MO,
    }
}
