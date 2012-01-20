using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mooege.Common.Storage
{
    public class PersistentPropertyAttribute : Attribute
    {
        public string Name { get; private set; }
        public int Count { get; private set; }

        public PersistentPropertyAttribute(string name) { Name = name; Count = 1; }
        public PersistentPropertyAttribute() { }
        public PersistentPropertyAttribute(string name, int count) { Name = name; Count = count; }
    }
}
