using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using QueryTextDriver;
//VALUES(1,'','text'),(4,5,6)
namespace TestProject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            QueryExecutor executor = new QueryExecutor(",", "", false);
            executor.Execute("INSERT INTO \"C:\\Users\\IgnVV\\Desktop\\1.csv\" SELECT * FROM \"C:\\Users\\IgnVV\\Desktop\\1.csv\"");
        }
    }
}
