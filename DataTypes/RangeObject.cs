using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTypes
{
    public class RangeObject : CsvObject
    {
        public CsvObject StartObject { get; set; }
        public CsvObject EndObject { get; set; }

        public RangeObject(CsvObject startObject, CsvObject endObject)
        {
            if (startObject > endObject)
            {
                EndObject = startObject;
                StartObject = endObject;
            }
            else
            {
                StartObject = startObject;
                EndObject = endObject;
            }
        }

        public override IntObject AsInt()
        {
            if (StartObject == EndObject)
                return new IntObject(StartObject.Value());
            else
                return null;
        }

        public override BoolObject AsBool()
        {
            if (StartObject == EndObject)
                return new BoolObject(StartObject.Value());
            else
                return null;
        }

        public override StringObject AsString()
        {
            return new StringObject(String.Format("[{0},{1}]", StartObject.Value().ToString(), EndObject.Value().ToString()));
        }

        public override DateTimeObject AsDateTime()
        {
            if (StartObject == EndObject)
                return new DateTimeObject(StartObject.Value());
            else
                return null;
        }

        public override DoubleObject AsDouble()
        {
            if (StartObject == EndObject)
                return new DoubleObject(StartObject.Value());
            else
                return null;
        }

        public override RangeObject AsRange()
        {
            return this;
        }

        public override CsvCollection AsArray()
        {
            return null;
        }

        public override BoolObject InRange(RangeObject range)
        {
            if ((this.StartObject > range.StartObject).Value() && (this.EndObject < range.EndObject).Value())
                return new BoolObject(true);
            else
                return new BoolObject(false);
        }
    }

}
