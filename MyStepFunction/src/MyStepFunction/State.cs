using System;
using System.Collections.Generic;
using System.Text;

namespace MyStepFunction
{
    /// <summary>
    /// The state passed between the step function executions.
    /// </summary>
    public class State
    {
        /// <summary>
        /// Input value when starting the execution
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The message built through the step function execution.
        /// </summary>
        public string Message { get; set; }

        public DateTime TimeStamp { get; set; }

        public int TheNumber { get; set; }

        public bool IsOdd { get; set; }

        public bool IsEven { get; set; }

        /// <summary>
        /// The number of seconds to wait between calling the Salutations task and Greeting task.
        /// </summary>
        public int WaitInSeconds { get; set; }
    }
}
