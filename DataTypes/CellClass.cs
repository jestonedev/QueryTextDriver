using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTypes
{
    public class CellClass
    {
        public CsvObject Value { get; set; }
        public ColumnClass Column { get; set; }
        public RowClass Row { get; set; }
        public Type ValueType
        {
            get
            {
                return Value.GetValueType();
            }
        }

        public CellClass(object value, ColumnClass column, RowClass row)
        {
            this.Value = CsvObject.Create(value);
            this.Column = column;
            this.Row = row;
        }

        public CellClass()
        {
            Column = new ColumnClass();
            Row = new RowClass();
        }
    }
}
