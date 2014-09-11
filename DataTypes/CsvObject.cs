using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;
using QueryTextDriverExceptionNS;

namespace DataTypes
{
    public abstract class CsvObject
    {
        protected object value { get; set; }

        protected CsvObject()
        {
        }

        public static CsvObject Create(object value)
        {
            long result;
            double result2;
            DateTime result3;
            bool result4;
            if (long.TryParse(value.ToString(), out result))
                return new IntObject(System.Convert.ChangeType(value, typeof(long)));
            else
                if (double.TryParse(value.ToString(), out result2))
                    return new DoubleObject(System.Convert.ChangeType(value, typeof(double)));
                else
                    if (DateTime.TryParse(value.ToString(), out result3))
                        return new DateTimeObject(System.Convert.ChangeType(value, typeof(DateTime)));
                    else
                        if (bool.TryParse(value.ToString(), out result4))
                            return new BoolObject(System.Convert.ChangeType(value, typeof(bool)));
                        else
                            return new StringObject(System.Convert.ChangeType(value, typeof(string)));
        }

        public Type GetValueType()
        {
            return value.GetType();
        }

        public CsvObject Convert(Type type)
        {
            if (type == typeof(long))
                return new IntObject(this.value);
            else
                if (type == typeof(double))
                    return new DoubleObject(this.value);
                else
                    if (type == typeof(DateTime))
                        return new DateTimeObject(this.value);
                    else
                        if (type == typeof(bool))
                            return new BoolObject(this.value);
                        else
                            if (type == typeof(string))
                                return new StringObject(this.value);
            QueryTextDriverException exception = new QueryTextDriverException("Тип данных {0} не известен");
            exception.Data.Add("{0}", type.ToString());
            throw exception;
        }

        public abstract IntObject AsInt();
        public abstract BoolObject AsBool();
        public abstract StringObject AsString();
        public abstract DateTimeObject AsDateTime();
        public abstract DoubleObject AsDouble();
        public virtual RangeObject AsRange()
        {
            return new RangeObject(this, this);
        }
        public virtual CsvCollection AsArray()
        {
            CsvCollection array = new CsvCollection();
            array.Add(this);
            return array;
        }

        public static CsvObject operator +(CsvObject value1, CsvObject value2)
        {
            if (value1.GetType() == typeof(IntObject))
                return (((IntObject)value1) + value2);
            if (value1.GetType() == typeof(DoubleObject))
                return (((DoubleObject)value1) + value2);
            if (value1.GetType() == typeof(DateTimeObject))
                return (((DateTimeObject)value1) + value2);
            if (value1.GetType() == typeof(BoolObject))
                return (((BoolObject)value1) + value2);
            if (value1.GetType() == typeof(StringObject))
                return (((StringObject)value1) + value2);
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value1.GetType().ToString());
            throw exception;
        }

        public static CsvObject operator -(CsvObject value1, CsvObject value2)
        {
            if (value1.GetType() == typeof(IntObject))
                return (((IntObject)value1) - value2);
            if (value1.GetType() == typeof(DoubleObject))
                return (((DoubleObject)value1) - value2);
            if (value1.GetType() == typeof(DateTimeObject))
                return (((DateTimeObject)value1) - value2);
            if (value1.GetType() == typeof(BoolObject))
                return (((BoolObject)value1) - value2);
            if (value1.GetType() == typeof(StringObject))
                return (((StringObject)value1) - value2);
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value1.GetType().ToString());
            throw exception;
        }

        public static CsvObject operator /(CsvObject value1, CsvObject value2)
        {
            if (value1.GetType() == typeof(IntObject))
                return (((IntObject)value1) / value2);
            if (value1.GetType() == typeof(DoubleObject))
                return (((DoubleObject)value1) / value2);
            if (value1.GetType() == typeof(DateTimeObject))
                return (((DateTimeObject)value1) / value2);
            if (value1.GetType() == typeof(BoolObject))
                return (((BoolObject)value1) / value2);
            if (value1.GetType() == typeof(StringObject))
                return (((StringObject)value1) / value2);
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value1.GetType().ToString());
            throw exception;
        }

        public static CsvObject operator *(CsvObject value1, CsvObject value2)
        {
            if (value1.GetType() == typeof(IntObject))
                return (((IntObject)value1) * value2);
            if (value1.GetType() == typeof(DoubleObject))
                return (((DoubleObject)value1) * value2);
            if (value1.GetType() == typeof(DateTimeObject))
                return (((DateTimeObject)value1) * value2);
            if (value1.GetType() == typeof(BoolObject))
                return (((BoolObject)value1) * value2);
            if (value1.GetType() == typeof(StringObject))
                return (((StringObject)value1) * value2);
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value1.GetType().ToString());
            throw exception;
        }

        public static CsvObject operator %(CsvObject value1, CsvObject value2)
        {
            if (value1.GetType() == typeof(IntObject))
                return (((IntObject)value1) % value2);
            if (value1.GetType() == typeof(DoubleObject))
                return (((DoubleObject)value1) % value2);
            if (value1.GetType() == typeof(DateTimeObject))
                return (((DateTimeObject)value1) % value2);
            if (value1.GetType() == typeof(BoolObject))
                return (((BoolObject)value1) % value2);
            if (value1.GetType() == typeof(StringObject))
                return (((StringObject)value1) % value2);
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value1.GetType().ToString());
            throw exception;
        }

        public static BoolObject operator >(CsvObject value1, CsvObject value2)
        {
            if (value1.GetType() == typeof(IntObject))
                return (((IntObject)value1) > value2);
            if (value1.GetType() == typeof(DoubleObject))
                return (((DoubleObject)value1) > value2);
            if (value1.GetType() == typeof(DateTimeObject))
                return (((DateTimeObject)value1) > value2);
            if (value1.GetType() == typeof(BoolObject))
                return (((BoolObject)value1) > value2);
            if (value1.GetType() == typeof(StringObject))
                return (((StringObject)value1) > value2);
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value1.GetType().ToString());
            throw exception;
        }

        public static BoolObject operator <(CsvObject value1, CsvObject value2)
        {
            if (value1.GetType() == typeof(IntObject))
                return (((IntObject)value1) < value2);
            if (value1.GetType() == typeof(DoubleObject))
                return (((DoubleObject)value1) < value2);
            if (value1.GetType() == typeof(DateTimeObject))
                return (((DateTimeObject)value1) < value2);
            if (value1.GetType() == typeof(BoolObject))
                return (((BoolObject)value1) < value2);
            if (value1.GetType() == typeof(StringObject))
                return (((StringObject)value1) < value2);
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value1.GetType().ToString());
            throw exception;
        }

        public static BoolObject operator <=(CsvObject value1, CsvObject value2)
        {
            return !(value1 > value2);
        }

        public static BoolObject operator >=(CsvObject value1, CsvObject value2)
        {
            return !(value1 < value2);
        }

        public static BoolObject operator ==(CsvObject value1, CsvObject value2)
        {
            if (value1.GetType() == typeof(IntObject))
                return (((IntObject)value1) == value2);
            if (value1.GetType() == typeof(DoubleObject))
                return (((DoubleObject)value1) == value2);
            if (value1.GetType() == typeof(DateTimeObject))
                return (((DateTimeObject)value1) == value2);
            if (value1.GetType() == typeof(BoolObject))
                return (((BoolObject)value1) == value2);
            if (value1.GetType() == typeof(StringObject))
                return (((StringObject)value1) == value2);
            return new BoolObject(false);
        }

        public static BoolObject operator !=(CsvObject value1, CsvObject value2)
        {
            return !(value1 == value2);
        }

        public static CsvObject operator |(CsvObject value1, CsvObject value2)
        {
            if (value1.GetType() == typeof(IntObject))
                return (((IntObject)value1) | value2);
            if (value1.GetType() == typeof(DoubleObject))
                return (((DoubleObject)value1) | value2);
            if (value1.GetType() == typeof(DateTimeObject))
                return (((DateTimeObject)value1) | value2);
            if (value1.GetType() == typeof(BoolObject))
                return (((BoolObject)value1) | value2);
            if (value1.GetType() == typeof(StringObject))
                return (((StringObject)value1) | value2);
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value1.GetType().ToString());
            throw exception;
        }

        public static CsvObject operator &(CsvObject value1, CsvObject value2)
        {
            if (value1.GetType() == typeof(IntObject))
                return (((IntObject)value1) & value2);
            if (value1.GetType() == typeof(DoubleObject))
                return (((DoubleObject)value1) & value2);
            if (value1.GetType() == typeof(DateTimeObject))
                return (((DateTimeObject)value1) & value2);
            if (value1.GetType() == typeof(BoolObject))
                return (((BoolObject)value1) & value2);
            if (value1.GetType() == typeof(StringObject))
                return (((StringObject)value1) & value2);
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value1.GetType().ToString());
            throw exception;
        }

        public static CsvObject operator ^(CsvObject value1, CsvObject value2)
        {
            if (value1.GetType() == typeof(IntObject))
                return (((IntObject)value1) ^ value2);
            if (value1.GetType() == typeof(DoubleObject))
                return (((DoubleObject)value1) ^ value2);
            if (value1.GetType() == typeof(DateTimeObject))
                return (((DateTimeObject)value1) ^ value2);
            if (value1.GetType() == typeof(BoolObject))
                return (((BoolObject)value1) ^ value2);
            if (value1.GetType() == typeof(StringObject))
                return (((StringObject)value1) ^ value2);
            QueryTextDriverException exception = new QueryTextDriverException("Неизвестный тип данных {0}");
            exception.Data.Add("{0}", value1.GetType().ToString());
            throw exception;
        }

        public static CsvObject Add(CsvObject value1, CsvObject value2)
        {
            return value1 + value2;
        }

        public static CsvObject Subtract(CsvObject value1, CsvObject value2)
        {
            return value1 - value2;
        }

        public static CsvObject Divide(CsvObject value1, CsvObject value2)
        {
            return value1 / value2;
        }

        public static CsvObject Mod(CsvObject value1, CsvObject value2)
        {
            return value1 % value2;
        }

        public static CsvObject Multiply(CsvObject value1, CsvObject value2)
        {
            return value1 * value2;
        }

        public static int Compare(CsvObject value1, CsvObject value2)
        {
            return value1 > value2 ? 1 : value1 < value2 ? -1 : 0;
        }

        public static CsvObject BitwiseAnd(CsvObject value1, CsvObject value2)
        {
            return value1 & value2;
        }

        public static CsvObject BitwiseOr(CsvObject value1, CsvObject value2)
        {
            return value1 | value2;
        }

        public static CsvObject Xor(CsvObject value1, CsvObject value2)
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

        public virtual BoolObject InRange(RangeObject range)
        {
            if ((this > range.StartObject).Value() && (this < range.EndObject).Value())
                return new BoolObject(true);
            else
                return new BoolObject(false);
        }

        public object Value()
        {
            return value;
        }
    
    }
}
