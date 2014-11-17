using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace DataTypes
{
    public class TableClass
    {
        public Collection<ColumnClass> Columns { get; set; }
        public Collection<RowClass> Rows { get; set; }
        public string TableName { get; set; }
        public string TableAlias { get; set; }

        public TableClass(Collection<ColumnClass> columns, Collection<RowClass> rows, string tableName, string tableAlias)
        {
            this.TableName = tableName;
            this.TableAlias = tableAlias;
            this.Columns = columns;
            this.Rows = rows;
        }

        public TableClass()
        {
            this.Columns = new Collection<ColumnClass>();
            this.Rows = new Collection<RowClass>();
            this.TableName = "";
            this.TableAlias = "";
        }
    }
}

