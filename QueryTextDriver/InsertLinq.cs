using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gudusoft.gsqlparser;
using System.IO;
using QueryTextDriverExceptionNS;
using DataTypes;
using System.Collections.ObjectModel;

namespace QueryTextDriver
{
    public interface IInsertInto
    {
        int Values(TLz_List values);
        int Values(TableJoin values);
    }

    public class InsertLinq: IInsertInto
    {
        private QueryConfig config;
        private ExpressionEvaluator evaluator;
        private string fileName;

        //Вставленные строки
        private RowJoin InsertedRows = new RowJoin();

        public InsertLinq(QueryConfig config)
        {
            if (config == null)
                this.config = new QueryConfig(" ", Environment.NewLine, false, false);
            else
                this.config = new QueryConfig(config.ColumnSeparator, config.RowSeparator, config.FirstRowHeader, config.IgnoreDataTypes);
            this.evaluator = ExpressionEvaluator.CreateEvaluator(this.config);
        }

        public IInsertInto Into(TLzTable table)
        {
            if (table == null)
                throw new QueryTextDriverException("Не передана ссылка на таблицу");
            fileName = table.TableToken.AsText.Trim(new char[] { '"' });
            if (!File.Exists(fileName))
            {
                QueryTextDriverException exception = new QueryTextDriverException("Файл {0} не существует");
                exception.Data.Add("{0}", fileName);
                throw exception;
            }
            return this;
        }

        public int Values(TLz_List values)
        {
            if (values == null)
                throw new QueryTextDriverException("Не передана ссылка на список вставляемых значений");
            int rowAffected = 0;
            for (int i = 0; i < values.Count(); i++)
            {
                TLzFieldList valueList = (TLzFieldList)values[i];
                RowClass row = new RowClass();
                for (int j = 0; j < valueList.Count(); j++ )
                {
                    CellClass cell = new CellClass();
                    cell.Row = row;
                    cell.Column = new ColumnClass();
                    if (valueList[j].FieldExpr != null);
                       cell.Value = evaluator.Evaluate(new CellJoin(), new RowJoin(), valueList[j].FieldExpr);
                    row.Cells.Add(cell);
                }
                InsertedRows.Rows.Add(row);
            }
            SaveValuesToFile();
            return rowAffected;
        }

        public int Values(TableJoin values)
        {
            if (values == null)
                throw new QueryTextDriverException("Не передана ссылка на список вставляемых значений");
            for (int i = 0; i < values.Rows.Count; i++)
                InsertedRows.Rows.Add(values.Rows[i]);
            SaveValuesToFile();
            return values.Rows.Count;
        }

        private void SaveValuesToFile()
        {
            string csv = "";
            if (File.ReadAllBytes(fileName).Length > 0)
                csv += config.RowSeparator;
            for (int i = 0; i < InsertedRows.Rows.Count; i++)
            {
                for (int j = 0; j < InsertedRows.Rows[i].Cells.Count; j++)
                {
                    csv += InsertedRows.Rows[i].Cells[j].Value.AsString().Value();
                    if (j < (InsertedRows.Rows[i].Cells.Count - 1))
                        csv += config.ColumnSeparator;
                }
                if (i < (InsertedRows.Rows.Count - 1))
                    csv += config.RowSeparator;
            }
            File.AppendAllText(fileName, csv);
        }
    }
}
