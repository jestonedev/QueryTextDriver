using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using QueryTextDriverExceptionNS;

namespace DataTypes
{
    public class StringObject : CsvObject
    {
        public StringObject(object value)
        {
            this.value = System.Convert.ToString(value, CultureInfo.CurrentCulture);
        }

        public override IntObject AsInt()
        {
            long value = 0;
            Int64.TryParse((string)this.value, out value);
            return new IntObject(value);
        }

        public override BoolObject AsBool()
        {
            bool value = false;
            Boolean.TryParse((string)this.value, out value);
            return new BoolObject(value);
        }

        public override StringObject AsString()
        {
            return this;
        }

        public override DateTimeObject AsDateTime()
        {
            DateTime value = DateTime.MinValue;
            DateTime.TryParse((string)this.value, out value);
            return new DateTimeObject(value);
        }

        public override DoubleObject AsDouble()
        {
            double value = 0;
            Double.TryParse((string)this.value, out value);
            return new DoubleObject(value);
        }

        public new string Value()
        {
            return (string)value;
        }

        public static CsvObject operator +(StringObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                throw new QueryTextDriverException("В оператор \"+\" не передана ссылка на объект");
            return new StringObject(value1.Value() + value2.AsString().Value());
        }

        public static CsvObject operator -(StringObject value1, CsvObject value2)
        {
            return new IntObject(0);
        }

        public static CsvObject operator /(StringObject value1, CsvObject value2)
        {
            return new IntObject(0);
        }

        public static CsvObject operator *(StringObject value1, CsvObject value2)
        {
            return new IntObject(0);
        }

        public static CsvObject operator %(StringObject value1, CsvObject value2)
        {
            return new IntObject(0);
        }

        public static BoolObject operator >(StringObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                return new BoolObject(false);
            if (value2.GetType() == typeof(IntObject))
                return new BoolObject(String.Compare(value1.Value(),((IntObject)value2).AsString().Value(), StringComparison.CurrentCulture) > 0);
            if (value2.GetType() == typeof(DoubleObject))
                return new BoolObject(String.Compare(value1.Value(), ((DoubleObject)value2).AsString().Value(), StringComparison.CurrentCulture) > 0);
            if (value2.GetType() == typeof(DateTimeObject))
                return new BoolObject(String.Compare(value1.Value(), ((DateTimeObject)value2).AsString().Value(), StringComparison.CurrentCulture) > 0);
            if (value2.GetType() == typeof(BoolObject))
                return new BoolObject(String.Compare(value1.Value(), ((BoolObject)value2).AsString().Value(), StringComparison.CurrentCulture) > 0);
            if (value2.GetType() == typeof(StringObject))
                return new BoolObject(String.Compare(value1.Value(), ((StringObject)value2).Value(), StringComparison.CurrentCulture) > 0);
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static BoolObject operator <(StringObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                return new BoolObject(false);
            if (value2.GetType() == typeof(IntObject))
                return new BoolObject(String.Compare(value1.Value(), ((IntObject)value2).AsString().Value(), StringComparison.CurrentCulture) < 0);
            if (value2.GetType() == typeof(DoubleObject))
                return new BoolObject(String.Compare(value1.Value(), ((DoubleObject)value2).AsString().Value(), StringComparison.CurrentCulture) < 0);
            if (value2.GetType() == typeof(DateTimeObject))
                return new BoolObject(String.Compare(value1.Value(), ((DateTimeObject)value2).AsString().Value(), StringComparison.CurrentCulture) < 0);
            if (value2.GetType() == typeof(BoolObject))
                return new BoolObject(String.Compare(value1.Value(), ((BoolObject)value2).AsString().Value(), StringComparison.CurrentCulture) < 0);
            if (value2.GetType() == typeof(StringObject))
                return new BoolObject(String.Compare(value1.Value(), ((StringObject)value2).Value(), StringComparison.CurrentCulture) < 0);
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static BoolObject operator ==(StringObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                return new BoolObject(false);
            if (value2.GetType() == typeof(IntObject))
                return new BoolObject(String.Compare(value1.Value(), ((IntObject)value2).AsString().Value(), StringComparison.CurrentCulture) == 0);
            if (value2.GetType() == typeof(DoubleObject))
                return new BoolObject(String.Compare(value1.Value(), ((DoubleObject)value2).AsString().Value(), StringComparison.CurrentCulture) == 0);
            if (value2.GetType() == typeof(DateTimeObject))
                return new BoolObject(String.Compare(value1.Value(), ((DateTimeObject)value2).AsString().Value(), StringComparison.CurrentCulture) == 0);
            if (value2.GetType() == typeof(BoolObject))
                return new BoolObject(String.Compare(value1.Value(), ((BoolObject)value2).AsString().Value(), StringComparison.CurrentCulture) == 0);
            if (value2.GetType() == typeof(StringObject))
                return new BoolObject(String.Compare(value1.Value(), ((StringObject)value2).Value(), StringComparison.CurrentCulture) == 0);
            return new BoolObject(false);
        }

        public static BoolObject operator <=(StringObject value1, CsvObject value2)
        {
            return !(value1 > value2);
        }

        public static BoolObject operator >=(StringObject value1, CsvObject value2)
        {
            return !(value1 < value2);
        }

        public static BoolObject operator !=(StringObject value1, CsvObject value2)
        {
            return !(value1 == value2);
        }

        public static CsvObject Add(StringObject value1, CsvObject value2)
        {
            return value1 + value2;
        }

        public static CsvObject Subtract(StringObject value1, CsvObject value2)
        {
            return value1 - value2;
        }

        public static CsvObject Divide(StringObject value1, CsvObject value2)
        {
            return value1 / value2;
        }

        public static CsvObject Mod(StringObject value1, CsvObject value2)
        {
            return value1 % value2;
        }

        public static CsvObject Multiply(StringObject value1, CsvObject value2)
        {
            return value1 * value2;
        }

        public static int Compare(StringObject value1, CsvObject value2)
        {
            return value1 > value2 ? 1 : value1 < value2 ? -1 : 0;
        }

        public static CsvObject BitwiseAnd(StringObject value1, CsvObject value2)
        {
            return value1 & value2;
        }

        public static CsvObject BitwiseOr(StringObject value1, CsvObject value2)
        {
            return value1 | value2;
        }

        public static CsvObject Xor(StringObject value1, CsvObject value2)
        {
            return value1 ^ value2;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }
}
