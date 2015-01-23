using System;
using System.Diagnostics;

namespace OpenBank.Core.Data
{
    public sealed class TrendsGenerator : IDataGenerator
    {
        private readonly int m_LowerBound;
        private readonly int m_UpperBound;
        private readonly int m_MaxDelta;
        private readonly int m_MinTrendLength;
        private readonly int m_MaxTrendLength;
        private readonly Random m_Random;
        private int m_CurrenValue;
        private int m_TrendDirection;
        private int m_TrendLength;

        public TrendsGenerator(int lowerBound = 10, int upperBound = 70, int maxDelta = 5, int minTrendLength = 2, int maxTrendLength = 15)
        {
            if (maxDelta <= 0) throw new ArgumentException("maxDelta must be a positive value", "maxDelta");
            if (minTrendLength <= 0) throw new ArgumentException("minTrendLength must be a positive value", "minTrendLength");
            if (maxTrendLength <= 0) throw new ArgumentException("maxTrendLength must be a positive value", "maxTrendLength");
            if (lowerBound >= upperBound) throw new ArgumentException("lowerBound must be less then upperBound");
            if (minTrendLength >= maxTrendLength) throw new ArgumentException("minTrendLength must be less maxTrendLength");

            m_LowerBound = lowerBound;
            m_UpperBound = upperBound;
            m_MaxDelta = maxDelta;
            m_MinTrendLength = minTrendLength;
            m_MaxTrendLength = maxTrendLength;
            m_Random = new Random(DateTime.Now.Millisecond);
            init();
        }

        public int GetNextValue()
        {
            // If current trend is completed - start a new one with opposite direction
            if (m_TrendLength-- < 0)
            {
                m_TrendDirection = -1 * m_TrendDirection;
                m_TrendLength = m_Random.Next(m_MinTrendLength, m_MaxTrendLength);
                Debug.WriteLine("Change trend. New direction: {0}, trend len: {1}", m_TrendDirection, m_TrendLength);
            }

            // Generate next value
            int min, max;
            if (m_TrendDirection > 0)
            {
                min = Math.Min(m_CurrenValue + 1, m_UpperBound);
                max = Math.Min(m_CurrenValue + m_MaxDelta, m_UpperBound);
            }
            else
            {
                max = Math.Max(m_CurrenValue - 1, m_LowerBound);
                min = Math.Max(m_CurrenValue - m_MaxDelta, m_LowerBound);
            }

            m_CurrenValue = m_Random.Next(min, max);
            return m_CurrenValue;
        }

        private void init()
        {
            m_CurrenValue = m_Random.Next(m_LowerBound, m_UpperBound);
            m_TrendDirection = m_CurrenValue > (m_UpperBound - m_LowerBound) / 2 ? -1 : 1;
            m_TrendLength = m_Random.Next(m_MinTrendLength, m_MaxTrendLength);
            Debug.WriteLine("Initialized. Value: {0}, direction: {1}, trend len: {2}", m_CurrenValue, m_TrendDirection, m_TrendLength);
        }
    }

}