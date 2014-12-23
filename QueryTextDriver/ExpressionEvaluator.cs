using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gudusoft.gsqlparser;
using System.Text.RegularExpressions;
using DataTypes;
using QueryTextDriverExceptionNS;
using System.Globalization;

namespace QueryTextDriver
{
    public class ExpressionEvaluator
    {
        private QueryConfig config;
        private static ExpressionEvaluator singleton;

        private ExpressionEvaluator()
        {
        }

        public static ExpressionEvaluator CreateEvaluator(QueryConfig config)
        {
            if (ExpressionEvaluator.singleton == null)
            {
                ExpressionEvaluator evaluator = new ExpressionEvaluator();
                if (config != null)
                    evaluator.config = new QueryConfig(config.ColumnSeparator, config.RowSeparator, config.FirstRowHeader, config.IgnoreDataTypes);
                else
                    evaluator.config = new QueryConfig(" ", Environment.NewLine, false, false);
                ExpressionEvaluator.singleton = evaluator;
                return evaluator;
            }
            else
                return singleton;
        }

        public bool IsAggregate(TLzCustomExpression expression)
        {
            if (expression == null)
                throw new QueryTextDriverException("Не передана ссылка на выражение агрегатора");
            switch (expression.oper)
            {
                case TLzOpType.Expr_FuncCall:
                    TLz_FuncCall func = (TLz_FuncCall)expression.lexpr;
                    switch (func.FunctionName.ToLower(CultureInfo.CurrentCulture))
                    {
                        case "max":
                        case "min":
                        case "avg":
                        case "count":
                        case "sum": return true;
                        default:
                            return false;
                    }
                default:
                    bool l_val = false;
                    bool r_val = false;
                    if ((expression.lexpr != null) && (expression.lexpr is TLzCustomExpression))
                        l_val = IsAggregate((TLzCustomExpression)expression.lexpr);
                    if ((expression.rexpr != null) && (expression.rexpr is TLzCustomExpression))
                        r_val = IsAggregate((TLzCustomExpression)expression.rexpr);
                    return l_val || r_val;
            }
        }

        public CsvObject Evaluate(CellJoin cellJoin, RowJoin rowGroup, TLzCustomExpression expression)
        {
            if (expression == null)
                throw new QueryTextDriverException("Не передана ссылка на выражение агрегатора");
            switch (expression.oper)
            {
                case TLzOpType.Expr_AND:
                    CsvObject lval = Evaluate(cellJoin, rowGroup, (TLzCustomExpression)expression.lexpr);
                    if ((lval.GetType() == typeof(BoolObject)) && ((BoolObject)lval).Value() == false)
                        return lval;
                    CsvObject rval = Evaluate(cellJoin, rowGroup, (TLzCustomExpression)expression.rexpr);
                    if (rval.GetType() == typeof(BoolObject))
                        return rval;
                    else
                        return new BoolObject(false);
                case TLzOpType.Expr_OR:
                    lval = Evaluate(cellJoin, rowGroup, (TLzCustomExpression)expression.lexpr);
                    if ((lval.GetType() == typeof(BoolObject)) && ((BoolObject)lval).Value() == true)
                        return lval;
                    rval = Evaluate(cellJoin, rowGroup, (TLzCustomExpression)expression.rexpr);
                    if (rval.GetType() == typeof(bool))
                        return rval;
                    else
                        return new BoolObject(false);
                case TLzOpType.Expr_XOR:
                    lval = Evaluate(cellJoin, rowGroup, (TLzCustomExpression)expression.lexpr);
                    rval = Evaluate(cellJoin, rowGroup, (TLzCustomExpression)expression.rexpr);
                    if ((lval.GetType() == typeof(bool)) && (rval.GetType() == typeof(bool)))
                        return lval ^ rval;
                    else
                        return new BoolObject(false);
                case TLzOpType.Expr_NOT:
                    lval = Evaluate(cellJoin, rowGroup, (TLzCustomExpression)expression.rexpr);
                    if (lval.GetType() == typeof(BoolObject))
                        return !(BoolObject)lval;
                    else
                        return new BoolObject(false);
                case TLzOpType.Expr_Arithmetic:
                    return ArithmeticEval(cellJoin, rowGroup, expression);
                case TLzOpType.Expr_Attr: 
                    return GetAttr(cellJoin, expression);
                case TLzOpType.Expr_Comparison:
                    return ComparisonEval(cellJoin, rowGroup, expression);
                case TLzOpType.Expr_Parenthesis:
                    return Evaluate(cellJoin, rowGroup, (TLzCustomExpression)expression.lexpr);
                case TLzOpType.Expr_Const:
                    string value = expression.AsText;
                    if (QueryHelper.GetValueType(value) == typeof(string))
                        value = value.Trim(new char[] {'\''});
                    return CsvObject.Create(value, config.IgnoreDataTypes);
                case TLzOpType.Expr_In: ;
                    return InEval(cellJoin, rowGroup, expression);
                case TLzOpType.Expr_NotIn: ;
                    return !InEval(cellJoin, rowGroup, expression);
                case TLzOpType.Expr_Between:
                    return BetweenEval(cellJoin, rowGroup, expression);
                case TLzOpType.Expr_NotBetween:
                    return !BetweenEval(cellJoin, rowGroup, expression);
                case TLzOpType.Expr_BetweenTo:
                    lval = Evaluate(cellJoin, rowGroup, (TLzCustomExpression)expression.lexpr);
                    rval = Evaluate(cellJoin, rowGroup, (TLzCustomExpression)expression.rexpr);
                    return new RangeObject(lval, rval);
                case TLzOpType.Expr_Comma:
                    CsvCollection array = new CsvCollection();
                    array.Add(Evaluate(cellJoin, rowGroup, (TLzCustomExpression)expression.lexpr));
                    if (((TLzCustomExpression)expression.rexpr).oper == TLzOpType.Expr_Comma)
                        array.AddRange((CsvCollection)Evaluate(cellJoin, rowGroup, (TLzCustomExpression)expression.rexpr));
                    else
                        array.Add(Evaluate(cellJoin, rowGroup, (TLzCustomExpression)expression.rexpr));
                    return array;
                case TLzOpType.Expr_subquery:
                    QueryExecutor executor = new QueryExecutor(config);
                    TableJoin result = executor.Execute(expression.AsText);
                    if (result.Columns.Count != 1)
                    {
                        QueryTextDriverException exception = new QueryTextDriverException("Подзапрос {0} вернул больше одной колонки");
                        exception.Data.Add("{0}", expression.AsText.ToString());
                        throw exception;
                    }
                    array = new CsvCollection();
                    if (result.Columns[0].CellsCount == 0)
                    {
                        QueryTextDriverException exception = new QueryTextDriverException("Подзапрос {0} не вернул значение");
                        exception.Data.Add("{0}", expression.AsText.ToString());
                        throw exception;
                    }
                    if (result.Columns[0].CellsCount == 1)
                        return result.Columns[0].GetCell(0).Value;
                    for (int i = 0; i < result.Columns[0].CellsCount; i++)
                        array.Add(result.Columns[0].GetCell(i).Value);
                    return array;
                case TLzOpType.Expr_FuncCall:
                    return FuncEval(cellJoin, rowGroup, expression);
                case TLzOpType.Expr_Like:
                    return LikeEval(cellJoin, rowGroup, expression);
                case TLzOpType.Expr_NotLike:
                    return !LikeEval(cellJoin, rowGroup, expression);
                case TLzOpType.Expr_BitWise:
                    return BitWiseEval(cellJoin, rowGroup, expression);
                case TLzOpType.Expr_OP:
                    switch (expression.opname.AsText)
                    {
                        case "&":
                        case "|": return BitWiseEval(cellJoin, rowGroup, expression);
                        default:
                            QueryTextDriverException defaultOpException = new QueryTextDriverException("Неизвестное выражение {0}");
                            defaultOpException.Data.Add("{0}", expression.AsText.ToString());
                            throw defaultOpException;
                    }
                default:
                    QueryTextDriverException defaultException = new QueryTextDriverException("Неизвестное выражение {0}");
                    defaultException.Data.Add("{0}", expression.AsText.ToString());
                    throw defaultException;
            }
        }

        private CsvObject FuncMaxEval(CellJoin cellJoin, RowJoin rowGroup, TLz_FuncCall func, TLzOwnerLocation context)
        {
            if (func.args.Count() != 1)
            {
                QueryTextDriverException exception = new QueryTextDriverException("Функция {0} принимает один аргумент");
                exception.Data.Add("{0}", func.AsText);
                throw exception;
            }
            TLzCustomExpression expr = (TLzCustomExpression)func.args[0];
            if ((context == TLzOwnerLocation.elWhere) || (context == TLzOwnerLocation.elWhere_not_top_level))
                return Evaluate(cellJoin, rowGroup, expr);
            double Max = Double.MinValue;
            if (rowGroup.Rows.Count == 0)
                return Evaluate(cellJoin, rowGroup, expr);
            for (int i = 0; i < rowGroup.Rows.Count; i++)
            {
                RowClass row = rowGroup.Rows[i];
                CellJoin subCellJoin = new CellJoin();
                for (int j = 0; j < row.Cells.Count; j++)
                    subCellJoin.Cells.Add(row.Cells[j]);
                CsvObject lval = Evaluate(subCellJoin, rowGroup, expr);
                if (lval.AsDouble().Value() > Max)
                    Max = lval.AsDouble().Value();
            }
            return new DoubleObject(Max);
        }

        private CsvObject FuncMinEval(CellJoin cellJoin, RowJoin rowGroup, TLz_FuncCall func, TLzOwnerLocation context)
        {
            if (func.args.Count() != 1)
            {
                QueryTextDriverException exception = new QueryTextDriverException("Функция {0} принимает один аргумент");
                exception.Data.Add("{0}", func.AsText);
                throw exception;
            }
            TLzCustomExpression expr = (TLzCustomExpression)func.args[0];
            if ((context == TLzOwnerLocation.elWhere) || (context == TLzOwnerLocation.elWhere_not_top_level))
                return Evaluate(cellJoin, rowGroup, expr);
            double Min = Double.MaxValue;
            if (rowGroup.Rows.Count == 0)
                return Evaluate(cellJoin, rowGroup, expr);
            for (int i = 0; i < rowGroup.Rows.Count; i++)
            {
                RowClass row = rowGroup.Rows[i];
                CellJoin subCellJoin = new CellJoin();
                for (int j = 0; j < row.Cells.Count; j++)
                    subCellJoin.Cells.Add(row.Cells[j]);
                CsvObject lval = Evaluate(subCellJoin, rowGroup, expr);
                if (lval.AsDouble().Value() < Min)
                    Min = lval.AsDouble().Value();
            }
            return new DoubleObject(Min);
        }

        private CsvObject FuncAvgEval(CellJoin cellJoin, RowJoin rowGroup, TLz_FuncCall func, TLzOwnerLocation context)
        {
            if (func.args.Count() != 1)
            {
                QueryTextDriverException exception = new QueryTextDriverException("Функция {0} принимает один аргумент");
                exception.Data.Add("{0}", func.AsText);
                throw exception;
            }
            TLzCustomExpression expr = (TLzCustomExpression)func.args[0];
            if ((context == TLzOwnerLocation.elWhere) || (context == TLzOwnerLocation.elWhere_not_top_level))
                return Evaluate(cellJoin, rowGroup, expr);
            if (rowGroup.Rows.Count == 0)
                return Evaluate(cellJoin, rowGroup, expr);
            double Sum = 0;
            int Count = 0;
            for (int i = 0; i < rowGroup.Rows.Count; i++)
            {
                RowClass row = rowGroup.Rows[i];
                CellJoin subCellJoin = new CellJoin();
                for (int j = 0; j < row.Cells.Count; j++)
                    subCellJoin.Cells.Add(row.Cells[j]);
                CsvObject lval = Evaluate(subCellJoin, rowGroup, expr);
                Count++;
                Sum += lval.AsDouble().Value();
            }
            return new DoubleObject(Sum/Count);
        }

        private CsvObject FuncCountEval(CellJoin cellJoin, RowJoin rowGroup, TLz_FuncCall func, TLzOwnerLocation context)
        {
            if (func.args != null && func.args.Count() > 1)
            {
                QueryTextDriverException exception = new QueryTextDriverException("Функция {0} принимает один аргумент");
                exception.Data.Add("{0}", func.AsText);
                throw exception;
            }
            if ((context == TLzOwnerLocation.elWhere) || (context == TLzOwnerLocation.elWhere_not_top_level))
                return new IntObject(1);
            return new IntObject(rowGroup.Rows.Count);
        }

        private CsvObject FuncSumEval(CellJoin cellJoin, RowJoin rowGroup, TLz_FuncCall func, TLzOwnerLocation context)
        {
            if (func.args == null || func.args.Count() != 1)
            {
                QueryTextDriverException exception = new QueryTextDriverException("Функция {0} принимает один аргумент");
                exception.Data.Add("{0}", func.AsText);
                throw exception;
            }
            TLzCustomExpression expr = (TLzCustomExpression)func.args[0];
            if ((context == TLzOwnerLocation.elWhere) || (context == TLzOwnerLocation.elWhere_not_top_level))
                return Evaluate(cellJoin, rowGroup, expr);
            double Sum = 0;
            for (int i = 0; i < rowGroup.Rows.Count; i++)
            {
                RowClass row = rowGroup.Rows[i];
                CellJoin subCellJoin = new CellJoin();
                for (int j = 0; j < row.Cells.Count; j++)
                    subCellJoin.Cells.Add(row.Cells[j]);
                CsvObject lval = Evaluate(subCellJoin, rowGroup, expr);
                Sum += lval.AsDouble().Value();
            }
            return new DoubleObject(Sum);
        }

        private CsvObject FuncEval(CellJoin cellJoin, RowJoin rowGroup, TLzCustomExpression expression)
        {
            if (expression == null)
                throw new QueryTextDriverException("Не передана ссылка на выражение");
            TLz_FuncCall func = (TLz_FuncCall)expression.lexpr;
            switch (func.FunctionName.ToLower(CultureInfo.CurrentCulture))
            {
                case "max": return FuncMaxEval(cellJoin, rowGroup, func, expression.Location);
                case "min": return FuncMinEval(cellJoin, rowGroup, func, expression.Location);
                case "avg": return FuncAvgEval(cellJoin, rowGroup, func, expression.Location);
                case "count": return FuncCountEval(cellJoin, rowGroup, func, expression.Location);
                case "sum": return FuncSumEval(cellJoin, rowGroup, func, expression.Location);
                default:
                    QueryTextDriverException exception = new QueryTextDriverException("Попытка вызова неизвестной функции {0}");
                    exception.Data.Add("{0}", expression.lexpr.AsText);
                    throw exception;
            }
        }

        private BoolObject BetweenEval(CellJoin cellJoin, RowJoin rowGroup, TLzCustomExpression expression)
        {
            if (expression == null)
                throw new QueryTextDriverException("Не передана ссылка на выражение");
            CsvObject lval = Evaluate(cellJoin, rowGroup, (TLzCustomExpression)expression.lexpr);
            CsvObject rval = Evaluate(cellJoin, rowGroup, (TLzCustomExpression)expression.rexpr);
            RangeObject range = rval.AsRange();
            if (((object)range) != null)
                return lval.InRange(range);
            return new BoolObject(false);
        }


        public BoolObject LikeEval(CellJoin cellJoin, RowJoin rowGroup, TLzCustomExpression expression)
        {
            if (expression == null)
                throw new QueryTextDriverException("Не передана ссылка на выражение");
            StringObject lval = Evaluate(cellJoin, rowGroup, (TLzCustomExpression)expression.lexpr).AsString();
            StringObject rval = Evaluate(cellJoin, rowGroup, (TLzCustomExpression)expression.rexpr).AsString();
            rval = new StringObject(rval.Value().Replace("%", ".*"));
            if (Regex.IsMatch(lval.Value(), rval.Value()))
                return new BoolObject(true);
            else
                return new BoolObject(false);
        }

        private BoolObject InEval(CellJoin cellJoin, RowJoin rowGroup, TLzCustomExpression expression)
        {
            if (expression == null)
                throw new QueryTextDriverException("Не передана ссылка на выражение");
            CsvObject lval = Evaluate(cellJoin, rowGroup, (TLzCustomExpression)expression.lexpr);
            CsvObject rval = Evaluate(cellJoin, rowGroup, (TLzCustomExpression)expression.rexpr);
            CsvCollection array = rval.AsArray();
            if (((object)array) != null)
                foreach (CsvObject obj in array)
                    if (lval == obj)
                        return new BoolObject(true);
            return new BoolObject(false);
        }

        private CsvObject BitWiseEval(CellJoin cellJoin, RowJoin rowGroup, TLzCustomExpression expression)
        {
            if (expression == null)
                throw new QueryTextDriverException("Не передана ссылка на выражение");
            CsvObject lval = Evaluate(cellJoin, rowGroup, (TLzCustomExpression)expression.lexpr);
            CsvObject rval = Evaluate(cellJoin, rowGroup, (TLzCustomExpression)expression.rexpr);

            //Вычисляем
            if (expression.opname.AsText == "|")
                return lval | rval;
            if (expression.opname.AsText == "&")
                return lval & rval;
            if (expression.opname.AsText == "^")
                return lval ^ rval;
            QueryTextDriverException exception = new QueryTextDriverException("Некорректная побитовая операция {0}");
            exception.Data.Add("{0}", expression.AsText);
            throw exception;
        }

        private CsvObject ArithmeticEval(CellJoin cellJoin, RowJoin rowGroup, TLzCustomExpression expression)
        {
            if (expression == null)
                throw new QueryTextDriverException("Не передана ссылка на выражение");
            CsvObject lval = Evaluate(cellJoin, rowGroup, (TLzCustomExpression)expression.lexpr);
            CsvObject rval = Evaluate(cellJoin, rowGroup, (TLzCustomExpression)expression.rexpr);

            //Вычисляем
            if (expression.opname.AsText == "+")
                return lval + rval;
            if (expression.opname.AsText == "-")
                return lval - rval;
            if (expression.opname.AsText == "*")
                return lval * rval;
            if (expression.opname.AsText == "/")
                return lval / rval;
            if (expression.opname.AsText == "%")
                return lval % rval;
            //На самом деле это битовая операция, но синтаксический анализатор думает иначе
            if (expression.opname.AsText == "^")
                return lval ^ rval;
            QueryTextDriverException exception = new QueryTextDriverException("Некорректная арифметическая операция {0}");
            exception.Data.Add("{0}", expression.AsText);
            throw exception;
        }

        /// <summary>
        /// Сравнение двух элементов
        /// </summary>
        private BoolObject ComparisonEval(CellJoin cellJoin, RowJoin rowGroup, TLzCustomExpression expression)
        {
            if (expression == null)
                throw new QueryTextDriverException("Не передана ссылка на выражение");
            CsvObject lval = Evaluate(cellJoin, rowGroup, (TLzCustomExpression)expression.lexpr);
            CsvObject rval = Evaluate(cellJoin, rowGroup, (TLzCustomExpression)expression.rexpr);
            string oper = expression.opname.AsText;
            //Производим сравнение
            if (oper == "=")
                return lval == rval;
            if (oper == "<>")
                return lval != rval;
            if (oper == ">")
                return lval > rval;
            if (oper == "<")
                return lval < rval;
            if (oper == "<=")
                return lval <= rval;
            if (oper == ">=")
                return lval >= rval;
            if (oper == "LIKE")
                return LikeEval(cellJoin, rowGroup, expression);
            QueryTextDriverException not_found_exception = new QueryTextDriverException("Ошибка сравнения {0}");
            not_found_exception.Data.Add("{0}", expression.AsText);
            throw not_found_exception;
        }

        /// <summary>
        /// Получение значения атрибута
        /// </summary>
        private CsvObject GetAttr(CellJoin cellJoin, TLzCustomExpression expression)
        {
            if (expression == null)
                throw new QueryTextDriverException("Не передана ссылка на выражение");
            if (expression.AsText.ToLower(CultureInfo.CurrentCulture) == "true")
                return new BoolObject(true);
            if (expression.AsText.ToLower(CultureInfo.CurrentCulture) == "false")
                return new BoolObject(false);
            string TableName = ((TLz_Attr)expression.lexpr).Prefix;
            string ColumnName = ((TLz_Attr)expression.lexpr).ColumnNameToken.AsText;
            foreach (CellClass cell in cellJoin.Cells)
            {
                if ((cell.Column.ColumnName == ColumnName) &&
                    ((cell.Column.Table.TableName == TableName) || (cell.Column.Table.TableAlias == TableName) || String.IsNullOrEmpty(TableName)))
                    return cell.Value;
            }
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный атрибут {0}");
            exception.Data.Add("{0}", expression.lexpr.ToString());
            throw exception;
        }
    }
}
