using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace MyStepFunction
{
    public class StepFunctionTasks
    {
        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public StepFunctionTasks()
        {
        }

        public State SetStartTime(State state, ILambdaContext context)
        {
            state.StartTime = DateTime.UtcNow;
            return state;
        }

        public State SetEndTime(State state, ILambdaContext context)
        {
            state.EndTime = DateTime.UtcNow;
            state.TookMs = state.EndTime.Subtract(state.StartTime).TotalMilliseconds;
            return state;
        }

        public State TestIfIsOddOrEven(State state, ILambdaContext context)
        {
            state.IsEven = ((state.Number / 2D) == Math.Abs(state.Number / 2));
            return state;
        }

        public State SetPar(State state, ILambdaContext context)
        {
            state.OddOrEven = "par";
            return state;
        }

        public State SetImpar(State state, ILambdaContext context)
        {
            state.OddOrEven = "ímpar";
            return state;
        }
    }
}
