using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace DataTypes
{
    public class CellJoin
    {
        private Collection<CellClass> cells;
        public Collection<CellClass> Cells { get { return cells; } }
        public CellJoin(Collection<CellClass> cells)
        {
            this.cells = cells;
        }

        public CellJoin()
        {
            cells = new Collection<CellClass>();
        }
    }
}
