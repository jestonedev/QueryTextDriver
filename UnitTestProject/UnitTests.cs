using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryTextDriver;
using System.IO;
using DataTypes;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            QueryExecutor executor = new QueryExecutor("|", "", false, false);
            TableJoin table = executor.Execute("SELECT * FROM \""+Path.Combine(Environment.CurrentDirectory, "csvs", "test1.csv")+"\"");
            Assert.AreEqual(table.Rows.Count, 6);
            Assert.AreEqual(table.Columns.Count, 5);
            Assert.AreEqual(table.Rows[2].Cells[0].Value.Value(), (Int64)3);
            Assert.AreEqual(table.Rows[3].Cells[2].Value.Value(), "text4");
        }

        [TestMethod]
        public void TestMethod2()
        {
            QueryExecutor executor = new QueryExecutor("|", "", false, false);
            TableJoin table = executor.Execute("SELECT COUNT(*) FROM \"" + Path.Combine(Environment.CurrentDirectory, "csvs", "test1.csv") + "\"");
            Assert.AreEqual(table.Rows.Count, 1);
            Assert.AreEqual(table.Columns.Count, 1);
            Assert.AreEqual(table.Rows[0].Cells[0].Value.Value(), (Int64)6);
        }

        [TestMethod]
        public void TestMethod3()
        {
            QueryExecutor executor = new QueryExecutor("|", "", false, false);
            TableJoin table = executor.Execute("SELECT COUNT(*), SUM(`1`) FROM \"" + Path.Combine(Environment.CurrentDirectory, "csvs", "test1.csv") + "\"");
            Assert.AreEqual(table.Rows.Count, 1);
            Assert.AreEqual(table.Columns.Count, 2);
            Assert.AreEqual(table.Rows[0].Cells[0].Value.Value(), (Int64)6);
            Assert.AreEqual(table.Rows[0].Cells[1].Value.Value(), (Double)21);
        }

        [TestMethod]
        public void TestMethod4()
        {
            QueryExecutor executor = new QueryExecutor("|", "", false, false);
            TableJoin table = executor.Execute("SELECT * FROM (SELECT COUNT(*), SUM(`1`) FROM \"" + Path.Combine(Environment.CurrentDirectory, "csvs", "test1.csv") + "\") v");
            Assert.AreEqual(table.Rows.Count, 1);
            Assert.AreEqual(table.Columns.Count, 2);
            Assert.AreEqual(table.Rows[0].Cells[0].Value.Value(), (Int64)6);
            Assert.AreEqual(table.Rows[0].Cells[1].Value.Value(), (Double)21);
        }

        [TestMethod]
        public void TestMethod5()
        {
            QueryExecutor executor = new QueryExecutor("|", "", false, false);
            TableJoin table = executor.Execute("SELECT * FROM \"" + Path.Combine(Environment.CurrentDirectory, "csvs", "test1.csv") + "\" WHERE `1`=1");
            Assert.AreEqual(table.Rows.Count, 1);
            Assert.AreEqual(table.Columns.Count, 5);
            Assert.AreEqual(table.Rows[0].Cells[0].Value.Value(), (Int64)1);
        }

        [TestMethod]
        public void TestMethod6()
        {
            QueryExecutor executor = new QueryExecutor("|", "", false, false);
            TableJoin table = executor.Execute("SELECT * FROM \"" + Path.Combine(Environment.CurrentDirectory, "csvs", "test1.csv") + "\" WHERE `1` IN (1,2)");
            Assert.AreEqual(table.Rows.Count, 2);
            Assert.AreEqual(table.Columns.Count, 5);
            Assert.AreEqual(table.Rows[0].Cells[0].Value.Value(), (Int64)1);
        }

        [TestMethod]
        public void TestMethod7()
        {
            QueryExecutor executor = new QueryExecutor("|", "", false, false);
            TableJoin table = executor.Execute("SELECT `1` AS col1, `3` AS col2 FROM \"" + Path.Combine(Environment.CurrentDirectory, "csvs", "test1.csv") + "\"");
            Assert.AreEqual(table.Rows.Count, 6);
            Assert.AreEqual(table.Columns.Count, 2);
            Assert.AreEqual(table.Columns[0].ColumnName, "col1");
            Assert.AreEqual(table.Columns[1].ColumnName, "col2");
        }

        [TestMethod]
        public void TestMethod8()
        {
            QueryExecutor executor = new QueryExecutor("|", "", false, false);
            TableJoin table = executor.Execute("SELECT * FROM \"" + Path.Combine(Environment.CurrentDirectory, "csvs", "test1.csv") + "\" a" +
                    " INNER JOIN \"" + Path.Combine(Environment.CurrentDirectory, "csvs", "test2.csv") + "\" b ON (a.`1` = b.`1`)");
            Assert.AreEqual(table.Rows.Count, 4);
            Assert.AreEqual(table.Columns.Count, 7);
            Assert.AreEqual(table.Rows[0].Cells[0].Value.Value(), (Int64)1);
            Assert.AreEqual(table.Rows[0].Cells[5].Value.Value(), (Int64)1);
            Assert.AreEqual(table.Rows[3].Cells[0].Value.Value(), (Int64)4);
            Assert.AreEqual(table.Rows[3].Cells[5].Value.Value(), (Int64)4);
        }

        [TestMethod]
        public void TestMethod9()
        {
            QueryExecutor executor = new QueryExecutor("|", "", false, false);
            TableJoin table = executor.Execute("SELECT * FROM \"" + Path.Combine(Environment.CurrentDirectory, "csvs", "test1.csv") + "\" a" +
                    " LEFT JOIN \"" + Path.Combine(Environment.CurrentDirectory, "csvs", "test2.csv") + "\" b ON (a.`1` = b.`1`)");
            Assert.AreEqual(table.Rows.Count, 6);
            Assert.AreEqual(table.Columns.Count, 7);
            Assert.AreEqual(table.Rows[0].Cells[0].Value.Value(), (Int64)1);
            Assert.AreEqual(table.Rows[0].Cells[5].Value.Value(), "1");
            Assert.AreEqual(table.Rows[4].Cells[0].Value.Value(), (Int64)5);
            Assert.AreEqual(table.Rows[4].Cells[5].Value.Value(), "");
        }

        [TestMethod]
        public void TestMethod10()
        {
            QueryExecutor executor = new QueryExecutor("|", "", false, false);
            TableJoin table = executor.Execute("SELECT * FROM \"" + Path.Combine(Environment.CurrentDirectory, "csvs", "test1.csv") + "\" a" +
                    " RIGHT JOIN \"" + Path.Combine(Environment.CurrentDirectory, "csvs", "test2.csv") + "\" b ON (a.`1` = b.`1`)");
            Assert.AreEqual(table.Rows.Count, 4);
            Assert.AreEqual(table.Columns.Count, 7);
            Assert.AreEqual(table.Rows[0].Cells[5].Value.Value(), (Int64)1);
            Assert.AreEqual(table.Rows[0].Cells[0].Value.Value(), (Int64)1);
            Assert.AreEqual(table.Rows[3].Cells[5].Value.Value(), (Int64)4);
            Assert.AreEqual(table.Rows[3].Cells[0].Value.Value(), (Int64)4);
        }

        [TestMethod]
        public void TestMethod11()
        {
            QueryExecutor executor = new QueryExecutor("|", "", false, false);
            TableJoin table = executor.Execute("SELECT v.`1` FROM \"" + Path.Combine(Environment.CurrentDirectory, "csvs", "test1.csv") + "\" v WHERE `5` = 3");
            Assert.AreEqual(table.Rows.Count, 3);
            Assert.AreEqual(table.Columns.Count, 1);
            Assert.AreEqual(table.Rows[0].Cells[0].Value.Value(), (Int64)1);
            Assert.AreEqual(table.Rows[1].Cells[0].Value.Value(), (Int64)2);
            Assert.AreEqual(table.Rows[2].Cells[0].Value.Value(), (Int64)3);
        }

        [TestMethod]
        public void TestMethod12()
        {
            QueryExecutor executor = new QueryExecutor("|", "", false, false);
            TableJoin table = executor.Execute("SELECT v.`1` FROM \"" + Path.Combine(Environment.CurrentDirectory, "csvs", "test1.csv") + "\" v GROUP BY `2`");
            Assert.AreEqual(table.Rows.Count, 3);
            Assert.AreEqual(table.Columns.Count, 1);
            Assert.AreEqual(table.Rows[0].Cells[0].Value.Value(), (Int64)1);
            Assert.AreEqual(table.Rows[1].Cells[0].Value.Value(), (Int64)3);
            Assert.AreEqual(table.Rows[2].Cells[0].Value.Value(), (Int64)5);
        }

        [TestMethod]
        public void TestMethod13()
        {
            QueryExecutor executor = new QueryExecutor("|", "", false, false);
            TableJoin table = executor.Execute("SELECT COUNT(*), SUM(`1`) AS col2 FROM \"" + Path.Combine(Environment.CurrentDirectory, "csvs", "test1.csv") + "\" v GROUP BY `5`");
            Assert.AreEqual(table.Rows.Count, 2);
            Assert.AreEqual(table.Columns.Count, 2);
            Assert.AreEqual(table.Rows[0].Cells[0].Value.Value(), (Int64)3); 
            Assert.AreEqual(table.Rows[0].Cells[1].Value.Value(), (Double)6);
            Assert.AreEqual(table.Rows[1].Cells[0].Value.Value(), (Int64)3);
            Assert.AreEqual(table.Rows[1].Cells[1].Value.Value(), (Double)15);
        }

        [TestMethod]
        public void TestMethod14()
        {
            QueryExecutor executor = new QueryExecutor("|", "", false, false);
            TableJoin table = executor.Execute("SELECT * FROM \"" + Path.Combine(Environment.CurrentDirectory, "csvs", "test1.csv") + "\" v ORDER BY `1` DESC");
            Assert.AreEqual(table.Rows.Count, 6);
            Assert.AreEqual(table.Columns.Count, 5);
            Assert.AreEqual(table.Rows[0].Cells[0].Value.Value(), (Int64)6);
            Assert.AreEqual(table.Rows[1].Cells[0].Value.Value(), (Int64)5);
            Assert.AreEqual(table.Rows[2].Cells[0].Value.Value(), (Int64)4);
            Assert.AreEqual(table.Rows[3].Cells[0].Value.Value(), (Int64)3);
        }

        [TestMethod]
        public void TestMethod15()
        {
            QueryExecutor executor = new QueryExecutor("|", "", false, false);
            TableJoin table = executor.Execute("SELECT `5`,`2` FROM \"" + Path.Combine(Environment.CurrentDirectory, "csvs", "test1.csv") + "\" v GROUP BY `5`,`2`");
            Assert.AreEqual(table.Rows.Count, 4);
            Assert.AreEqual(table.Columns.Count, 2);
            Assert.AreEqual(table.Rows[0].Cells[1].Value.Value(), (Int64)2);
            Assert.AreEqual(table.Rows[0].Cells[0].Value.Value(), (Int64)3);
            Assert.AreEqual(table.Rows[1].Cells[1].Value.Value(), (Int64)3);
            Assert.AreEqual(table.Rows[1].Cells[0].Value.Value(), (Int64)3);
            Assert.AreEqual(table.Rows[2].Cells[1].Value.Value(), (Int64)3);
            Assert.AreEqual(table.Rows[2].Cells[0].Value.Value(), (Int64)4);
            Assert.AreEqual(table.Rows[3].Cells[1].Value.Value(), (Int64)4);
            Assert.AreEqual(table.Rows[3].Cells[0].Value.Value(), (Int64)4);
        }

        [TestMethod]
        public void TestMethod16()
        {
            QueryExecutor executor = new QueryExecutor("|", "", false, false);
            TableJoin table = executor.Execute("SELECT `5`,`2` FROM \"" + Path.Combine(Environment.CurrentDirectory, "csvs", "test1.csv") + "\" v GROUP BY `5`,`2` ORDER BY `1` DESC");
            Assert.AreEqual(table.Rows.Count, 4);
            Assert.AreEqual(table.Columns.Count, 2);
            Assert.AreEqual(table.Rows[0].Cells[1].Value.Value(), (Int64)4);
            Assert.AreEqual(table.Rows[0].Cells[0].Value.Value(), (Int64)4);
            Assert.AreEqual(table.Rows[1].Cells[1].Value.Value(), (Int64)3);
            Assert.AreEqual(table.Rows[1].Cells[0].Value.Value(), (Int64)4);
            Assert.AreEqual(table.Rows[2].Cells[1].Value.Value(), (Int64)3);
            Assert.AreEqual(table.Rows[2].Cells[0].Value.Value(), (Int64)3);
            Assert.AreEqual(table.Rows[3].Cells[1].Value.Value(), (Int64)2);
            Assert.AreEqual(table.Rows[3].Cells[0].Value.Value(), (Int64)3);
        }

        [TestMethod]
        public void TestMethod17()
        {
            QueryExecutor executor = new QueryExecutor("|", "", false, false);
            TableJoin table = executor.Execute("SELECT `5`,`2` FROM \"" + Path.Combine(Environment.CurrentDirectory, "csvs", "test1.csv") + "\" v GROUP BY `5`,`2` ORDER BY `5` DESC");
            Assert.AreEqual(table.Rows.Count, 4);
            Assert.AreEqual(table.Columns.Count, 2);
            Assert.AreEqual(table.Rows[0].Cells[1].Value.Value(), (Int64)3);
            Assert.AreEqual(table.Rows[0].Cells[0].Value.Value(), (Int64)4);
            Assert.AreEqual(table.Rows[1].Cells[1].Value.Value(), (Int64)4);
            Assert.AreEqual(table.Rows[1].Cells[0].Value.Value(), (Int64)4);
            Assert.AreEqual(table.Rows[2].Cells[1].Value.Value(), (Int64)2);
            Assert.AreEqual(table.Rows[2].Cells[0].Value.Value(), (Int64)3);
            Assert.AreEqual(table.Rows[3].Cells[1].Value.Value(), (Int64)3);
            Assert.AreEqual(table.Rows[3].Cells[0].Value.Value(), (Int64)3);
        }

        [TestMethod]
        public void TestMethod18()
        {
            QueryExecutor executor = new QueryExecutor("|", "", false, false);
            TableJoin table = executor.Execute("SELECT * FROM \"" + Path.Combine(Environment.CurrentDirectory, "csvs", "test1.csv") + "\" v GROUP BY `5` HAVING SUM(`5`) > 9");
            Assert.AreEqual(table.Rows.Count, 1);
            Assert.AreEqual(table.Columns.Count, 5);
            Assert.AreEqual(table.Rows[0].Cells[0].Value.Value(), (Int64)4);
            Assert.AreEqual(table.Rows[0].Cells[1].Value.Value(), (Int64)3);
            Assert.AreEqual(table.Rows[0].Cells[4].Value.Value(), (Int64)4);
        }

        [TestMethod]
        public void TestMethod19()
        {
            QueryExecutor executor = new QueryExecutor("|", "", false, false);
            TableJoin table = executor.Execute("SELECT * FROM \"" + Path.Combine(Environment.CurrentDirectory, "csvs", "test1.csv") + "\" v WHERE `5` IN (SELECT 4 UNION SELECT 3)");
            Assert.AreEqual(table.Rows.Count, 6);
            Assert.AreEqual(table.Columns.Count, 5);
            Assert.AreEqual(table.Rows[3].Cells[0].Value.Value(), (Int64)4);
            Assert.AreEqual(table.Rows[3].Cells[1].Value.Value(), (Int64)3);
            Assert.AreEqual(table.Rows[3].Cells[4].Value.Value(), (Int64)4);
        }

        [TestMethod]
        public void TestMethod20()
        {
            QueryExecutor executor = new QueryExecutor("|", "", false, false);
            TableJoin table = executor.Execute("SELECT * FROM \"" + Path.Combine(Environment.CurrentDirectory, "csvs", "test1.csv") + "\" v WHERE `5` IN (SELECT 4 UNION SELECT 3) LIMIT 1,4");
            Assert.AreEqual(table.Rows.Count, 4);
            Assert.AreEqual(table.Columns.Count, 5);
            Assert.AreEqual(table.Rows[3].Cells[0].Value.Value(), (Int64)4);
            Assert.AreEqual(table.Rows[3].Cells[1].Value.Value(), (Int64)3);
            Assert.AreEqual(table.Rows[3].Cells[4].Value.Value(), (Int64)4);
        }

        [TestMethod]
        public void TestMethod21()
        {
            QueryExecutor executor = new QueryExecutor("|", "", false, false);
            TableJoin table = executor.Execute("SELECT * FROM \"" + Path.Combine(Environment.CurrentDirectory, "csvs", "test1.csv") + "\" v WHERE `5` IN (SELECT 4 UNION SELECT 3) LIMIT 1");
            Assert.AreEqual(table.Rows.Count, 1);
            Assert.AreEqual(table.Columns.Count, 5);
            Assert.AreEqual(table.Rows[0].Cells[0].Value.Value(), (Int64)1);
            Assert.AreEqual(table.Rows[0].Cells[1].Value.Value(), (Int64)2);
            Assert.AreEqual(table.Rows[0].Cells[4].Value.Value(), (Int64)3);
        }

        [TestMethod]
        public void TestMethod22()
        {
            QueryExecutor executor = new QueryExecutor("|", "", false, false);
            TableJoin table = executor.Execute("SELECT SUM(`2`) FROM \"" + Path.Combine(Environment.CurrentDirectory, "csvs", "test1.csv") + "\" v GROUP BY `2` HAVING `2` >= 3");
            Assert.AreEqual(table.Rows.Count, 2);
            Assert.AreEqual(table.Columns.Count, 1);
            Assert.AreEqual(table.Rows[0].Cells[0].Value.Value(), (Double)6);
            Assert.AreEqual(table.Rows[1].Cells[0].Value.Value(), (Double)8);
        }

        [TestMethod]
        public void TestMethod23()
        {
            QueryExecutor executor = new QueryExecutor("|", "", false, false);
            TableJoin table = executor.Execute("SELECT SUM(`2`), AVG(`5`) FROM \"" + Path.Combine(Environment.CurrentDirectory, "csvs", "test1.csv") + "\" v GROUP BY `2` HAVING SUM(`5`) <= 7 ");
            Assert.AreEqual(table.Rows.Count, 2);
            Assert.AreEqual(table.Columns.Count, 2);
            Assert.AreEqual(table.Rows[0].Cells[0].Value.Value(), (Double)4);
            Assert.AreEqual(table.Rows[1].Cells[0].Value.Value(), (Double)6);
            Assert.AreEqual(table.Rows[0].Cells[1].Value.Value(), (Double)3);
            Assert.AreEqual(table.Rows[1].Cells[1].Value.Value(), (Double)3.5);
        }

        [TestMethod]
        public void TestMethod24()
        {
            QueryExecutor executor = new QueryExecutor("|", "", false, false);
            TableJoin table = executor.Execute("SELECT a.*, b.* FROM \"" + Path.Combine(Environment.CurrentDirectory, "csvs", "test1.csv") + "\" a" +
                    ", \"" + Path.Combine(Environment.CurrentDirectory, "csvs", "test2.csv") + "\" b");
            Assert.AreEqual(table.Rows.Count, 24);
            Assert.AreEqual(table.Columns.Count, 7);
        }
    }
}
