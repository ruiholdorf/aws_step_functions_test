using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace MyStepFunction
{
    public class StepFunctionTasks
    {
        public async Task<State> FetchRandomNumber(State state, ILambdaContext context)
        {
            try
            {
                using (var httpClient = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Get, "https://4xnebbz8yl.execute-api.sa-east-1.amazonaws.com/Prod/api/numbers"))
                using (var response = await httpClient.SendAsync(request).ConfigureAwait(false))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var number = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        state.Message = "recuperou da WebAPI";
                        state.Number = int.Parse(number);
                    }
                    else
                    {
                        state.Number = -1;
                    }
                }
            }
            catch (Exception e)
            {
                state.Message = e.Message;
                state.Number = -2;
            }

            return state;
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
            state.IsEven = state.Number % 2 == 0;
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
