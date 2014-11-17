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
        private SortList sortList;

        public OrderJoin(SortList sortList)
        {
            this.sortList = sortList;
            RowJoin = new RowJoin();
        }

        public void Add(RowClass row)
        {
            if (row == null)
                throw new QueryTextDriverException("Не задан ссылка на строку");
            if (sortList.Count == 0)
                throw new QueryTextDriverException("Не заданы параметры сортировки");
            //Составляем список индексов значений, соответствующих сортировочному списку
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
                {
                    QueryTextDriverException exception = new QueryTextDriverException("Неизвестный параметр сортировки {0}");
                    exception.Data.Add("{0}", sortList[i].TableName+"."+sortList[i].ColumnName+" "+(sortList[i].SortType == TLzSortType.srtAsc? "ASC" : "DESC"));
                    throw exception;
                }
            }
            bool is_finded_index = false;
            int finded_index = RowJoin.Rows.Count;
            for (int i = 0; i < RowJoin.Rows.Count; i++)
            {
                foreach (KeyValuePair<int, SortItem> index in indexes)
                {
                    if (((RowJoin.Rows[i].Cells[index.Key].Value > row.Cells[index.Key].Value) && (index.Value.SortType == TLzSortType.srtAsc)) ||
                        ((RowJoin.Rows[i].Cells[index.Key].Value < row.Cells[index.Key].Value) && (index.Value.SortType == TLzSortType.srtDesc)))
                    {
                        finded_index = i;
                        is_finded_index = true;
                        break;
                    }
                }
                if (is_finded_index)
                    break;
            }
            RowJoin.Rows.Insert(finded_index, row);
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
