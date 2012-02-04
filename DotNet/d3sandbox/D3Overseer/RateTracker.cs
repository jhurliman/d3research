using System;
using System.Collections.Generic;

namespace D3Overseer
{
    public class RateTracker
    {
        private TimeSpan maxAge;
        private Queue<Tuple<DateTime, double>> values;

        public double PerHour
        {
            get
            {
                if (values.Count == 0)
                    return 0.0;

                // Remove expired values
                DateTime now = DateTime.UtcNow;
                while (now - values.Peek().Item1 > maxAge)
                    values.Dequeue();

                DateTime oldest = values.Peek().Item1;
                double sum = 0.0;

                foreach (var entry in values)
                    sum += entry.Item2;

                return sum / (now - oldest).TotalHours;
            }
        }

        public RateTracker(TimeSpan maxAge)
        {
            this.maxAge = maxAge;
            this.values = new Queue<Tuple<DateTime, double>>();
        }

        public void AddValue(double value)
        {
            DateTime now = DateTime.UtcNow;

            if (values.Count > 0)
            {
                // Remove expired values
                while (now - values.Peek().Item1 > maxAge)
                    values.Dequeue();
            }

            values.Enqueue(new Tuple<DateTime, double>(now, value));
        }
    }
}
