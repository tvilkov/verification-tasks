using System;
using System.Text.RegularExpressions;

namespace Kontur.ImageTransformer.Filters
{
    internal class FiltersRegistry
    {
        // PERF: 
        // Filters are stateless, so we can pre-create and cache them to avoid multiple memory allocations        
        private readonly ImageFilter m_Sepia;
        private readonly ImageFilter m_Grayscale;
        private readonly ImageFilter[] m_Thresholds;

        public FiltersRegistry()
        {
            m_Sepia = new SepiaFilter();
            m_Grayscale = new GrayscaleFilter();
            m_Thresholds = new ImageFilter[101];
            for (var i = 0; i < m_Thresholds.Length; i++)
            {
                m_Thresholds[i] = new ThresholdFilter(i);
            }
        }

        public virtual ImageFilter GetFilterByDefinition(string filterDefinition)
        {
            if (filterDefinition == null) throw new ArgumentNullException(nameof(filterDefinition));

            if (string.Equals(filterDefinition, "sepia", StringComparison.OrdinalIgnoreCase))
            {
                return m_Sepia;
            }

            if (string.Equals(filterDefinition, "grayscale", StringComparison.OrdinalIgnoreCase))
            {
                return m_Grayscale;
            }

            if (tryParseThresholdFilterDefinition(filterDefinition, out var threathold) && threathold >= 0 && threathold <= 100)
            {
                return m_Thresholds[threathold];
            }

            return null;
        }

        private readonly Regex m_ThresholdFilterDefinitionRegex = new Regex(@"^threshold\((?<val>\d{1,3})\)$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline, TimeSpan.FromSeconds(1));

        private bool tryParseThresholdFilterDefinition(string input, out int threathold)
        {
            threathold = -1;
            var match = m_ThresholdFilterDefinitionRegex.Match(input);
            return match.Success && int.TryParse(match.Groups["val"].Value, out threathold);
        }
    }
}