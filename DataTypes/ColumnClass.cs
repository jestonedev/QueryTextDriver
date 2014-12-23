using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using QueryTextDriverExceptionNS;

namespace DataTypes
{
    public class ColumnClass
    {
        private Collection<CellClass> Cells { get; set; }
        public string ColumnAlias { get; set; }
        public string ColumnName { get; set; }
        public Type ColumnType { get; set; }
        public TableClass Table { get; set; }
        public int CellsCount { get { return Cells.Count; } }
        public bool IsHidden { get; set; }

        public ColumnClass(Collection<CellClass> cells, string columnName, string columnAlias, TableClass table)
        {
            if (cells == null)
                throw new QueryTextDriverException("Не передана ссылка на список ячеек колонки");
            this.Cells = new Collection<CellClass>();
            foreach (CellClass cell in cells)
                AddCell(cell);
            this.ColumnName = columnName;
            this.ColumnAlias = columnAlias;
            this.Table = table;
            this.IsHidden = false;
        }

        public ColumnClass()
        {
            Table = new TableClass();
            Cells = new Collection<CellClass>();
            this.ColumnName = "";
            this.ColumnAlias = "";
            this.IsHidden = false;
        }

        public CellClass GetCell(int index)
        {
            return Cells[index];
        }

        public void AddCell(CellClass cell)
        {
            if (cell == null)
                throw new QueryTextDriverException("Не передана ссылка на ячейку колонки");
            Cells.Add(cell);
            Type CellType = cell.ValueType;
            //Если тип значений колонки не задан, то задаем его по значению текущей(первой) ячейки
            if (ColumnType == null)
                ColumnType = CellType;
            else
                //Если тип ячейки и колонки не совпадают, то пробуем конвертировать колонку в тип ячейки. 
                //При неудаче конвертируем все в строки.
                if (ColumnType != CellType)
                {
                    try
                    {
                        foreach (CellClass n_cell in this.Cells)
                            n_cell.Value = n_cell.Value.Convert(CellType);
                        ColumnType = CellType;
                    }
                    catch (FormatException)
                    {
                        foreach (CellClass n_cell in this.Cells)
                            n_cell.Value = n_cell.Value.Convert(typeof(string));
                        ColumnType = typeof(string);
                    }
                }
        }
    }
}
