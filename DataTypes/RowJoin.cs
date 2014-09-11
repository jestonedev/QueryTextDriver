using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace DataTypes
{
    public class RowJoin
    {
        public Collection<RowClass> Rows { get; set; }
        public RowJoin(Collection<RowClass> rows)
        {
            this.Rows = rows;
        }

        public RowJoin()
        {
            Rows = new Collection<RowClass>();
        }
    }
}
