using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gudusoft.gsqlparser;

namespace QueryTextDriver
{
    public class QueryHelper
    {
        private QueryHelper()
        {
        }

        //Метод определяет тип данных в соответствии с приоритетом
        public static Type GetValueType(object value)
        {
            int result;
            double result2;
            DateTime result3;
            bool result4;
            if (int.TryParse(value.ToString(), out result))
                return typeof(int);
            else
                if (double.TryParse(value.ToString(), out result2))
                    return typeof(double);
                else
                    if (DateTime.TryParse(value.ToString(), out result3))
                        return typeof(DateTime);
                    else
                        if (bool.TryParse(value.ToString(), out result4))
                            return typeof(bool);
                        else return typeof(string);
        }
    }
}
