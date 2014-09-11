using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gudusoft.gsqlparser;
using QueryTextDriverExceptionNS;
using System.IO;
using DataTypes;
using System.Collections.ObjectModel;

namespace QueryTextDriver
{
    public interface IDeleteFrom
    {
        IDeleteWhere Where(TLzCustomExpression expression);
    }

    public interface IDeleteWhere
    {
        int Delete();
    }

    public class DeleteLinq: IDeleteFrom, IDeleteWhere
    {
        private QueryConfig config;
        private ExpressionEvaluator evaluator;
        private TableJoin resultJoin = new TableJoin();
        private string fileName;

        //Строки, которые не будут удалены
        public RowJoin noDeleteRows = new RowJoin();

        public DeleteLinq(QueryConfig config)
        {
            this.config = new QueryConfig(config.ColumnSeparator, config.RowSeparator, config.FirstRowHeader);
            this.evaluator = ExpressionEvaluator.CreateEvaluator(this.config);
        }

        public IDeleteFrom From(TLzTable table)
        {
            if (table.SubQuery != null)
            {
                QueryTextDriverException exception = new QueryTextDriverException("Некорректное использование подзапроса {0}");
                exception.Data.Add("{0}", table.SubQuery.AsText);
                throw exception;
            }
            fileName = table.TableToken.AsText.Trim(new char[] { '"' });
            if (!File.Exists(fileName))
            {
                QueryTextDriverException exception = new QueryTextDriverException("Файл {0} в блоке UPDATE не существует");
                exception.Data.Add("{0}", fileName);
                throw exception;
            }
            //Формируем таблицу из файла
            StreamReader sr = new StreamReader(fileName);
            string text = sr.ReadToEnd();
            sr.Close();
            string[] rowsStr = text.Split(new string[] { config.RowSeparator }, StringSplitOptions.None);
            TableClass tableInfo = new TableClass();
            tableInfo.TableName = fileName;
            tableInfo.TableAlias = table.TableAlias;
            Collection<ColumnClass> columns = new Collection<ColumnClass>();
            Collection<RowClass> rows = new Collection<RowClass>();
            Collection<string[]> rawRows = new Collection<string[]>();
            int columnCount = 0;
            for (int i = 0; i < rowsStr.Length; i++)
            {
                string row_s = rowsStr[i];
                string[] cells = row_s.Split(new string[] { config.ColumnSeparator }, StringSplitOptions.None);
                rawRows.Add(cells);
                if (columnCount < cells.Length)
                    columnCount = cells.Length;
            }
            for (int i = 0; i < rowsStr.Length; i++)
            {
                string[] cells = rawRows[i];
                if (i == 0)
                {
                    //Инициализируем колонки
                    for (int j = 0; j < columnCount; j++)
                        columns.Add(new ColumnClass(new Collection<CellClass>(), "", "", tableInfo));
                    //Заполняем заголовки
                    if (config.FirstRowHeader)
                    {
                        for (int j = 0; j < cells.Length; j++)
                            columns[j].ColumnName = cells[j];
                        for (int j = cells.Length; j < columnCount; j++)
                            columns[j].ColumnName = "`" + (j + 1).ToString() + "`";
                        continue;
                    }
                    else
                    {
                        for (int j = 0; j < columnCount; j++)
                            columns[j].ColumnName = "`" + (j + 1).ToString() + "`";
                    }
                }
                //Заполняем колонки и строки
                RowClass row = new RowClass();
                row.Table = tableInfo;
                rows.Add(row);
                for (int j = 0; j < columnCount; j++)
                {
                    CellClass cell;
                    if (j < cells.Length)
                        cell = new CellClass(cells[j], columns[j], row);
                    else
                        cell = new CellClass("", columns[j], row);
                    row.Cells.Add(cell);
                    columns[j].AddCell(cell);
                }
            }
            tableInfo.Columns = columns;
            tableInfo.Rows = rows;
            resultJoin = new TableJoin(columns, rows);
            return this;
        }

        public IDeleteWhere Where(TLzCustomExpression expression)
        {
            if (expression == null)
                return this;
            Collection<RowClass> new_rows = new Collection<RowClass>();
            RowJoin rowJoin = new RowJoin();
            for (int i = 0; i < resultJoin.Columns[0].CellsCount; i++)
            {
                //Объединяем ячейки для проверки выражения
                CellJoin cellJoin = new CellJoin();
                foreach (ColumnClass column in resultJoin.Columns)
                    cellJoin.Cells.Add(column.GetCell(i));
                //Фильтруем
                bool result = false;
                try
                {
                    result = evaluator.Evaluate(cellJoin, rowJoin, expression).AsBool().Value();
                }
                catch (InvalidCastException)
                {
                    QueryTextDriverException exception = new QueryTextDriverException("Ошибка вычисления выражения {0}");
                    exception.Data.Add("{0}", expression.AsText);
                    throw exception;
                }
                if (!result)
                    new_rows.Add(resultJoin.Rows[i]);
            }
            noDeleteRows = new RowJoin(new_rows);
            return this;
        }

        public int Delete()
        {
            StreamWriter sw = new StreamWriter(fileName);
            string csv = "";
            if (config.FirstRowHeader)
            {
                for (int i = 0; i < resultJoin.Columns.Count; i++)
                    csv += resultJoin.Columns[i].ColumnName + config.ColumnSeparator;
                csv += config.RowSeparator;
            }
            for (int i = 0; i < noDeleteRows.Rows.Count; i++)
            {
                for (int j = 0; j < noDeleteRows.Rows[i].Cells.Count; j++)
                {
                    csv += noDeleteRows.Rows[i].Cells[j].Value.AsString().Value();
                    if (j != (noDeleteRows.Rows[i].Cells.Count - 1))
                        csv += config.ColumnSeparator;
                }
                if (i != noDeleteRows.Rows.Count - 1)
                    csv += config.RowSeparator;
            }
            sw.Write(csv);
            sw.Close();
            //Возвращаем число измененных строк
            return resultJoin.Rows.Count - noDeleteRows.Rows.Count;
        }
    }
}
