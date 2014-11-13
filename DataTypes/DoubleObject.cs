using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using QueryTextDriverExceptionNS;

namespace DataTypes
{
    public class DoubleObject : CsvObject
    {
        public DoubleObject(object value)
        {
            this.value = System.Convert.ToDouble(value, CultureInfo.CurrentCulture); ;
        }

        public override BoolObject AsBool()
        {
            if ((double)value == 0)
                return new BoolObject(false);
            else
                return new BoolObject(true);
        }

        public override IntObject AsInt()
        {
            return new IntObject(System.Convert.ToInt32((double)value));
        }

        public override StringObject AsString()
        {
            return new StringObject(value.ToString());
        }

        public override DoubleObject AsDouble()
        {
            return this;
        }

        public override DateTimeObject AsDateTime()
        {
            return new DateTimeObject(System.Convert.ToDateTime((DateTime)value));
        }

        public static CsvObject operator +(DoubleObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                throw new QueryTextDriverException("В оператор \"+\" не передана ссылка на объект");
            if (value2.GetType() == typeof(IntObject))
                return new DoubleObject(value1.Value() + ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new DoubleObject(value1.Value() + ((DoubleObject)value2).Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.Value() + ((DateTimeObject)value2).AsDouble().Value()));
            if (value2.GetType() == typeof(BoolObject))
                return new DoubleObject(value1.Value() + ((BoolObject)value2).AsDouble().Value());
            if (value2.GetType() == typeof(StringObject))
                return new StringObject(value1.AsString().Value() + ((StringObject)value2).Value());
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static CsvObject operator -(DoubleObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                throw new QueryTextDriverException("В оператор \"-\" не передана ссылка на объект");
            if (value2.GetType() == typeof(IntObject))
                return new DoubleObject(value1.Value() - ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new DoubleObject(value1.Value() - ((DoubleObject)value2).Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.Value() - ((DateTimeObject)value2).AsDouble().Value()));
            if (value2.GetType() == typeof(BoolObject))
                return new DoubleObject(value1.Value() - ((BoolObject)value2).AsDouble().Value());
            if (value2.GetType() == typeof(StringObject))
                return new DoubleObject(0);
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static CsvObject operator /(DoubleObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                throw new QueryTextDriverException("В оператор \"/\" не передана ссылка на объект");
            if (value2.GetType() == typeof(IntObject))
                return new DoubleObject(value1.Value() / ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new DoubleObject(value1.Value() / ((DoubleObject)value2).Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.Value() / ((DateTimeObject)value2).AsDouble().Value()));
            if (value2.GetType() == typeof(BoolObject))
                return new DoubleObject(value1.Value() / ((BoolObject)value2).AsDouble().Value());
            if (value2.GetType() == typeof(StringObject))
                return new DoubleObject(0);
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static CsvObject operator *(DoubleObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                throw new QueryTextDriverException("В оператор \"*\" не передана ссылка на объект");
            if (value2.GetType() == typeof(IntObject))
                return new DoubleObject(value1.Value() * ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new DoubleObject(value1.Value() * ((DoubleObject)value2).Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.Value() * ((DateTimeObject)value2).AsDouble().Value()));
            if (value2.GetType() == typeof(BoolObject))
                return new DoubleObject(value1.Value() * ((BoolObject)value2).AsDouble().Value());
            if (value2.GetType() == typeof(StringObject))
                return new DoubleObject(0);
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static CsvObject operator %(DoubleObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                throw new QueryTextDriverException("В оператор \"%\" не передана ссылка на объект");
            if (value2.GetType() == typeof(IntObject))
                return new DoubleObject(value1.Value() % ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new DoubleObject(value1.Value() % ((DoubleObject)value2).Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.Value() % ((DateTimeObject)value2).AsDouble().Value()));
            if (value2.GetType() == typeof(BoolObject))
                return new DoubleObject(value1.Value() % ((BoolObject)value2).AsDouble().Value());
            if (value2.GetType() == typeof(StringObject))
                return new DoubleObject(0);
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static BoolObject operator >(DoubleObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                return new BoolObject(false);
            if (value2.GetType() == typeof(IntObject))
                return new BoolObject(value1.Value() > ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new BoolObject(value1.Value() > ((DoubleObject)value2).Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new BoolObject(value1.Value() > ((DateTimeObject)value2).AsDouble().Value());
            if (value2.GetType() == typeof(BoolObject))
                return new BoolObject(value1.Value() > ((BoolObject)value2).AsDouble().Value());
            if (value2.GetType() == typeof(StringObject))
                return new BoolObject(value1.Value() > ((StringObject)value2).AsDouble().Value());
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static BoolObject operator <(DoubleObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                return new BoolObject(false);
            if (value2.GetType() == typeof(IntObject))
                return new BoolObject(value1.Value() < ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new BoolObject(value1.Value() < ((DoubleObject)value2).Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new BoolObject(value1.Value() < ((DateTimeObject)value2).AsDouble().Value());
            if (value2.GetType() == typeof(BoolObject))
                return new BoolObject(value1.Value() < ((BoolObject)value2).AsDouble().Value());
            if (value2.GetType() == typeof(StringObject))
                return new BoolObject(value1.Value() < ((StringObject)value2).AsDouble().Value());
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static BoolObject operator ==(DoubleObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                return new BoolObject(false);
            if (value2.GetType() == typeof(IntObject))
                return new BoolObject(value1.Value() == ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new BoolObject(value1.Value() == ((DoubleObject)value2).Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new BoolObject(value1.Value() == ((DateTimeObject)value2).AsDouble().Value());
            if (value2.GetType() == typeof(BoolObject))
                return new BoolObject(value1.Value() == ((BoolObject)value2).AsDouble().Value());
            if (value2.GetType() == typeof(StringObject))
                return new BoolObject(value1.Value() == ((StringObject)value2).AsDouble().Value());
            return new BoolObject(false);
        }

        public static BoolObject operator <=(DoubleObject value1, CsvObject value2)
        {
            return !(value1 > value2);
        }

        public static BoolObject operator >=(DoubleObject value1, CsvObject value2)
        {
            return !(value1 < value2);
        }

        public static BoolObject operator !=(DoubleObject value1, CsvObject value2)
        {
            return !(value1 == value2);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static CsvObject operator |(DoubleObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                throw new QueryTextDriverException("В оператор \"|\" не передана ссылка на объект");
            if (value2.GetType() == typeof(IntObject))
                return new IntObject(value1.AsInt().Value() | ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new IntObject(value1.AsInt().Value() | ((DoubleObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new IntObject(value1.AsInt().Value() | ((DateTimeObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(BoolObject))
                return new IntObject(value1.AsInt().Value() | ((BoolObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(StringObject))
                return new IntObject(value1.AsInt().Value() | ((StringObject)value2).AsInt().Value());
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static CsvObject operator &(DoubleObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                throw new QueryTextDriverException("В оператор \"&\" не передана ссылка на объект");
            if (value2.GetType() == typeof(IntObject))
                return new IntObject(value1.AsInt().Value() & ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new IntObject(value1.AsInt().Value() & ((DoubleObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new IntObject(value1.AsInt().Value() & ((DateTimeObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(BoolObject))
                return new IntObject(value1.AsInt().Value() & ((BoolObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(StringObject))
                return new IntObject(value1.AsInt().Value() & ((StringObject)value2).AsInt().Value());
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static CsvObject operator ^(DoubleObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                throw new QueryTextDriverException("В оператор \"^\" не передана ссылка на объект");
            if (value2.GetType() == typeof(IntObject))
                return new IntObject(value1.AsInt().Value() ^ ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new IntObject(value1.AsInt().Value() ^ ((DoubleObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new IntObject(value1.AsInt().Value() ^ ((DateTimeObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(BoolObject))
                return new IntObject(value1.AsInt().Value() ^ ((BoolObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(StringObject))
                return new IntObject(value1.AsInt().Value() ^ ((StringObject)value2).AsInt().Value());
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static CsvObject Add(DoubleObject value1, CsvObject value2)
        {
            return value1 + value2;
        }

        public static CsvObject Subtract(DoubleObject value1, CsvObject value2)
        {
            return value1 - value2;
        }

        public static CsvObject Divide(DoubleObject value1, CsvObject value2)
        {
            return value1 / value2;
        }

        public static CsvObject Mod(DoubleObject value1, CsvObject value2)
        {
            return value1 % value2;
        }

        public static CsvObject Multiply(DoubleObject value1, CsvObject value2)
        {
            return value1 * value2;
        }

        public static int Compare(DoubleObject value1, CsvObject value2)
        {
            return value1 > value2 ? 1 : value1 < value2 ? -1 : 0;
        }

        public static CsvObject BitwiseAnd(DoubleObject value1, CsvObject value2)
        {
            return value1 & value2;
        }

        public static CsvObject BitwiseOr(DoubleObject value1, CsvObject value2)
        {
            return value1 | value2;
        }

        public static CsvObject Xor(DoubleObject value1, CsvObject value2)
        {
            return value1 ^ value2;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public new double Value()
        {
            return (double)value;
        }
    }
}
