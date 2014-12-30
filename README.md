QueryTextDriver
===============

.Net движок для работы с CSV-файлами через SQL-запросы

Требования
===============
.Net Framework 3.5 или выше

Лицензия
===============
GPL

Примеры
===============
Пример HAVING:
```c#
QueryExecutor executor = new QueryExecutor("|", "", false, false);
TableJoin table = executor.Execute("SELECT SUM(`2`), AVG(`5`) FROM \"" + 
    Path.Combine(Environment.CurrentDirectory, "csvs", "test1.csv") + 
    "\" v GROUP BY `2` HAVING SUM(`5`) <= 7 ");
```
Пример IN ():
```c#
QueryExecutor executor = new QueryExecutor("|", "", false, false);
TableJoin table = executor.Execute("SELECT * FROM \"" + 
    Path.Combine(Environment.CurrentDirectory, "csvs", "test1.csv") + 
    "\" WHERE `1` IN (1,2)");
```
Пример JOIN:
```c#
QueryExecutor executor = new QueryExecutor("|", "", false, false);
TableJoin table = executor.Execute("SELECT * FROM \"" + 
    Path.Combine(Environment.CurrentDirectory, "csvs", "test1.csv") + 
    "\" a" + " INNER JOIN \"" + 
    Path.Combine(Environment.CurrentDirectory, "csvs", "test2.csv") + 
    "\" b ON (a.`1` = b.`1`)");
 ```
Пример ORDER BY:
```c#
QueryExecutor executor = new QueryExecutor("|", "", false, false);
TableJoin table = executor.Execute("SELECT * FROM \"" + 
    Path.Combine(Environment.CurrentDirectory, "csvs", "test1.csv") + 
    "\" v ORDER BY `1` DESC");
 ```
 Пример LIMIT:
 ```c#
 QueryExecutor executor = new QueryExecutor("|", "", false, false);
TableJoin table = executor.Execute("SELECT * FROM \"" + 
   Path.Combine(Environment.CurrentDirectory, "csvs", "test1.csv") + 
   "\" v WHERE `5` IN (SELECT 4 UNION SELECT 3) LIMIT 1,4");
 ```
Ссылка на nuget-пакет
===============
https://www.nuget.org/packages/QueryTextDriver/
