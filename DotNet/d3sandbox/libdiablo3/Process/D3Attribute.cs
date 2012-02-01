using System;
using System.Runtime.InteropServices;

namespace libdiablo3.Process
{
    public enum D3AttributeEncoding
    {
        Int,
        IntMinMax,
        Float16,
        Float16Or32,
        Float32,
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct D3AttributeValue
    {
        [FieldOffset(0)]
        public int Value;
        [FieldOffset(0)]
        public float ValueF;

        public D3AttributeValue(int value) { ValueF = 0f; Value = value; }
        public D3AttributeValue(float value) { Value = 0; ValueF = value; }

        public override string ToString()
        {
            return String.Format("Int: {0}, Float: {1}", Value, ValueF);
        }
    }

    public partial class D3Attribute
    {
        public const float Float16Min = -65536.0f;
        public const float Float16Max = 65536.0f;

        public int ID;
        public int U3;
        public int U4;
        public int U5;

        public string ScriptA;
        public string ScriptB;
        public string Name;

        public D3AttributeEncoding EncodingType;

        public byte U10;

        public D3AttributeValue Min;
        public D3AttributeValue Max;
        public int BitCount;

        protected D3AttributeValue defaultValue;

        public bool IsInteger { get { return EncodingType == D3AttributeEncoding.Int || EncodingType == D3AttributeEncoding.IntMinMax; } }

        public D3Attribute() { }

        public D3Attribute(int id, int defaultValue, int u3, int u4, int u5, string scriptA, string scriptB, string name, D3AttributeEncoding encodingType, byte u10, int min, int max, int bitCount)
        {
            ID = id;
            this.defaultValue.Value = defaultValue;
            U3 = u3;
            U4 = u4;
            U5 = u5;
            ScriptA = scriptA;
            ScriptB = scriptB;
            Name = name;
            EncodingType = encodingType;
            U10 = u10;

            Min = new D3AttributeValue(min);
            Max = new D3AttributeValue(max);
            BitCount = bitCount;
        }

        public D3Attribute(int id, float defaultValue, int u3, int u4, int u5, string scriptA, string scriptB, string name, D3AttributeEncoding encodingType, byte u10, float min, float max, int bitCount)
        {
            ID = id;
            this.defaultValue.ValueF = defaultValue;
            U3 = u3;
            U4 = u4;
            U5 = u5;
            ScriptA = scriptA;
            ScriptB = scriptB;
            Name = name;
            EncodingType = encodingType;
            U10 = u10;

            Min = new D3AttributeValue(min);
            Max = new D3AttributeValue(max);
            BitCount = bitCount;
        }

        public virtual string ValueToString(D3AttributeValue value)
        {
            return "[Invalid]";
        }

        public override int GetHashCode()
        {
            return ID;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class D3AttributeI : D3Attribute
    {
        public int DefaultValue { get { return defaultValue.Value; } }

        public D3AttributeI() { }

        public D3AttributeI(int id, int defaultValue, int u3, int u4, int u5, string scriptA, string scriptB, string name, D3AttributeEncoding encodingType, byte u10, int min, int max, int bitCount)
            : base(id, defaultValue, u3, u4, u5, scriptA, scriptB, name, encodingType, u10, min, max, bitCount)
        {

        }

        public override string ValueToString(D3AttributeValue value)
        {
            return value.Value.ToString();
        }
    }

    public class D3AttributeF : D3Attribute
    {
        public float DefaultValue { get { return defaultValue.ValueF; } }

        public D3AttributeF() { }
        public D3AttributeF(int id, float defaultValue, int u3, int u4, int u5, string scriptA, string scriptB, string name, D3AttributeEncoding encodingType, byte u10, float min, float max, int bitCount)
            : base(id, defaultValue, u3, u4, u5, scriptA, scriptB, name, encodingType, u10, min, max, bitCount)
        {

        }

        public override string ValueToString(D3AttributeValue value)
        {
            return value.ValueF.ToString();
        }
    }

    public class D3AttributeB : D3Attribute
    {
        public bool DefaultValue { get { return defaultValue.Value != 0; } }

        public D3AttributeB() { }
        public D3AttributeB(int id, int defaultValue, int u3, int u4, int u5, string scriptA, string scriptB, string name, D3AttributeEncoding encodingType, byte u10, int min, int max, int bitCount)
            : base(id, defaultValue, u3, u4, u5, scriptA, scriptB, name, encodingType, u10, min, max, bitCount)
        {

        }

        public override string ValueToString(D3AttributeValue value)
        {
            return (value.Value != 0).ToString();
        }
    }
}
