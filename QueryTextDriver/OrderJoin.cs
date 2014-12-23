using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gudusoft.gsqlparser;
using System.Collections.ObjectModel;
using DataTypes;
using QueryTextDriverExceptionNS;

namespace QueryTextDriver
{
    public class OrderJoin
    {
        public RowJoin RowJoin { get; set; }
        private RowJoin OriginalJoin { get; set; }
        private SortList sortList;

        public OrderJoin(SortList sortList)
        {
            this.sortList = sortList;
            RowJoin = new RowJoin();
            OriginalJoin = new RowJoin();
        }

        public void Add(RowClass resultRow, RowClass originalRow)
        {
            if (resultRow == null)
                throw new QueryTextDriverException("Не задан ссылка на строку resultRow");
            if (originalRow == null)
                throw new QueryTextDriverException("Не задан ссылка на строку originalRow");
            if (sortList.Count == 0)
                throw new QueryTextDriverException("Не заданы параметры сортировки");
            int? finded_index = FindIndex(resultRow, RowJoin);
            if (finded_index == null)
                finded_index = FindIndex(originalRow, OriginalJoin);
            if (finded_index == null)
            {
                QueryTextDriverException exception = new QueryTextDriverException("Неверный идентификатор колонки в блоке ORDER BY");
                throw exception;
            }
            RowJoin.Rows.Insert(finded_index.Value, resultRow);
            OriginalJoin.Rows.Insert(finded_index.Value, originalRow);
        }

        private int? FindIndex(RowClass row, RowJoin rowJoin)
        {
            Dictionary<int, SortItem> indexes = new Dictionary<int, SortItem>();
            bool is_finded = false;
            for (int i = 0; i < sortList.Count; i++)
            {
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    if ((row.Cells[j].Column.ColumnName == sortList[i].ColumnName) &&
                            ((row.Cells[j].Column.Table.TableName == sortList[i].TableName) ||
                             (row.Cells[j].Column.Table.TableAlias == sortList[i].TableName) ||
                             String.IsNullOrEmpty(sortList[0].TableName)))
                    {
                        indexes.Add(j, sortList[i]);
                        is_finded = true;
                        break;
                    }
                }
                if (!is_finded)
                    return null;
            }
            int finded_index = rowJoin.Rows.Count;
            for (int i = 0; i < rowJoin.Rows.Count; i++)
            {
                foreach (KeyValuePair<int, SortItem> index in indexes)
                {
                    if (((rowJoin.Rows[i].Cells[index.Key].Value > row.Cells[index.Key].Value) && (index.Value.SortType == TLzSortType.srtAsc)) ||
                        ((rowJoin.Rows[i].Cells[index.Key].Value < row.Cells[index.Key].Value) && (index.Value.SortType == TLzSortType.srtDesc)))
                        return i;
                }
            }
            return rowJoin.Rows.Count;
        }
    }

    public class SortList
    {
        private List<SortItem> sortList = new List<SortItem>();
        public void Add(string table, string column, TLzSortType sortType)
        {
            sortList.Add(new SortItem(table, column, sortType));
        }

        public SortItem this[int index] { get { return sortList[index]; } set { sortList[index] = value; } }

        public int Count { get { return sortList.Count; } }

        public SortList GetRange(int index, int count)
        {
            SortList list = new SortList();
            list.sortList.AddRange(sortList.GetRange(index, count));
            return list;
        }
    }

    public class SortItem
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public TLzSortType SortType { get; set; }

        public SortItem(string tableName, string columnName, TLzSortType sortType)
        {
            this.TableName = tableName;
            this.ColumnName = columnName;
            if (sortType == TLzSortType.srtNone)
                this.SortType = TLzSortType.srtAsc;
            else
                this.SortType = sortType;
        }
    }
}
