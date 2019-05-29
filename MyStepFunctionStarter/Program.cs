using System;
using System.Dynamic;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime.CredentialManagement;
using Amazon.StepFunctions;
using Amazon.StepFunctions.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MyStepFunctionStarter
{
    public static class Program
    {
        private const string _stateMachineArn = "arn:aws:states:sa-east-1:625718629374:stateMachine:StateMachine-JXS9zp6wTc8S";
        private const string _executionArn = "arn:aws:states:sa-east-1:625718629374:execution:StateMachine-JXS9zp6wTc8S";

        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            },
            NullValueHandling = NullValueHandling.Include
        };

        private static readonly Random _rnd = new Random();

        public static void Main(string[] args)
        {
            Task.Run(async () => await StartAndMonitorExecution().ConfigureAwait(false)).Wait();

            Console.WriteLine();
            Console.WriteLine("Done. Press [Enter] or [Return]...");
            Console.ReadLine();
        }

        private static async Task StartAndMonitorExecution()
        {
            string executionName = Guid.NewGuid().ToString();

            using (var client = new AmazonStepFunctionsClient(RegionEndpoint.SAEast1))
            {
                // manda executar o Step Function
                var payload = new SimpleStepFunctionInput
                {
                    Name = "pirilampo",
                    Number = _rnd.Next()
                };

                var request = new StartExecutionRequest
                {
                    Input = JsonConvert.SerializeObject(payload, Formatting.None, _jsonSerializerSettings),
                    Name = executionName,
                    StateMachineArn = _stateMachineArn
                };

                Console.WriteLine("Sending:");
                Console.WriteLine(request.Input);

                var response = await client.StartExecutionAsync(request).ConfigureAwait(false);
                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine($"Err: {response.HttpStatusCode}");
                    return;
                }

                // verifica o estado da execução
                var requestStatus = new DescribeExecutionRequest
                {
                    ExecutionArn = $"{_executionArn}:{executionName}",
                };
                while (true)
                {
                    var responseStatus = await client.DescribeExecutionAsync(requestStatus).ConfigureAwait(false);
                    if (responseStatus.HttpStatusCode == System.Net.HttpStatusCode.OK && responseStatus.Status == ExecutionStatus.SUCCEEDED)
                    {
                        dynamic parsed = JsonConvert.DeserializeObject(responseStatus.Output);
                        Console.WriteLine(JsonConvert.SerializeObject(parsed, Formatting.Indented, _jsonSerializerSettings));
                        break;
                    }

                    Console.WriteLine($"{responseStatus.Status}...");
                    await Task.Delay(1500).ConfigureAwait(false);
                }
            }
        }
    }
}
