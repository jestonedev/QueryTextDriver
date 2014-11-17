using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using QueryTextDriverExceptionNS;

namespace DataTypes
{
    public class DateTimeObject : CsvObject
    {
        public DateTimeObject(object value)
        {
            this.value = System.Convert.ToDateTime(value, CultureInfo.CurrentCulture);
        }

        public override BoolObject AsBool()
        {
            return new BoolObject(System.Convert.ToBoolean((DateTime)value));
        }

        public override IntObject AsInt()
        {
            return new IntObject(System.Convert.ToInt64((DateTime)value));
        }

        public override DoubleObject AsDouble()
        {
            return new DoubleObject(System.Convert.ToDouble((DateTime)value));
        }

        public override DateTimeObject AsDateTime()
        {
            return this;
        }

        public override StringObject AsString()
        {
            return new StringObject(value.ToString());
        }

        public static CsvObject operator +(DateTimeObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                throw new QueryTextDriverException("В оператор \"+\" не передана ссылка на объект");
            if (value2.GetType() == typeof(IntObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.AsInt().Value() + ((IntObject)value2).Value()));
            if (value2.GetType() == typeof(DoubleObject))
                return new DateTimeObject(System.Convert.ToDateTime((value1.AsDouble().Value() + ((DoubleObject)value2).Value())));
            if (value2.GetType() == typeof(DateTimeObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.AsInt().Value() + ((DateTimeObject)value2).AsInt().Value()));
            if (value2.GetType() == typeof(BoolObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.AsInt().Value() + ((BoolObject)value2).AsInt().Value()));
            if (value2.GetType() == typeof(StringObject))
                return new StringObject(value1.AsString().Value() + ((StringObject)value2).Value());
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static CsvObject operator -(DateTimeObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                throw new QueryTextDriverException("В оператор \"-\" не передана ссылка на объект");
            if (value2.GetType() == typeof(IntObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.AsInt().Value() - ((IntObject)value2).Value()));
            if (value2.GetType() == typeof(DoubleObject))
                return new DateTimeObject(System.Convert.ToDateTime((value1.AsDouble().Value() - ((DoubleObject)value2).Value())));
            if (value2.GetType() == typeof(DateTimeObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.AsInt().Value() - ((DateTimeObject)value2).AsInt().Value()));
            if (value2.GetType() == typeof(BoolObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.AsInt().Value() - ((BoolObject)value2).AsInt().Value()));
            if (value2.GetType() == typeof(StringObject))
                return new IntObject(0);
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static CsvObject operator /(DateTimeObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                throw new QueryTextDriverException("В оператор \"/\" не передана ссылка на объект");
            if (value2.GetType() == typeof(IntObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.AsInt().Value() / ((IntObject)value2).Value()));
            if (value2.GetType() == typeof(DoubleObject))
                return new DateTimeObject(System.Convert.ToDateTime((value1.AsDouble().Value() / ((DoubleObject)value2).Value())));
            if (value2.GetType() == typeof(DateTimeObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.AsInt().Value() / ((DateTimeObject)value2).AsInt().Value()));
            if (value2.GetType() == typeof(BoolObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.AsInt().Value() / ((BoolObject)value2).AsInt().Value()));
            if (value2.GetType() == typeof(StringObject))
                return new IntObject(0);
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static CsvObject operator *(DateTimeObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                throw new QueryTextDriverException("В оператор \"*\" не передана ссылка на объект");
            if (value2.GetType() == typeof(IntObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.AsInt().Value() * ((IntObject)value2).Value()));
            if (value2.GetType() == typeof(DoubleObject))
                return new DateTimeObject(System.Convert.ToDateTime((value1.AsDouble().Value() * ((DoubleObject)value2).Value())));
            if (value2.GetType() == typeof(DateTimeObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.AsInt().Value() * ((DateTimeObject)value2).AsInt().Value()));
            if (value2.GetType() == typeof(BoolObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.AsInt().Value() * ((BoolObject)value2).AsInt().Value()));
            if (value2.GetType() == typeof(StringObject))
                return new IntObject(0);
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static CsvObject operator %(DateTimeObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                throw new QueryTextDriverException("В оператор \"%\" не передана ссылка на объект");
            if (value2.GetType() == typeof(IntObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.AsInt().Value() % ((IntObject)value2).Value()));
            if (value2.GetType() == typeof(DoubleObject))
                return new DateTimeObject(System.Convert.ToDateTime((value1.AsDouble().Value() % ((DoubleObject)value2).Value())));
            if (value2.GetType() == typeof(DateTimeObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.AsInt().Value() % ((DateTimeObject)value2).AsInt().Value()));
            if (value2.GetType() == typeof(BoolObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.AsInt().Value() % ((BoolObject)value2).AsInt().Value()));
            if (value2.GetType() == typeof(StringObject))
                return new IntObject(0);
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static BoolObject operator >(DateTimeObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                return new BoolObject(false);
            if (value2.GetType() == typeof(IntObject))
                return new BoolObject(value1.AsInt().Value() > ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new BoolObject(value1.AsDouble().Value() > ((DoubleObject)value2).Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new BoolObject(value1.Value() > ((DateTimeObject)value2).Value());
            if (value2.GetType() == typeof(BoolObject))
                return new BoolObject(value1.AsInt().Value() > ((BoolObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(StringObject))
                return new BoolObject(value1.Value() > ((StringObject)value2).AsDateTime().Value());
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static BoolObject operator <(DateTimeObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                return new BoolObject(false);
            if (value2.GetType() == typeof(IntObject))
                return new BoolObject(value1.AsInt().Value() < ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new BoolObject(value1.AsDouble().Value() < ((DoubleObject)value2).Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new BoolObject(value1.Value() < ((DateTimeObject)value2).Value());
            if (value2.GetType() == typeof(BoolObject))
                return new BoolObject(value1.AsInt().Value() < ((BoolObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(StringObject))
                return new BoolObject(value1.Value() < ((StringObject)value2).AsDateTime().Value());
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static BoolObject operator ==(DateTimeObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                return new BoolObject(false);
            if (value2.GetType() == typeof(IntObject))
                return new BoolObject(value1.AsInt().Value() == ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new BoolObject(value1.AsDouble().Value() == ((DoubleObject)value2).Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new BoolObject(value1.Value() == ((DateTimeObject)value2).Value());
            if (value2.GetType() == typeof(BoolObject))
                return new BoolObject(value1.AsInt().Value() == ((BoolObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(StringObject))
                return new BoolObject(value1.Value() == ((StringObject)value2).AsDateTime().Value());
            return new BoolObject(false);
        }

        public static BoolObject operator <=(DateTimeObject value1, CsvObject value2)
        {
            return !(value1 > value2);
        }

        public static BoolObject operator >=(DateTimeObject value1, CsvObject value2)
        {
            return !(value1 < value2);
        }

        public static BoolObject operator !=(DateTimeObject value1, CsvObject value2)
        {
            return !(value1 == value2);
        }

        public static CsvObject operator |(DateTimeObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                throw new QueryTextDriverException("В оператор \"|\" не передана ссылка на объект");
            if (value2.GetType() == typeof(IntObject))
                return new BoolObject(value1.AsInt().Value() | ((IntObject)value2).AsInt().Value());
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

        public static CsvObject operator &(DateTimeObject value1, CsvObject value2)
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

        public static CsvObject operator ^(DateTimeObject value1, CsvObject value2)
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

        public static CsvObject Add(DateTimeObject value1, CsvObject value2)
        {
            return value1 + value2;
        }

        public static CsvObject Subtract(DateTimeObject value1, CsvObject value2)
        {
            return value1 - value2;
        }

        public static CsvObject Divide(DateTimeObject value1, CsvObject value2)
        {
            return value1 / value2;
        }

        public static CsvObject Mod(DateTimeObject value1, CsvObject value2)
        {
            return value1 % value2;
        }

        public static CsvObject Multiply(DateTimeObject value1, CsvObject value2)
        {
            return value1 * value2;
        }

        public static int Compare(DateTimeObject value1, CsvObject value2)
        {
            return value1 > value2 ? 1 : value1 < value2 ? -1 : 0;
        }

        public static CsvObject BitwiseAnd(DateTimeObject value1, CsvObject value2)
        {
            return value1 & value2;
        }

        public static CsvObject BitwiseOr(DateTimeObject value1, CsvObject value2)
        {
            return value1 | value2;
        }

        public static CsvObject Xor(DateTimeObject value1, CsvObject value2)
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

        public new DateTime Value()
        {
            return (DateTime)value;
        }
    }
}
