using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gudusoft.gsqlparser;
using System.IO;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using DataTypes;
using QueryTextDriverExceptionNS;
using System.Globalization;

namespace QueryTextDriver
{
    public interface ISelectFrom
    {
        ISelectJoin Join(TLzJoinList joins);
    }

    public interface ISelectWhere
    {
        TableJoin Select(TLzFieldList fields);
        ISelectGroupBy GroupBy(TLzGroupBy groupBy);
        ISelectOrderBy OrderBy(TLzOrderByList sortList);
        ISelectLimit Limit(TLz_SelectLimitClause limit);
    }

    public interface ISelectGroupBy
    {
        TableJoin Select(TLzFieldList fields);
        ISelectOrderBy OrderBy(TLzOrderByList sortList);
        ISelectHaving Having(TLzCustomExpression expression);
        ISelectLimit Limit(TLz_SelectLimitClause limit);
    }

    public interface ISelectHaving
    {
        TableJoin Select(TLzFieldList fields);
        ISelectOrderBy OrderBy(TLzOrderByList sortList);
        ISelectLimit Limit(TLz_SelectLimitClause limit);
    }

    public interface ISelectOrderBy
    {
        TableJoin Select(TLzFieldList fields);
        ISelectLimit Limit(TLz_SelectLimitClause limit);
    }

    public interface ISelectJoin
    {
        TableJoin Select(TLzFieldList fields);
        ISelectWhere Where(TLzCustomExpression expression, QueryConfig config);
        ISelectGroupBy GroupBy(TLzGroupBy groupBy);
        ISelectOrderBy OrderBy(TLzOrderByList sortList);
        ISelectLimit Limit(TLz_SelectLimitClause limit);
    }

    public interface ISelectLimit
    {
        TableJoin Select(TLzFieldList fields);
    }

    public class SelectLinq : ISelectFrom, ISelectWhere, ISelectGroupBy, ISelectHaving, ISelectOrderBy, ISelectJoin, ISelectLimit
	{
        //Список таблиц выборки
        private Collection<TableJoin> tmpJoins = new Collection<TableJoin>();
        //Результирующая таблица
        private TableJoin resultJoin = new TableJoin();
        //Группы groupBy
        private Collection<RowJoin> rowGroups = new Collection<RowJoin>();
        //Сортировочные группы
        private Collection<OrderJoin> orderJoins = new Collection<OrderJoin>();

        private QueryConfig config;
        private ExpressionEvaluator evaluator;

        //Limit StartIndex, EndIndex
        private int StartIndex;
        private int EndIndex;

        public SelectLinq(QueryConfig config)
        {
            if (config == null)
                this.config = new QueryConfig(" ", Environment.NewLine, false, false);
            else
                this.config = new QueryConfig(config.ColumnSeparator, config.RowSeparator, config.FirstRowHeader, config.IgnoreDataTypes);
            this.evaluator = ExpressionEvaluator.CreateEvaluator(this.config);
            this.EndIndex = int.MaxValue;
        }

        public ISelectFrom From(TLzTableList tables)
        {
            if (tables == null)
                throw new QueryTextDriverException("Не передана ссылка на список таблиц");
            if (tables.Count() == 0)
                resultJoin.Rows.Add(new RowClass());
            foreach (TLzTable table in tables)
            {
                if (table.SubQuery != null)
                {
                    QueryExecutor executor = new QueryExecutor(config);
                    this.tmpJoins.Add(executor.Execute(table.SubQuery.AsText));
                    foreach (ColumnClass column in this.tmpJoins[this.tmpJoins.Count - 1].Columns)
                    {
                        column.Table.TableName = "";
                        column.Table.TableAlias = table.AliasClause.aliastext;
                    }
                }
                else
                {
                    string fileName = table.TableToken.AsText.Trim(new char[] { '"' });
                    if (!File.Exists(fileName))
                    {
                        QueryTextDriverException exception = new QueryTextDriverException("Файл {0} в блоке FROM не существует");
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
                    this.tmpJoins.Add(new TableJoin(columns, rows));
                }
            }
            return this;
        }

        public ISelectJoin Join(TLzJoinList joins)
        {
            if (joins == null)
                throw new QueryTextDriverException("Не передана ссылка на коллекцию объединений");
            Collection<TableJoin> dekart_tables = new Collection<TableJoin>();
            int index = 0;  //Индекс таблицы в списке таблиц
            foreach (TLzJoin join in joins)
            {
                TableJoin rt = tmpJoins[index];
                foreach (TLzJoinItem joinItem in join.JoinItems)
                {
                    index++;
                    switch (joinItem.JoinType)
                    {
                        case TSelectJoinType.sjtjoininner:
                            if (joinItem.JoinQualType != TSelectJoinQual.sjqOn)
                            {
                                QueryTextDriverException exceptionJqOn = new QueryTextDriverException("Пропущена инструкция ON в объединении {0}");
                                exceptionJqOn.Data.Add("{0}", joinItem.AsText);
                                throw exceptionJqOn;
                            }
                            rt = InnerJoin(rt, tmpJoins[index], joinItem.JoinQual);
                            break;
                        case TSelectJoinType.sjtleftouter:
                        case TSelectJoinType.sjtleft:
                            rt = LeftJoin(rt, tmpJoins[index], joinItem.JoinQual);
                            break;
                        case TSelectJoinType.sjtrightouter:
                        case TSelectJoinType.sjtright:
                            rt = RightJoin(rt, tmpJoins[index], joinItem.JoinQual);
                            break;
                        default:
                            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип соединения {0}");
                            exception.Data.Add("{0}", joinItem.AsText);
                            throw exception;
                    }
                }
                index++;
                dekart_tables.Add(rt);
            }
            //Декартово произведение
            if (dekart_tables.Count > 0)
            {
                resultJoin = dekart_tables[0];
                for (int i = 1; i < dekart_tables.Count; i++)
                {
                    TableJoin tmp_table = new TableJoin();
                    foreach (ColumnClass column in resultJoin.Columns)
                    {
                        ColumnClass new_column = new ColumnClass(new Collection<CellClass>(), column.ColumnName, column.ColumnAlias, column.Table);
                        tmp_table.Columns.Add(new_column);
                    }
                    foreach (ColumnClass column in dekart_tables[i].Columns)
                    {
                        ColumnClass new_column = new ColumnClass(new Collection<CellClass>(), column.ColumnName, column.ColumnAlias, column.Table);
                        tmp_table.Columns.Add(new_column);
                    }
                    for (int j = 0; j < resultJoin.Columns[0].CellsCount; j++)
                        for (int k = 0; k < dekart_tables[i].Columns[0].CellsCount; k++)
                        {
                            RowClass rows = new RowClass();
                            for (int l = 0; l < resultJoin.Columns.Count; l++)
                            {
                                tmp_table.Columns[l].AddCell(resultJoin.Columns[l].GetCell(j));
                                rows.Cells.Add(resultJoin.Columns[l].GetCell(j));
                            }
                            for (int l = 0; l < dekart_tables[i].Columns.Count; l++)
                            {
                                tmp_table.Columns[l + resultJoin.Columns.Count].AddCell(dekart_tables[i].Columns[l].GetCell(k));
                                rows.Cells.Add(dekart_tables[i].Columns[l].GetCell(k));
                            }
                            tmp_table.Rows.Add(rows);
                        }
                    resultJoin = tmp_table;
                }
            }
            return this;
        }

        private TableJoin InnerJoin(TableJoin left, TableJoin right, TLzCustomExpression expression)
        {
            if ((left == null) || (right == null) || (expression == null))
                throw new QueryTextDriverException("Заданы не все обязательные параметры");
            Collection<ColumnClass> new_columns = new Collection<ColumnClass>();
            Collection<RowClass> new_rows = new Collection<RowClass>();
            RowJoin rowJoin = new RowJoin();
            //Формируем структуру нового табличного выражения
            foreach (ColumnClass column in left.Columns)
            {
                ColumnClass new_column = new ColumnClass(new Collection<CellClass>(), column.ColumnName, column.ColumnAlias, column.Table);
                new_columns.Add(new_column);
            }
            foreach (ColumnClass column in right.Columns)
            {
                ColumnClass new_column = new ColumnClass(new Collection<CellClass>(), column.ColumnName, column.ColumnAlias, column.Table);
                new_columns.Add(new_column);
            }
            //Проходим по всем строкам левого табличного выражения и ищем для них соответствие в правом табличном выражении
            for (int i = 0; i < left.Columns[0].CellsCount; i++)
                for (int j = 0; j < right.Columns[0].CellsCount; j++)
                {
                    //Объединяем ячейки для проверки выражения
                    CellJoin cellJoin = new CellJoin();
                    foreach (ColumnClass column in left.Columns)
                        cellJoin.Cells.Add(column.GetCell(i));
                    foreach (ColumnClass column in right.Columns)
                        cellJoin.Cells.Add(column.GetCell(j));
                    bool result = false;
                    try
                    {
                        result = evaluator.Evaluate(cellJoin, rowJoin, expression).AsBool().Value();
                    }
                    catch (InvalidCastException)
                    {
                        QueryTextDriverException exception = new QueryTextDriverException("Ошибка вычисления выражения");
                        exception.Data.Add("{0}", expression.AsText);
                        throw exception;
                    }
                    if (result)
                    {
                        RowClass row = new RowClass();
                        new_rows.Add(row);
                        //Если условие в блоке ON истинно, то добавить строки из левого и правого табличного выражения
                        for (int k = 0; k < left.Columns.Count; k++)
                        {
                            new_columns[k].AddCell(left.Columns[k].GetCell(i));
                            row.Cells.Add(left.Columns[k].GetCell(i));
                        }
                        for (int k = 0; k < right.Columns.Count; k++)
                        {
                            new_columns[left.Columns.Count + k].AddCell(right.Columns[k].GetCell(j));
                            row.Cells.Add(right.Columns[k].GetCell(j));
                        }
                    }
                }
            return new TableJoin(new_columns, new_rows);
        }

        private TableJoin LeftJoin(TableJoin left, TableJoin right, TLzCustomExpression expression)
        {
            if ((left == null) || (right == null) || (expression == null))
                throw new QueryTextDriverException("Заданы не все обязательные параметры");
            Collection<ColumnClass> new_columns = new Collection<ColumnClass>();
            Collection<RowClass> new_rows = new Collection<RowClass>();
            RowJoin rowJoin = new RowJoin();
            //Формируем структуру нового табличного выражения
            foreach (ColumnClass column in left.Columns)
            {
                ColumnClass new_column = new ColumnClass(new Collection<CellClass>(), column.ColumnName, column.ColumnAlias, column.Table);
                new_columns.Add(new_column);
            }
            foreach (ColumnClass column in right.Columns)
            {
                ColumnClass new_column = new ColumnClass(new Collection<CellClass>(), column.ColumnName, column.ColumnAlias, column.Table);
                new_columns.Add(new_column);
            }
            //Проходим по всем строкам левого табличного выражения и ищем для них соответствие в правом табличном выражении
            for (int i = 0; i < left.Columns[0].CellsCount; i++)
            {
                bool equal_founded = false;
                for (int j = 0; j < right.Columns[0].CellsCount; j++)
                {
                    //Объединяем ячейки для проверки выражения
                    CellJoin cellJoin = new CellJoin();
                    foreach (ColumnClass column in left.Columns)
                        cellJoin.Cells.Add(column.GetCell(i));
                    foreach (ColumnClass column in right.Columns)
                        cellJoin.Cells.Add(column.GetCell(j));
                    bool result = false;
                    try
                    {
                        result = evaluator.Evaluate(cellJoin, rowJoin, expression).AsBool().Value();
                    }
                    catch (InvalidCastException)
                    {
                        QueryTextDriverException exception = new QueryTextDriverException("Ошибка вычисления выражения");
                        exception.Data.Add("{0}", expression.AsText);
                        throw exception;
                    }
                    if (result)
                    {
                        //Если условие в блоке ON истинно, то добавить строки из левого и правого табличного выражения
                        RowClass row = new RowClass();
                        new_rows.Add(row);
                        for (int k = 0; k < left.Columns.Count; k++)
                        {
                            new_columns[k].AddCell(left.Columns[k].GetCell(i));
                            row.Cells.Add(left.Columns[k].GetCell(i));
                        }
                        for (int k = 0; k < right.Columns.Count; k++)
                        {
                            new_columns[left.Columns.Count + k].AddCell(right.Columns[k].GetCell(j));
                            row.Cells.Add(right.Columns[k].GetCell(j));
                        }
                        equal_founded = true;
                    }
                }
                //Если вхождения были найдены, то с пустой правой частью строку не добавлять, иначе добавить
                if (!equal_founded)
                {
                    RowClass row = new RowClass();
                    new_rows.Add(row);
                    for (int k = 0; k < left.Columns.Count; k++)
                    {
                        new_columns[k].AddCell(left.Columns[k].GetCell(i));
                        row.Cells.Add(left.Columns[k].GetCell(i));
                    }
                    for (int k = 0; k < right.Columns.Count; k++)
                    {
                        CellClass cell = new CellClass("", new_columns[left.Columns.Count + k], row, config.IgnoreDataTypes);
                        row.Cells.Add(cell);
                        new_columns[left.Columns.Count + k].AddCell(cell);
                    }
                }
            }
            return new TableJoin(new_columns, new_rows);
        }

        private TableJoin RightJoin(TableJoin left, TableJoin right, TLzCustomExpression expression)
        {
            if ((left == null) || (right == null) || (expression == null))
                throw new QueryTextDriverException("Заданы не все обязательные параметры");
            Collection<ColumnClass> new_columns = new Collection<ColumnClass>();
            Collection<RowClass> new_rows = new Collection<RowClass>();
            RowJoin rowJoin = new RowJoin();
            //Формируем структуру нового табличного выражения
            foreach (ColumnClass column in left.Columns)
            {
                ColumnClass new_column = new ColumnClass(new Collection<CellClass>(), column.ColumnName, column.ColumnAlias, column.Table);
                new_columns.Add(new_column);
            }
            foreach (ColumnClass column in right.Columns)
            {
                ColumnClass new_column = new ColumnClass(new Collection<CellClass>(), column.ColumnName, column.ColumnAlias, column.Table);
                new_columns.Add(new_column);
            }
            //Проходим по всем строкам левого табличного выражения и ищем для них соответствие в правом табличном выражении
            for (int i = 0; i < right.Columns[0].CellsCount; i++)
            {
                bool equal_founded = false;
                for (int j = 0; j < left.Columns[0].CellsCount; j++)
                {
                    //Объединяем ячейки для проверки выражения
                    CellJoin cellJoin = new CellJoin();
                    foreach (ColumnClass column in left.Columns)
                        cellJoin.Cells.Add(column.GetCell(j));
                    foreach (ColumnClass column in right.Columns)
                        cellJoin.Cells.Add(column.GetCell(i));
                    bool result = false;
                    try
                    {
                        result = evaluator.Evaluate(cellJoin, rowJoin, expression).AsBool().Value();
                    }
                    catch (InvalidCastException)
                    {
                        QueryTextDriverException exception = new QueryTextDriverException("Ошибка вычисления выражения");
                        exception.Data.Add("{0}", expression.AsText);
                        throw exception;
                    }
                    if (result)
                    {
                        //Если условие в блоке ON истинно, то добавить строки из левого и правого табличного выражения
                        RowClass row = new RowClass();
                        new_rows.Add(row);
                        for (int k = 0; k < left.Columns.Count; k++)
                        {
                            new_columns[k].AddCell(left.Columns[k].GetCell(j));
                            row.Cells.Add(left.Columns[k].GetCell(j));
                        }
                        for (int k = 0; k < right.Columns.Count; k++)
                        {
                            new_columns[left.Columns.Count + k].AddCell(right.Columns[k].GetCell(i));
                            row.Cells.Add(right.Columns[k].GetCell(i));
                        }
                        equal_founded = true;
                    }
                }
                //Если вхождения были найдены, то с пустой левой частью строку не добавлять, иначе добавить
                if (!equal_founded)
                {
                    RowClass row = new RowClass();
                    new_rows.Add(row);
                    for (int k = 0; k < left.Columns.Count; k++)
                    {
                        CellClass cell = new CellClass("", new_columns[k], row, config.IgnoreDataTypes);
                        row.Cells.Add(cell);
                        new_columns[k].AddCell(cell);
                    }
                    for (int k = 0; k < right.Columns.Count; k++)
                    {
                        new_columns[left.Columns.Count + k].AddCell(right.Columns[k].GetCell(i));
                        row.Cells.Add(right.Columns[k].GetCell(i));
                    }
                }
            }
            return new TableJoin(new_columns, new_rows);
        }

        public ISelectWhere Where(TLzCustomExpression expression, QueryConfig config)
        {          
            if (expression == null)
                return this;
            Collection<ColumnClass> new_columns = new Collection<ColumnClass>();
            Collection<RowClass> new_rows = new Collection<RowClass>();
            RowJoin rowJoin = new RowJoin();
            //Формируем структуру нового табличного выражения
            foreach (ColumnClass column in resultJoin.Columns)
            {
                ColumnClass new_column = new ColumnClass(new Collection<CellClass>(), column.ColumnName, column.ColumnAlias, column.Table);
                new_columns.Add(new_column);
            }
            for (int i = 0; i < resultJoin.Columns[0].CellsCount; i++)
            {
                //Объединяем ячейки для проверки выражения
                CellJoin cellJoin = new CellJoin();
                foreach (ColumnClass column in resultJoin.Columns)
                    cellJoin.Cells.Add(column.GetCell(i));
                //Ищем группу, в которую входит строка
                if (rowGroups.Count > 0)
                {
                    for (int j = 0; j < rowGroups.Count; j++)
                    {
                        bool founded = false;
                        for (int k = 0; k < rowGroups[j].Rows.Count; k++)
                            if (rowGroups[j].Rows[k] == resultJoin.Rows[i])
                            {
                                founded = true;
                                break;
                            }
                        if (founded)
                        {
                            rowJoin = rowGroups[j];
                            break;
                        }
                    }
                }
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
                {
                    RowClass row = new RowClass();
                    new_rows.Add(row);
                    for (int j = 0; j < resultJoin.Columns.Count; j++)
                    {
                        new_columns[j].AddCell(resultJoin.Columns[j].GetCell(i));
                        row.Cells.Add(resultJoin.Columns[j].GetCell(i));
                    }
                }
            }
            resultJoin = new TableJoin(new_columns, new_rows);
            return this;
        }

        public TableJoin Select(TLzFieldList fields)
        {
            if (fields == null)
                throw new QueryTextDriverException("Не задана ссылка на список полей для выборки данных");
            TableJoin result = new TableJoin();
            Collection<ColumnClass> columns = new Collection<ColumnClass>();
            //Формируем выходные строки
            int rowIndex = 0; //Номер строки
            if ((orderJoins.Count == 1) && (rowGroups.Count == 0))
            {
                //Если сортировочных группа 1, а группировочных 0, то используется сортировка без группировки => вычислять каждую строку
                for (int i = 0; i < orderJoins.Count; i++)
                    for (int j = 0; j < orderJoins[i].RowJoin.Rows.Count; j++)
                    {
                        if ((rowIndex >= StartIndex) && (rowIndex <= EndIndex))
                            result.Rows.Add(SelectRow(orderJoins[i].RowJoin.Rows[j], orderJoins[i].RowJoin, fields));
                        rowIndex++;
                    }
            } else
                if (orderJoins.Count > 1)
                {
                    //Если сортировочных групп больше 1, то используется группировка => вычислять только одну строку для каждой группы
                    
                    for (int i = 0; i < orderJoins.Count; i++)
                    {
                        if ((rowIndex >= StartIndex) && (rowIndex <= EndIndex))
                            result.Rows.Add(SelectRow(orderJoins[i].RowJoin.Rows[0], orderJoins[i].RowJoin, fields));
                        rowIndex++;
                    }
                } else
                    if (rowGroups.Count > 0)
                    {
                        //Если используется группировка, то вычислять только одну строку для каждой группы
                        for (int i = 0; i < rowGroups.Count; i++)
                        {
                            if ((rowIndex >= StartIndex) && (rowIndex <= EndIndex))
                                result.Rows.Add(SelectRow(rowGroups[i].Rows[0], rowGroups[i], fields));
                            rowIndex++;
                        }
                    }
                    else
                    {
                        //Если ни группировка, ни сортировка не используются, то проверяем, есть ли агрегации
                        bool isAggregate = false;
                        foreach(TLzField field in fields)
                            if ((field.FieldExpr != null) && (evaluator.IsAggregate(field.FieldExpr)))
                            {
                                isAggregate = true;
                                break;
                            }
                        RowJoin join = new RowJoin();
                        if (isAggregate)
                        {
                            //Если агрегации есть, то вычисляем одну строку
                            for (int i = 0; i < resultJoin.Rows.Count; i++)
                                join.Rows.Add(resultJoin.Rows[i]);
                            if ((resultJoin.Rows.Count > 0) && ((rowIndex >= StartIndex) && (rowIndex <= EndIndex)))
                                result.Rows.Add(SelectRow(resultJoin.Rows[0], join, fields));
                        }
                        else
                        {
                            //Если агрегаций нет, то вычисляем каждую строку
                            for (int i = 0; i < resultJoin.Rows.Count; i++)
                            {
                                if ((rowIndex >= StartIndex) && (rowIndex <= EndIndex))
                                    result.Rows.Add(SelectRow(resultJoin.Rows[i], join, fields));
                                rowIndex++;
                            }
                        }
                    }
            //Формируем колонки
            int columnIndex = 0;
            for (int i = 0; i < fields.Count(); i++)
            {
                //Если используется * в качестве поля, то
                if (fields[i].FieldName == "*")
                {
                    for (int j = 0; j < resultJoin.Columns.Count; j++ )
                    {
                        if (String.IsNullOrEmpty(fields[i].FieldPrefix) || (fields[i].FieldPrefix == resultJoin.Columns[j].Table.TableAlias) ||
                            (fields[i].FieldPrefix == resultJoin.Columns[j].Table.TableName))
                        {
                            ColumnClass column = new ColumnClass();
                            column.ColumnName = resultJoin.Columns[j].ColumnName;
                            column.ColumnType = resultJoin.Columns[j].ColumnType;
                            column.Table = resultJoin.Columns[j].Table;
                            result.Columns.Add(column);
                            columnIndex++;
                        }
                    }

                    for (int j = 0; resultJoin.Rows.Count > 0 && j < resultJoin.Rows[0].Cells.Count; j++)
                    {
                        if (String.IsNullOrEmpty(fields[i].FieldPrefix) || (fields[i].FieldPrefix == resultJoin.Rows[0].Cells[j].Column.Table.TableAlias) ||
                            (fields[i].FieldPrefix == resultJoin.Rows[0].Cells[j].Column.Table.TableName))
                        {
                            for (int k = 0; k < result.Rows.Count; k++)
                            {
                                result.Columns[result.Columns.Count - 1].AddCell(result.Rows[k].Cells[result.Columns.Count - 1]);
                                result.Rows[k].Cells[result.Columns.Count - 1].Column = result.Columns[result.Columns.Count - 1];
                            }
                        }
                    }
                    continue;
                }
                bool foundedField = false;
                for (int j = 0; j < resultJoin.Columns.Count; j++)
                {
                    string TableName = fields[i].FieldPrefix;
                    string ColumnName = fields[i].FieldName;
                    string ColumnAlias = fields[i].FieldAlias;
                    if ((resultJoin.Columns[j].ColumnName == ColumnName) &&
                        ((resultJoin.Columns[j].Table.TableName == TableName) || (resultJoin.Columns[j].Table.TableAlias == TableName) ||
                        String.IsNullOrEmpty(TableName)))
                    {
                        ColumnClass newColumn = new ColumnClass();
                        if (ColumnAlias == "")
                            newColumn.ColumnName = resultJoin.Columns[j].ColumnName;
                        else
                            newColumn.ColumnName = ColumnAlias;
                        newColumn.ColumnType = resultJoin.Columns[j].ColumnType;
                        newColumn.Table = resultJoin.Columns[j].Table;
                        result.Columns.Add(newColumn);
                        columnIndex++;
                        foundedField = true;
                        break;
                    }
                }
                if (!foundedField)
                {
                    string ColumnAlias = fields[i].FieldAlias;
                    ColumnClass newColumn = new ColumnClass();
                    if (ColumnAlias == "")
                        newColumn.ColumnName = '`'+(columnIndex+1).ToString()+'`';
                    else
                        newColumn.ColumnName = ColumnAlias;
                    result.Columns.Add(newColumn);
                    columnIndex++;
                }
                for (int j = 0; j < result.Rows.Count; j++)
                {
                    result.Columns[result.Columns.Count-1].AddCell(result.Rows[j].Cells[result.Columns.Count-1]);
                    result.Rows[j].Cells[result.Columns.Count - 1].Column = result.Columns[result.Columns.Count - 1];
                }
                columnIndex++;
            }
            return result;
        }

        public RowClass SelectRow(RowClass row, RowJoin rowJoin, TLzFieldList fields)
        {
            if (fields == null)
                throw new QueryTextDriverException("Не задана ссылка на список полей для выборки");
            if (row == null)
                throw new QueryTextDriverException("Не задана ссылка на объект row");
            if (rowJoin == null)
                throw new QueryTextDriverException("Не задана ссылка на объект rowJoin");
            RowClass newRow = new RowClass();
            foreach (TLzField field in fields)
            {
                //Объединяем ячейки для проверки выражения
                CellJoin cellJoin = new CellJoin();
                foreach(CellClass cell in row.Cells)
                    cellJoin.Cells.Add(cell);
                CellClass newCell = new CellClass();
                if (field.SubQuery != null)
                {
                    QueryExecutor executor = new QueryExecutor(config);
                    TableJoin columnJoin = executor.Execute(field.SubQuery.AsText);
                    if (columnJoin.Columns.Count != 1 || columnJoin.Rows.Count != 1)
                    {
                        QueryTextDriverException exception = new QueryTextDriverException("Подзапрос {0} должен возвращать единственное значение");
                        exception.Data.Add("{0}", field.SubQuery.AsText);
                        throw exception;
                    }
                    newCell.Value = columnJoin.Rows[0].Cells[0].Value;
                } else
                    if (field.FieldExpr != null)
                    {
                        newCell.Value = evaluator.Evaluate(cellJoin, rowJoin, field.FieldExpr); 
                    }
                    else
                    {
                        //Если используется * в качестве поля, то
                        if (field.FieldName == "*")
                        {
                            for (int i = 0; i < row.Cells.Count; i++)
                            {
                                if (String.IsNullOrEmpty(field.FieldPrefix) || (field.FieldPrefix == row.Cells[i].Column.Table.TableAlias) ||
                                    (field.FieldPrefix == row.Cells[i].Column.Table.TableName))
                                {
                                    CellClass cell = new CellClass();
                                    cell.Value = row.Cells[i].Value;
                                    cell.Row = newRow;
                                    newRow.Cells.Add(cell);
                                }
                            }
                            continue;
                        }
                        //Иначе
                        for (int i = 0; i < row.Cells.Count; i++)
                        {
                            string TableName = field.FieldPrefix;
                            string ColumnName = field.FieldName;
                            if ((row.Cells[i].Column.ColumnName == ColumnName) &&
                                ((row.Cells[i].Column.Table.TableName == TableName) || (row.Cells[i].Column.Table.TableAlias == TableName) ||
                                String.IsNullOrEmpty(TableName)))
                            {
                                newCell.Value = row.Cells[i].Value;
                                break;
                            }
                        }
                    }
                if (newCell.Value.Equals(null))
                {
                    QueryTextDriverException exception = new QueryTextDriverException("Не удалось получить значение поля {0}");
                    exception.Data.Add("{0}", field.AsText);
                    throw exception;
                }
                newCell.Row = newRow;
                newRow.Cells.Add(newCell);
            }
            return newRow;
        }

        public ISelectGroupBy GroupBy(TLzGroupBy groupBy)
        {
            if (groupBy == null)
                return this;
            //Список значений критериев и групп
            Dictionary<Collection<CsvObject>, RowJoin> rowJoins = new Dictionary<Collection<CsvObject>, RowJoin>();
            
            //Критерии группировки
            Collection<string[]> criterias = new Collection<string[]>();
            for (int i = 0; i < groupBy.GroupItems.Count(); i++)
            {
                TLzGroupByItem item = (TLzGroupByItem)groupBy.GroupItems[i];
                if ((item.Nodetype != TNodeTag.T_GroupByItem) || (item.RawTokens.Count() > 2))
                {
                    QueryTextDriverException exception = new QueryTextDriverException("Неизвестнео выражение {0}");
                    exception.Data.Add("{0}", item.AsText);
                    throw exception;
                }
                string[] parts = item.AsText.Split(new char[] { '.' }, StringSplitOptions.None);
                if (parts.Length > 2 || parts.Length == 0)
                {
                    QueryTextDriverException exception = new QueryTextDriverException("Ошибка в идентификаторе группировки {0}");
                    exception.Data.Add("{0}", item.AsText);
                    throw exception;
                }
                string TableName = "";
                if (parts.Length == 2)
                    TableName = parts[1];
                string ColumnName = parts[0];
                criterias.Add(new string[] { TableName, ColumnName });
            }
            //Проходим по всем строкам и группируем их
            for (int i = 0; i < resultJoin.Columns[0].CellsCount; i++)
            {
                Collection<CsvObject> values = new Collection<CsvObject>(); //Значения колонок группировки для данной строки
                //Ищем значения критериев группировки для данной строки
                foreach (string[] criteria in criterias)
                {
                    bool criteria_is_finded = false;
                    foreach (ColumnClass column in resultJoin.Columns)
                    {
                        string TableName = criteria[0];
                        string ColumnName = criteria[1];
                        if ((column.ColumnName == ColumnName) &&
                            ((column.Table.TableName == TableName) || (column.Table.TableAlias == TableName) || String.IsNullOrEmpty(TableName)))
                        {
                            values.Add(column.GetCell(i).Value);
                            criteria_is_finded = true;
                            break;
                        }
                    }
                    if (!criteria_is_finded)
                    {
                        QueryTextDriverException exception = new QueryTextDriverException("Не найден критерий группировки {0}");
                        exception.Data.Add("{0}", criteria[0] + "." + criteria[1]);
                        throw exception;
                    }
                }
                //Ищем соответствующую группу в группировке, если группы нет, то создаем ее
                bool group_is_finded = false;
                foreach (KeyValuePair<Collection<CsvObject>, RowJoin> key in rowJoins)
                {
                    bool is_equal = true;
                    for (int j = 0; j < key.Key.Count; j++)
                    {
                        if ((key.Key.Count != values.Count) || (key.Key[j].GetType() != values[j].GetType()))
                        {
                            QueryTextDriverException exception = new QueryTextDriverException("Неизвестная ошибка при группировке");
                            throw exception;
                        }
                        if (key.Key[j] != values[j])
                            is_equal = false;
                    }
                    if (is_equal)
                    {
                        key.Value.Rows.Add(resultJoin.Rows[i]);
                        group_is_finded = true;
                    }
                }
                if (!group_is_finded)
                {
                    RowJoin rj = new RowJoin();
                    rj.Rows.Add(resultJoin.Rows[i]);
                    rowJoins.Add(values, rj);
                }
            }
            //Экспортируем локальный список групп в глобальную область памяти
            foreach (KeyValuePair<Collection<CsvObject>, RowJoin> key in rowJoins)
                rowGroups.Add(key.Value);
            return this;
        }

        public ISelectHaving Having(TLzCustomExpression expression)
        {
            if (expression == null)
                return this;
            Where(expression, config);
            return this;
        }

        public ISelectOrderBy OrderBy(TLzOrderByList sortList)
        {
            if (sortList == null)
                return this;
            //Строим список сортировки
            SortList list = new SortList();
            foreach (TLzOrderBy orderBy in sortList)
            {
                if ((orderBy.SortItemType != TLzSortItemType.sitExpression) || (orderBy.SortExpr.lexpr.Nodetype != TNodeTag.T_Attr))
                {
                    QueryTextDriverException exception = new QueryTextDriverException("Некорректный параметр сортировки {0}");
                    exception.Data.Add("{0}", orderBy.AsText);
                    throw exception;
                }
                string[] parts = ((TLz_Attr)orderBy.SortExpr.lexpr).AsText.Split(new char[] { '.' }, StringSplitOptions.None);
                if (parts.Length > 2 || parts.Length == 0)
                {
                    QueryTextDriverException exception = new QueryTextDriverException("Ошибка в идентификаторе сортировки {0}");
                    exception.Data.Add("{0}", ((TLz_Attr)orderBy.SortExpr.lexpr).AsText);
                    throw exception;
                }
                string TableName = "";
                if (parts.Length == 2)
                    TableName = parts[1];
                string ColumnName = parts[0];
                list.Add(TableName, ColumnName, orderBy.SortType);
            }
            //Группируем и сортируем строки в группах
            if (rowGroups.Count == 0)
            {
                orderJoins.Add(new OrderJoin(list));
                for (int j = 0; j < resultJoin.Rows.Count; j++)
                    orderJoins[0].Add(resultJoin.Rows[j]);
            }
            for (int i = 0; i < rowGroups.Count; i++)
            {
                orderJoins.Add(new OrderJoin(list));
                for (int j = 0; j < rowGroups[i].Rows.Count; j++)
                    orderJoins[i].Add(rowGroups[i].Rows[j]);
            }
            return this;
        }

        public ISelectLimit Limit(TLz_SelectLimitClause limit)
        {
            if (limit == null)
                return this;
            //Если токен 1, то офсета нет
            if (limit.StartToken == limit.EndToken)
            {
                if (!Int32.TryParse(limit.EndToken.AsText, out EndIndex))
                {
                    QueryTextDriverException exception = new QueryTextDriverException("Значение {0} в блоке LIMIT имеет нечисловое значение");
                    exception.Data.Add("{0}", limit.EndToken.AsText);
                    throw exception;
                }
                EndIndex -= 1; //В LIMIT исчисление идет от 1, в логике программы от 0
                return this;
            }
            //Получаем офсет первой строки
            if (!Int32.TryParse(limit.StartToken.AsText, out StartIndex))
            {
                QueryTextDriverException exception = new QueryTextDriverException("Значение {0} в блоке LIMIT имеет нечисловое значение");
                exception.Data.Add("{0}", limit.StartToken.AsText);
                throw exception;
            }
            StartIndex -= 1; //В LIMIT исчисление идет от 1, в логике программы от 0
            if (!Int32.TryParse(limit.EndToken.AsText, out EndIndex))
            {
                QueryTextDriverException exception = new QueryTextDriverException("Значение {0} в блоке LIMIT имеет нечисловое значение");
                exception.Data.Add("{0}", limit.EndToken.AsText);
                throw exception;
            }
            EndIndex -= 1; //В LIMIT исчисление идет от 1, в логике программы от 0
            return this;
        }
    }
}
