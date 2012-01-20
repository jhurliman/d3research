using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libdiablo3.Process
{
    public class MemoryReadException : Exception
    {
        public MemoryReadException(string msg, uint ptr)
            : base(String.Format("Failed to read memory from 0x{0:X}: {1}", ptr, msg))
        {
        }
    }
}
