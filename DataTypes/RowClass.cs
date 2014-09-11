using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace DataTypes
{
    public class RowClass: IEquatable<RowClass>
    {
        public Collection<CellClass> Cells { get; set; }
        public TableClass Table { get; set; }

        public RowClass(Collection<CellClass> cells, TableClass table)
        {
            this.Cells = cells;
            this.Table = table;
        }

        public RowClass()
        {
            Table = new TableClass();
            Cells = new Collection<CellClass>();
        }

        public bool Equals(RowClass other)
        {
            if (this.Cells.Count != other.Cells.Count)
                return false;
            for (int i = 0; i < other.Cells.Count; i++)
                if (this.Cells[i].Value != other.Cells[i].Value)
                    return false;
            return true;
        }
    }
}
