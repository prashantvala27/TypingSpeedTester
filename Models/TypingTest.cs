using System;

namespace TypingSpeedTester.Models
{
    public class TypingTest
    {
        public int TestId { get; set; }
        public string UserName { get; set; }
        public DateTime TestDate { get; set; }
        public int WPM { get; set; }
        public decimal Accuracy { get; set; }
        public int ErrorCount { get; set; }
        public int TimeDuration { get; set; }
        public string TestText { get; set; }
        public string TypedText { get; set; }
    }
}