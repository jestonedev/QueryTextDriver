using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gudusoft.gsqlparser;
using DataTypes;
using QueryTextDriverExceptionNS;

namespace QueryTextDriver
{
    public class QueryExecutor
    {
        private QueryConfig config;
        private int rowsAffected;
        public int RowsAffected { get { return rowsAffected; } }

        public QueryExecutor(string columnSeparator, string rowSeparator, bool firstRowHeader, bool ignoreDataTypes)
        {
            //Разделитель строк по умолчанию
            if (String.IsNullOrEmpty(rowSeparator) || (rowSeparator == null))
                rowSeparator = Environment.NewLine;
            this.config = new QueryConfig(columnSeparator, rowSeparator, firstRowHeader, ignoreDataTypes);
        }

        public QueryExecutor(QueryConfig config)
        {
            if (config != null)
                this.config = new QueryConfig(config.ColumnSeparator, config.RowSeparator, config.FirstRowHeader, config.IgnoreDataTypes);
            else
                this.config = new QueryConfig(" ",Environment.NewLine, false, false);
        }

        public TableJoin Execute(string query)
        {
            using (TGSqlParser parser = new TGSqlParser(TDbVendor.DbVMysql))
            {
                parser.SqlText.Text = query;
                int result = parser.Parse();
                if (result != 0)
                    throw new QueryTextDriverException(parser.ErrorMessages);
                TableJoin rt = null;
                rowsAffected = 0;
                foreach (TCustomSqlStatement stmt in parser.SqlStatements)
                {
                    switch (stmt.SqlStatementType)
                    {
                        case TSqlStatementType.sstSelect:
                            rt = rt ?? Select((TSelectSqlStatement)stmt);
                            break;
                        case TSqlStatementType.sstUpdate:
                            rowsAffected += Update((TUpdateSqlStatement)stmt);
                            break;
                        case TSqlStatementType.sstDelete:
                            rowsAffected += Delete((TDeleteSqlStatement)stmt);
                            break;
                        case TSqlStatementType.sstInsert:
                            rowsAffected += Insert((TInsertSqlStatement)stmt);
                            break;
                        default:
                            throw new QueryTextDriverException("SQL-запрос не распознан");
                    }
                }
                return rt;
            }
        }

        private TableJoin Select(TSelectSqlStatement stmt)
        {
            TableJoin left_join = new TableJoin();
            TableJoin right_join = new TableJoin();
            if ((stmt.LeftStmt != null) && (stmt.RightStmt != null))
            {
                right_join = new SelectLinq(config).From(stmt.RightStmt.Tables).Join(stmt.RightStmt.JoinTables).Where(stmt.RightStmt.WhereClause, config).
                    GroupBy(stmt.RightStmt.GroupbyClause).Having(stmt.RightStmt.HavingClause).Select(stmt.RightStmt.Fields).OrderBy(stmt.RightStmt.SortClause).
                    Limit(stmt.RightStmt.limitClause).AsTable();
                left_join = Select(stmt.LeftStmt);
            }
            if ((stmt.LeftStmt == null) && (stmt.RightStmt == null))
                return new SelectLinq(config).From(stmt.Tables).Join(stmt.JoinTables).Where(stmt.WhereClause, config).
                    GroupBy(stmt.GroupbyClause).Having(stmt.HavingClause).Select(stmt.Fields).OrderBy(stmt.SortClause).
                    Limit(stmt.limitClause).AsTable();
            if (left_join.Columns.Count != right_join.Columns.Count)
                throw new QueryTextDriverException("Несоответствие числа колонок в объединении");
            TableJoin result = new TableJoin();
            switch (stmt.SelectSetType)
            {
                case TSelectSetType.sltUnionAll:
                case TSelectSetType.sltUnion:
                        for (int i = 0; i < left_join.Columns.Count; i++)
                        {
                            ColumnClass column = new ColumnClass();
                            column.ColumnName = left_join.Columns[i].ColumnName;
                            column.ColumnAlias = left_join.Columns[i].ColumnAlias;
                            result.Columns.Add(column);
                        }
                        for (int i = 0; i < left_join.Rows.Count; i++)
                        {
                            RowClass row = new RowClass();
                            for (int j = 0; j < left_join.Rows[i].Cells.Count; j++)
                            {
                                CellClass cell = new CellClass();
                                cell.Row = left_join.Rows[i];
                                cell.Column = left_join.Columns[j];
                                cell.Value = left_join.Rows[i].Cells[j].Value;
                                row.Cells.Add(cell);
                            }
                            if (((!result.Rows.Contains(row)) && (stmt.SelectSetType == TSelectSetType.sltUnion)) ||
                                (stmt.SelectSetType == TSelectSetType.sltUnionAll))
                            {
                                for (int j = 0; j < row.Cells.Count; j++)
                                    result.Columns[j].AddCell(row.Cells[j]);
                                result.Rows.Add(row);
                            }
                        }
                        for (int i = 0; i < right_join.Rows.Count; i++)
                        {
                            RowClass row = new RowClass();
                            for (int j = 0; j < right_join.Rows[i].Cells.Count; j++)
                            {
                                CellClass cell = new CellClass();
                                cell.Row = right_join.Rows[i];
                                cell.Column = left_join.Columns[j];     //Информация о колонках сохраняется из левого объединения
                                cell.Value = right_join.Rows[i].Cells[j].Value;
                                row.Cells.Add(cell);
                            }
                            if (((!result.Rows.Contains(row)) && (stmt.SelectSetType == TSelectSetType.sltUnion)) ||
                                (stmt.SelectSetType == TSelectSetType.sltUnionAll))
                            {
                                for (int j = 0; j < row.Cells.Count; j++)
                                    result.Columns[j].AddCell(row.Cells[j]);
                                result.Rows.Add(row);
                            }
                        }
                        break;
                default:
                    QueryTextDriverException exception = new QueryTextDriverException("Неподдерживаемый оператор объединения {0}");
                    exception.Data.Add("{0}", stmt.SelectSetType.ToString());
                    throw exception;
            }
            return result;
        }

        private int Update(TUpdateSqlStatement stmt)
        {
            if (stmt.UpdateTables.Count() > 1)
                throw new QueryTextDriverException("Неподдерживается UPDATE множества таблиц");
            return new UpdateLinq(config).From(stmt.UpdateTables[0].LeftMostTable).Where(stmt.WhereClause).Set(stmt.Fields);
        }

        private int Delete(TDeleteSqlStatement stmt)
        {
            if (stmt.DeletedTables.Count() > 1)
                throw new QueryTextDriverException("Неподдерживается DELETE множества таблиц");
            return new DeleteLinq(config).From(stmt.DeletedTables[0].LeftMostTable).Where(stmt.WhereClause).Delete();
        }

        private int Insert(TInsertSqlStatement stmt)
        {
            if (stmt.Table != null)
            {
                if (stmt.subquery != null)
                {
                    QueryExecutor executor = new QueryExecutor(config);
                    return new InsertLinq(config).Into(stmt.Table).Values(executor.Select(stmt.subquery));
                }
                else
                    return new InsertLinq(config).Into(stmt.Table).Values(stmt.MultiValues);
            }
            throw new QueryTextDriverException("Неизвестная ошибка запроса INSERT");
        }
    }
}
