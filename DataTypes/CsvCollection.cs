using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections;

namespace DataTypes
{
    public class CsvCollection : CsvObject, IEnumerable<CsvObject>, IEnumerator<CsvObject>, IEnumerable, IEnumerator, IDisposable
    {
        private Collection<CsvObject> array;
        private int currentIndex = -1;

        public CsvCollection()
        {
            array = new Collection<CsvObject>();
        }

        public void Add(CsvObject value)
        {
            array.Add(value);
        }

        public CsvObject this[int index] { get { return array[index]; } set { array[index] = value; } }

        IEnumerator<CsvObject> IEnumerable<CsvObject>.GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        public bool MoveNext()
        {
            currentIndex++;
            if (currentIndex < array.Count)
                return true;
            else
                return false;
        }

        public void Reset()
        {
            currentIndex = 0;
        }

        CsvObject IEnumerator<CsvObject>.Current
        {
            get { return array[currentIndex]; }
        }

        object IEnumerator.Current
        {
            get { return array[currentIndex]; }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(array);
        }

        public new String Value()
        {
            return (String)value;
        }

        public void AddRange(CsvCollection range)
        {
            foreach (CsvObject value in range)
                this.Add(value);
        }

        public override IntObject AsInt()
        {
            if (array.Count == 1)
                return new IntObject(array[0].Value());
            else
                return null;
        }

        public override BoolObject AsBool()
        {
            if (array.Count == 1)
                return new BoolObject(array[0].Value());
            else
                return null;
        }

        public override StringObject AsString()
        {
            string result = "[{0}]";
            foreach (CsvObject obj in array)
                result = String.Format(result, obj.Value().ToString() + ",{0}");
            return new StringObject(result.Substring(0, result.Length - 5) + "]");
        }

        public override DateTimeObject AsDateTime()
        {
            if (array.Count == 1)
                return new DateTimeObject(array[0].Value());
            else
                return null;
        }

        public override DoubleObject AsDouble()
        {
            if (array.Count == 1)
                return new DoubleObject(array[0].Value());
            else
                return null;
        }

        public override RangeObject AsRange()
        {
            return null;
        }

        public override CsvCollection AsArray()
        {
            return this;
        }

        public override BoolObject InRange(RangeObject range)
        {
            bool included = true;
            foreach (CsvObject obj in array)
                if ((obj < range.StartObject).Value() || (obj > range.EndObject).Value())
                    included = false;
            return new BoolObject(included);
        }
    }
}
