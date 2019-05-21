using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime.CredentialManagement;
using Amazon.StepFunctions;
using Amazon.StepFunctions.Model;
using Newtonsoft.Json;

namespace MyStepFunctionStarter
{
    class Program
    {
        static void Main(string[] args)
        {
            //CreateCredentialProfile();

            Task.Run(async () => await StartAndMonitorExecution()).Wait();

            Console.WriteLine();
            Console.WriteLine("Done. Press [Enter] or [Return]");
            Console.ReadLine();
        }

        /// <summary>
        /// Registra o profile a ser usado.
        /// Pra se chamadao apenas uma vez.
        /// </summary>
        static void CreateCredentialProfile()
        {
            var options = new CredentialProfileOptions
            {
                AccessKey = "AKIAZDL54SP7JOWTQJFS",
                SecretKey = "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"
            };

            var profile = new CredentialProfile("basic_profile", options);
            profile.Region = RegionEndpoint.SAEast1;
            var netSDKFile = new NetSDKCredentialsFile();
            netSDKFile.RegisterProfile(profile);
        }

        static async Task StartAndMonitorExecution()
        {
            string executionName = Guid.NewGuid().ToString();

            using (var client = new AmazonStepFunctionsClient(RegionEndpoint.SAEast1))
            {
                // manda executar o Step Function
                var request = new StartExecutionRequest
                {
                    Input = "{\"Name\": \"Joelsonildomarisol\"}",
                    Name = executionName,
                    StateMachineArn = "arn:aws:states:sa-east-1:625718629374:stateMachine:StateMachine-GXfZlGTLUzOM"
                };

                var response = await client.StartExecutionAsync(request);
                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine($"Err: {response.HttpStatusCode}");
                    return;
                }

                // verifica o estado da execução
                var requestStatus = new DescribeExecutionRequest
                {
                    ExecutionArn = $"arn:aws:states:sa-east-1:625718629374:execution:StateMachine-GXfZlGTLUzOM:{executionName}",
                };
                while (true)
                {
                    var responseStatus = await client.DescribeExecutionAsync(requestStatus);
                    if (responseStatus.HttpStatusCode == System.Net.HttpStatusCode.OK && responseStatus.Status == ExecutionStatus.SUCCEEDED)
                    {
                        dynamic parsed = JsonConvert.DeserializeObject(responseStatus.Output);
                        Console.WriteLine(JsonConvert.SerializeObject(parsed, Formatting.Indented));
                        break;
                    }

                    Console.WriteLine($"Rec: {responseStatus.Status} - will delay for 1500ms before retry...");
                    await Task.Delay(1500);
                }
            }
        }
    }


}
