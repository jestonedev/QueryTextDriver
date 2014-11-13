using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gudusoft.gsqlparser;
using DataTypes;
using QueryTextDriverExceptionNS;
using System.IO;
using System.Collections.ObjectModel;
using System.Globalization;

namespace QueryTextDriver
{
    public interface IUpdate
    {
        IUpdateWhere Where(TLzCustomExpression expression);
    }

    public interface IUpdateWhere
    {
        int Set(TLzFieldList fields);
    }

    public class UpdateLinq : IUpdate, IUpdateWhere
    {
        private QueryConfig config;
        private ExpressionEvaluator evaluator;
        private TableJoin resultJoin = new TableJoin();
        private string fileName;

        //Строки, которые будут изменены
        private RowJoin updateRows = new RowJoin();

        public UpdateLinq(QueryConfig config)
        {
            if (config == null)
                this.config = new QueryConfig(" ", Environment.NewLine, false, false);
            else
                this.config = new QueryConfig(config.ColumnSeparator, config.RowSeparator, config.FirstRowHeader, config.IgnoreDataTypes);
            this.evaluator = ExpressionEvaluator.CreateEvaluator(this.config);
        }

        public IUpdate From(TLzTable table)
        {
            if (table == null)
                throw new QueryTextDriverException("Не передана ссылка на таблицу");
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
            string text = "";
            using (StreamReader sr = new StreamReader(fileName))
                text = sr.ReadToEnd();
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
                            columns[j].ColumnName = "`" + (j + 1).ToString(CultureInfo.CurrentCulture) + "`";
                        continue;
                    }
                    else
                    {
                        for (int j = 0; j < columnCount; j++)
                            columns[j].ColumnName = "`" + (j + 1).ToString(CultureInfo.CurrentCulture) + "`";
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
                        cell = new CellClass(cells[j], columns[j], row, config.IgnoreDataTypes);
                    else
                        cell = new CellClass("", columns[j], row, config.IgnoreDataTypes);
                    row.Cells.Add(cell);
                    columns[j].AddCell(cell);
                }
            }
            tableInfo.Columns = columns;
            tableInfo.Rows = rows;
            resultJoin = new TableJoin(columns, rows);
            return this;
        }

        public IUpdateWhere Where(TLzCustomExpression expression)
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
                if (result)
                    new_rows.Add(resultJoin.Rows[i]);
            }
            updateRows = new RowJoin(new_rows);
            return this;
        }

        public int Set(TLzFieldList fields)
        {
            if (fields == null)
                throw new QueryTextDriverException("Не передана ссылка на список изменяемых полей");
            if (updateRows.Rows.Count == 0)
                return 0;
            for (int i = 0; i < updateRows.Rows.Count; i++)
            {
                RowJoin rowJoin = new RowJoin();
                //Объединяем ячейки для вычисления выражения
                CellJoin cellJoin = new CellJoin();
                for (int j = 0; j < updateRows.Rows[i].Cells.Count; j++ )
                    cellJoin.Cells.Add(new CellClass(updateRows.Rows[i].Cells[j].Value.Value(), updateRows.Rows[i].Cells[j].Column, updateRows.Rows[i].Cells[j].Row, config.IgnoreDataTypes));
                //Проходим по всем полям и UPDATE'им их
                foreach (TLzField field in fields)
                {
                    if (field.FieldExpr.oper != TLzOpType.Expr_Assign)
                    {
                        QueryTextDriverException exception = new QueryTextDriverException("Ошибка в выражении {0}. Ожидается оператор присваивания '='");
                        exception.Data.Add("{0}", field.FieldExpr.AsText);
                        throw exception;
                    }
                    if (((TLzCustomExpression)field.FieldExpr.lexpr).oper != TLzOpType.Expr_Attr)
                    {
                        QueryTextDriverException exception = new QueryTextDriverException("Ошибка в выражении {0}. В левой части выражения ожидается имя столбца");
                        exception.Data.Add("{0}", field.FieldExpr.AsText);
                        throw exception;
                    }
                    //Ищем в resultJoin индекс столбца с указанным именем
                    int columnIndex = -1;
                    string TableName = field.FieldPrefix;
                    string ColumnName = ((TLzCustomExpression)field.FieldExpr.lexpr).AsText;
                    for (int j = 0; j < resultJoin.Columns.Count; j++)
                    {
                        if ((resultJoin.Columns[j].ColumnName == ColumnName) &&
                                ((resultJoin.Columns[j].Table.TableName == TableName) ||
                                (resultJoin.Columns[j].Table.TableAlias == TableName) ||
                                String.IsNullOrEmpty(TableName)))
                        {
                            columnIndex = j;
                            break;
                        }
                    }
                    if (columnIndex == -1)
                    {
                        QueryTextDriverException exception = new QueryTextDriverException("Неизвестный столбец {0}");
                        exception.Data.Add("{0}", field.FieldFullname);
                        throw exception;
                    }
                    updateRows.Rows[i].Cells[columnIndex].Value = evaluator.Evaluate(cellJoin, rowJoin, (TLzCustomExpression)field.FieldExpr.rexpr);
                }
            }
            //Сохраняем изменения в файл
            string csv = "";
            if (config.FirstRowHeader)
            {
                for (int i = 0; i < resultJoin.Columns.Count; i++)
                    csv += resultJoin.Columns[i].ColumnName + config.ColumnSeparator;
                csv += config.RowSeparator;
            }
            for (int i = 0; i < resultJoin.Rows.Count; i++)
            {
                for (int j = 0; j < resultJoin.Rows[i].Cells.Count; j++)
                {
                    csv += resultJoin.Rows[i].Cells[j].Value.AsString().Value();
                    if (j != (resultJoin.Rows[i].Cells.Count - 1))
                        csv += config.ColumnSeparator;
                }
                if (i != resultJoin.Rows.Count - 1)
                    csv += config.RowSeparator;
            }
            using (StreamWriter sw = new StreamWriter(fileName))
                sw.Write(csv);
            //Возвращаем число измененных строк
            return updateRows.Rows.Count;
        }
    }
}
