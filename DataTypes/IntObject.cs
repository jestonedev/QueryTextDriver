using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using QueryTextDriverExceptionNS;

namespace DataTypes
{
    public class IntObject : CsvObject
    {
        public IntObject(object value)
        {
            this.value = System.Convert.ToInt64(value, CultureInfo.CurrentCulture);
        }

        public override IntObject AsInt()
        {
            return this;
        }

        public override BoolObject AsBool()
        {
            if ((long)value == 0)
                return new BoolObject(false);
            else
                return new BoolObject(true);
        }

        public override DoubleObject AsDouble()
        {
            return new DoubleObject(System.Convert.ToDouble((long)value));
        }

        public override DateTimeObject AsDateTime()
        {
            return new DateTimeObject(System.Convert.ToDateTime((long)this.value));
        }

        public override StringObject AsString()
        {
            return new StringObject(value.ToString());
        }

        public static CsvObject operator +(IntObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                throw new QueryTextDriverException("В оператор \"+\" не передана ссылка на объект");
            if (value2.GetType() == typeof(IntObject))
                return new IntObject(value1.Value() + ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new DoubleObject(value1.Value() + ((DoubleObject)value2).Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.Value() + ((DateTimeObject)value2).AsInt().Value()));
            if (value2.GetType() == typeof(BoolObject))
                return new IntObject(value1.Value() + ((BoolObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(StringObject))
                return new StringObject(value1.AsString().Value() + ((StringObject)value2).Value());
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static CsvObject operator -(IntObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                throw new QueryTextDriverException("В оператор \"-\" не передана ссылка на объект");
            if (value2.GetType() == typeof(IntObject))
                return new IntObject(value1.Value() - ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new DoubleObject(value1.Value() - ((DoubleObject)value2).Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.Value() - ((DateTimeObject)value2).AsInt().Value()));
            if (value2.GetType() == typeof(BoolObject))
                return new IntObject(value1.Value() - ((BoolObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(StringObject))
                return new IntObject(0);
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static CsvObject operator /(IntObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                throw new QueryTextDriverException("В оператор \"/\" не передана ссылка на объект");
            if (value2.GetType() == typeof(IntObject))
                return new IntObject(value1.Value() / ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new DoubleObject(value1.Value() / ((DoubleObject)value2).Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.Value() / ((DateTimeObject)value2).AsInt().Value()));
            if (value2.GetType() == typeof(BoolObject))
                return new IntObject(value1.Value() / ((BoolObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(StringObject))
                return new IntObject(0);
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static CsvObject operator *(IntObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                throw new QueryTextDriverException("В оператор \"*\" не передана ссылка на объект");
            if (value2.GetType() == typeof(IntObject))
                return new IntObject(value1.Value() * ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new DoubleObject(value1.Value() * ((DoubleObject)value2).Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.Value() * ((DateTimeObject)value2).AsInt().Value()));
            if (value2.GetType() == typeof(BoolObject))
                return new IntObject(value1.Value() * ((BoolObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(StringObject))
                return new IntObject(0);
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static CsvObject operator %(IntObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                throw new QueryTextDriverException("В оператор \"%\" не передана ссылка на объект");
            if (value2.GetType() == typeof(IntObject))
                return new IntObject(value1.Value() % ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new DoubleObject(value1.Value() % ((DoubleObject)value2).Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.Value() % ((DateTimeObject)value2).AsInt().Value()));
            if (value2.GetType() == typeof(BoolObject))
                return new IntObject(value1.Value() % ((BoolObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(StringObject))
                return new IntObject(0);
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static BoolObject operator >(IntObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                return new BoolObject(false);
            if (value2.GetType() == typeof(IntObject))
                return new BoolObject(value1.Value() > ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new BoolObject(value1.Value() > ((DoubleObject)value2).Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new BoolObject(value1.Value() > ((DateTimeObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(BoolObject))
                return new BoolObject(value1.Value() > ((BoolObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(StringObject))
                return new BoolObject(value1.Value() > ((StringObject)value2).AsInt().Value());
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static BoolObject operator <(IntObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                return new BoolObject(false);
            if (value2.GetType() == typeof(IntObject))
                return new BoolObject(value1.Value() < ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new BoolObject(value1.Value() < ((DoubleObject)value2).Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new BoolObject(value1.Value() < ((DateTimeObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(BoolObject))
                return new BoolObject(value1.Value() < ((BoolObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(StringObject))
                return new BoolObject(value1.Value() < ((StringObject)value2).AsInt().Value());
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static BoolObject operator ==(IntObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                return new BoolObject(false);
            if (value2.GetType() == typeof(IntObject))
                return new BoolObject(value1.Value() == ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new BoolObject(value1.Value() == ((DoubleObject)value2).Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new BoolObject(value1.Value() == ((DateTimeObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(BoolObject))
                return new BoolObject(value1.Value() == ((BoolObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(StringObject))
                return new BoolObject(value1.Value() == ((StringObject)value2).AsInt().Value());
            return new BoolObject(false);
        }

        public static BoolObject operator <=(IntObject value1, CsvObject value2)
        {
            return !(value1 > value2);
        }

        public static BoolObject operator >=(IntObject value1, CsvObject value2)
        {
            return !(value1 < value2);
        }

        public static BoolObject operator !=(IntObject value1, CsvObject value2)
        {
            return !(value1 == value2);
        }

        public static CsvObject operator |(IntObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                throw new QueryTextDriverException("В оператор \"|\" не передана ссылка на объект");
            if (value2.GetType() == typeof(IntObject))
                return new IntObject(value1.Value() | ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new IntObject(value1.Value() | ((DoubleObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new IntObject(value1.Value() | ((DateTimeObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(BoolObject))
                return new IntObject(value1.Value() | ((BoolObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(StringObject))
                return new IntObject(value1.Value() | ((StringObject)value2).AsInt().Value());
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static CsvObject operator &(IntObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                throw new QueryTextDriverException("В оператор \"&\" не передана ссылка на объект");
            if (value2.GetType() == typeof(IntObject))
                return new IntObject(value1.Value() & ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new IntObject(value1.Value() & ((DoubleObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new IntObject(value1.Value() & ((DateTimeObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(BoolObject))
                return new IntObject(value1.Value() & ((BoolObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(StringObject))
                return new IntObject(value1.Value() & ((StringObject)value2).AsInt().Value());
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static CsvObject operator ^(IntObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                throw new QueryTextDriverException("В оператор \"^\" не передана ссылка на объект");
            if (value2.GetType() == typeof(IntObject))
                return new IntObject(value1.Value() ^ ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new IntObject(value1.Value() ^ ((DoubleObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new IntObject(value1.Value() ^ ((DateTimeObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(BoolObject))
                return new IntObject(value1.Value() ^ ((BoolObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(StringObject))
                return new IntObject(value1.Value() ^ ((StringObject)value2).AsInt().Value());
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static CsvObject Add(IntObject value1, CsvObject value2)
        {
            return value1 + value2;
        }

        public static CsvObject Subtract(IntObject value1, CsvObject value2)
        {
            return value1 - value2;
        }

        public static CsvObject Divide(IntObject value1, CsvObject value2)
        {
            return value1 / value2;
        }

        public static CsvObject Mod(IntObject value1, CsvObject value2)
        {
            return value1 % value2;
        }

        public static CsvObject Multiply(IntObject value1, CsvObject value2)
        {
            return value1 * value2;
        }

        public static int Compare(IntObject value1, CsvObject value2)
        {
            return value1 > value2 ? 1 : value1 < value2 ? -1 : 0;
        }

        public static CsvObject BitwiseAnd(IntObject value1, CsvObject value2)
        {
            return value1 & value2;
        }

        public static CsvObject BitwiseOr(IntObject value1, CsvObject value2)
        {
            return value1 | value2;
        }

        public static CsvObject Xor(IntObject value1, CsvObject value2)
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

        public new long Value()
        {
            return (long)value;
        }
    }
}
