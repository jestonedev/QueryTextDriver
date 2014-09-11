using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace DataTypes
{
    public class TableJoin
    {
        public Collection<ColumnClass> Columns { get; set; }
        public Collection<RowClass> Rows { get; set; }

        public TableJoin(Collection<ColumnClass> columns, Collection<RowClass> rows)
        {
            this.Columns = columns;
            this.Rows = rows;
        }

        public TableJoin()
        {
            Columns = new Collection<ColumnClass>();
            Rows = new Collection<RowClass>();
        }
    }
}
