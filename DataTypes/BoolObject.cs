﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using QueryTextDriverExceptionNS;

namespace DataTypes
{
    public class BoolObject : CsvObject
    {
        public BoolObject(object value)
        {
            this.value = System.Convert.ToBoolean(value, CultureInfo.CurrentCulture);
        }

        public override BoolObject AsBool()
        {
            return this;
        }

        public override IntObject AsInt()
        {
            if ((bool)value)
                return new IntObject(1);
            else
                return new IntObject(0);
        }

        public override DoubleObject AsDouble()
        {
            if ((bool)value)
                return new DoubleObject(1);
            else
                return new DoubleObject(0);
        }

        public override StringObject AsString()
        {
            return new StringObject(value.ToString());
        }

        public override DateTimeObject AsDateTime()
        {
            return new DateTimeObject(System.Convert.ToDateTime((bool)value));
        }

        public static CsvObject operator +(BoolObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                throw new QueryTextDriverException("В оператор \"+\" не передана ссылка на объект");
            if (value2.GetType() == typeof(IntObject))
                return new IntObject(value1.AsInt().Value() + ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new DoubleObject(value1.AsDouble().Value() + ((DoubleObject)value2).Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.AsInt().Value() + ((DateTimeObject)value2).AsInt().Value()));
            if (value2.GetType() == typeof(BoolObject))
                return new DoubleObject(value1.AsInt().Value() + ((BoolObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(StringObject))
                return new StringObject(value1.AsString().Value() + ((StringObject)value2).Value());
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static CsvObject operator -(BoolObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                throw new QueryTextDriverException("В оператор \"-\" не передана ссылка на объект");
            if (value2.GetType() == typeof(IntObject))
                return new IntObject(value1.AsInt().Value() - ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new DoubleObject(value1.AsDouble().Value() - ((DoubleObject)value2).Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.AsInt().Value() - ((DateTimeObject)value2).AsInt().Value()));
            if (value2.GetType() == typeof(BoolObject))
                return new DoubleObject(value1.AsInt().Value() - ((BoolObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(StringObject))
                return new BoolObject(false);
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static CsvObject operator /(BoolObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                throw new QueryTextDriverException("В оператор \"/\" не передана ссылка на объект");
            if (value2.GetType() == typeof(IntObject))
                return new IntObject(value1.AsInt().Value() / ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new DoubleObject(value1.AsDouble().Value() / ((DoubleObject)value2).Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.AsInt().Value() / ((DateTimeObject)value2).AsInt().Value()));
            if (value2.GetType() == typeof(BoolObject))
                return new DoubleObject(value1.AsInt().Value() / ((BoolObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(StringObject))
                return new BoolObject(false);
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static CsvObject operator *(BoolObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                throw new QueryTextDriverException("В оператор \"*\" не передана ссылка на объект");
            if (value2.GetType() == typeof(IntObject))
                return new IntObject(value1.AsInt().Value() * ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new DoubleObject(value1.AsDouble().Value() * ((DoubleObject)value2).Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.AsInt().Value() * ((DateTimeObject)value2).AsInt().Value()));
            if (value2.GetType() == typeof(BoolObject))
                return new DoubleObject(value1.AsInt().Value() * ((BoolObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(StringObject))
                return new BoolObject(false);
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static CsvObject operator %(BoolObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                throw new QueryTextDriverException("В оператор \"%\" не передана ссылка на объект");
            if (value2.GetType() == typeof(IntObject))
                return new IntObject(value1.AsInt().Value() % ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new DoubleObject(value1.AsDouble().Value() % ((DoubleObject)value2).Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new DateTimeObject(System.Convert.ToDateTime(value1.AsInt().Value() % ((DateTimeObject)value2).AsInt().Value()));
            if (value2.GetType() == typeof(BoolObject))
                return new DoubleObject(value1.AsInt().Value() % ((BoolObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(StringObject))
                return new BoolObject(false);
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static BoolObject operator >(BoolObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                return new BoolObject(false);
            if (value2.GetType() == typeof(IntObject))
                return new BoolObject(value1.AsInt().Value() > ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new BoolObject(value1.AsDouble().Value() > ((DoubleObject)value2).Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new BoolObject(value1.AsInt().Value() > ((DateTimeObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(BoolObject))
                return new BoolObject(value1.AsInt().Value() > ((BoolObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(StringObject))
                return new BoolObject(value1.AsInt().Value() > ((StringObject)value2).AsBool().AsInt().Value());
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static BoolObject operator <(BoolObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                return new BoolObject(false);
            if (value2.GetType() == typeof(IntObject))
                return new BoolObject(value1.AsInt().Value() < ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new BoolObject(value1.AsDouble().Value() < ((DoubleObject)value2).Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new BoolObject(value1.AsInt().Value() < ((DateTimeObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(BoolObject))
                return new BoolObject(value1.AsInt().Value() < ((BoolObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(StringObject))
                return new BoolObject(value1.AsInt().Value() < ((StringObject)value2).AsBool().AsInt().Value());
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value2.GetType().ToString());
            throw exception;
        }

        public static BoolObject operator ==(BoolObject value1, CsvObject value2)
        {
            if (((object)value1 == null) || ((object)value2 == null))
                return new BoolObject(false);
            if (value2.GetType() == typeof(IntObject))
                return new BoolObject(value1.AsInt().Value() == ((IntObject)value2).Value());
            if (value2.GetType() == typeof(DoubleObject))
                return new BoolObject(value1.AsDouble().Value() == ((DoubleObject)value2).Value());
            if (value2.GetType() == typeof(DateTimeObject))
                return new BoolObject(value1.AsInt().Value() == ((DateTimeObject)value2).AsInt().Value());
            if (value2.GetType() == typeof(BoolObject))
                return new BoolObject(value1.Value() == ((BoolObject)value2).Value());
            if (value2.GetType() == typeof(StringObject))
                return new BoolObject(value1.AsInt().Value() == ((StringObject)value2).AsBool().AsInt().Value());
            return new BoolObject(false);
        }

        public static BoolObject operator <=(BoolObject value1, CsvObject value2)
        {
            return !(value1 > value2);
        }

        public static BoolObject operator >=(BoolObject value1, CsvObject value2)
        {
            return !(value1 < value2);
        }

        public static BoolObject operator !=(BoolObject value1, CsvObject value2)
        {
            return !(value1 == value2);
        }

        public static BoolObject operator !(BoolObject value)
        {
            if ((object)value == null)
                throw new QueryTextDriverException("В оператор \"!\" не передана ссылка на объект");
            return new BoolObject(!(bool)value.value);
        }

        public static CsvObject operator |(BoolObject value1, CsvObject value2)
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

        public static CsvObject operator &(BoolObject value1, CsvObject value2)
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

        public static CsvObject operator ^(BoolObject value1, CsvObject value2)
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

        public static CsvObject Add(BoolObject value1, CsvObject value2)
        {
            return value1 + value2;
        }

        public static CsvObject Subtract(BoolObject value1, CsvObject value2)
        {
            return value1 - value2;
        }

        public static CsvObject Divide(BoolObject value1, CsvObject value2)
        {
            return value1 / value2;
        }

        public static CsvObject Mod(BoolObject value1, CsvObject value2)
        {
            return value1 % value2;
        }

        public static CsvObject Multiply(BoolObject value1, CsvObject value2)
        {
            return value1 * value2;
        }

        public static int Compare(BoolObject value1, CsvObject value2)
        {
            return value1 > value2 ? 1 : value1 < value2 ? -1 : 0;
        }

        public static CsvObject BitwiseAnd(BoolObject value1, CsvObject value2)
        {
            return value1 & value2;
        }

        public static CsvObject BitwiseOr(BoolObject value1, CsvObject value2)
        {
            return value1 | value2;
        }

        public static CsvObject Xor(BoolObject value1, CsvObject value2)
        {
            return value1 ^ value2;
        }

        public static CsvObject LogicalNot(BoolObject value)
        {
            return !value;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public new bool Value()
        {
            return (bool)value;
        }

        public static implicit operator bool(BoolObject value)
        {
            if ((object)value == null)
                return false;
            return value.Value();
        }

        public static bool ToBoolean(BoolObject value)
        {
            if ((object)value == null)
                return false;
            return value.Value();
        }
    }

}
