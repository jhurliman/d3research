using System;
using System.Collections;
using System.Collections.Generic;

namespace libdiablo3.Process
{
    public class D3AttributeMap : IEnumerable<KeyValuePair<uint, D3AttributeValue>>
    {
        private Dictionary<uint, D3AttributeValue> attributes;

        public D3AttributeMap()
        {
            attributes = new Dictionary<uint, D3AttributeValue>();
        }

        public D3AttributeMap(int capacity)
        {
            attributes = new Dictionary<uint, D3AttributeValue>(capacity);
        }

        public bool TryGetValue(D3Attribute attribute, out D3AttributeValue value)
        {
            return attributes.TryGetValue(GetID(attribute), out value);
        }

        public bool TryGetValue(D3Attribute attribute, int key, out D3AttributeValue value)
        {
            return attributes.TryGetValue(GetID(attribute, key), out value);
        }

        public void SetValue(D3Attribute attribute, D3AttributeValue value)
        {
            attributes[GetID(attribute)] = value;
        }

        public void SetValue(D3Attribute attribute, int key, D3AttributeValue value)
        {
            attributes[GetID(attribute, key)] = value;
        }

        public void SetValue(uint id, D3AttributeValue value)
        {
            attributes[id] = value;
        }

        #region IEnumerable

        public IEnumerator<KeyValuePair<uint, D3AttributeValue>> GetEnumerator()
        {
            return this.attributes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerable

        #region Operators

        public D3AttributeValue this[D3Attribute attribute]
        {
            get { return attributes[GetID(attribute)]; }
            set { attributes[GetID(attribute)] = value; }
        }

        public D3AttributeValue this[D3Attribute attribute, int key]
        {
            get { return attributes[GetID(attribute, key)]; }
            set { attributes[GetID(attribute, key)] = value; }
        }

        public D3AttributeValue this[uint id]
        {
            get { return attributes[id]; }
            set { attributes[id] = value; }
        }

        #endregion Operators

        private uint GetID(D3Attribute attribute)
        {
            return (uint)attribute.ID | Offsets.ATTRIBUTE_MASK;
        }

        private uint GetID(D3Attribute attribute, int key)
        {
            return ((uint)attribute.ID & 0xFFF) | ((uint)key << 12);
        }
    }
}
