using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QueryTextDriver
{
    public class QueryConfig
    {
        public string ColumnSeparator { get; set; }
        public string RowSeparator { get; set; }
        public bool FirstRowHeader { get; set; }

        public QueryConfig(string columnSeparator, string rowSeparator, bool firstRowHeader)
        {
            this.ColumnSeparator = columnSeparator;
            this.RowSeparator = rowSeparator;
            this.FirstRowHeader = firstRowHeader;
        }
    }
}
