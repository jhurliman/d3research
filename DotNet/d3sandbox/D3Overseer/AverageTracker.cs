using System;

namespace D3Overseer
{
    public class AverageTracker
    {
        private int length;
        private int position;
        private double[] values;

        public double Average
        {
            get
            {
                int curLen = length;

                double sum = 0.0;
                for (int i = 0; i < curLen; i++)
                    sum += values[i];

                return sum / curLen;
            }
        }

        public double Last
        {
            get
            {
                int curPos = position;
                return (curPos == 0) ? values[values.Length - 1] : values[curPos - 1];
            }
        }

        public AverageTracker(int capacity)
        {
            values = new double[capacity];
        }

        public void AddValue(double value)
        {
            values[position++ % values.Length] = value;
            length = Math.Min(length + 1, values.Length);
        }
    }
}
