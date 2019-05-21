using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MyStepFunction
{
    /// <summary>
    /// The state passed between the step function executions.
    /// </summary>
    public class State
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double TookMs { get; set; }
        public int Number { get; set; }
        public bool IsEven { get; set; }
        public string OddOrEven { get; set; }
    }
}
