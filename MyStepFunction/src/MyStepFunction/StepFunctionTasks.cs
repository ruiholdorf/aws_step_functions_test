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
        private static Random _rnd = new Random();

        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public StepFunctionTasks()
        {
        }


        public State Greeting(State state, ILambdaContext context)
        {
            state.Message = "Hello";
            if (!string.IsNullOrEmpty(state.Name))
                state.Message += " " + state.Name;

            state.TimeStamp = DateTime.UtcNow;
            return state;
        }

        public State Salutations(State state, ILambdaContext context)
        {
            state.Message += ", Goodbye";
            if (!string.IsNullOrEmpty(state.Name))
                state.Message += " " + state.Name;

            state.TimeStamp = DateTime.UtcNow;
            return state;
        }

        public State PickRandomInteger(State state, ILambdaContext context)
        {
            state.TheNumber = _rnd.Next(1, 100);
            state.TimeStamp = DateTime.UtcNow;
            return state;
        }

        public State SetPar(State state, ILambdaContext context)
        {
            state.IsEven = true;
            state.TimeStamp = DateTime.UtcNow;
            return state;
        }

        public State SetImpar(State state, ILambdaContext context)
        {
            state.IsOdd = true;
            state.TimeStamp = DateTime.UtcNow;
            return state;
        }
    }
}
