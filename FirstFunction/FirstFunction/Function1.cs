using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace FirstFunction
{
    public static class Function1
    {   
        [FunctionName("Function1")]
        public static async Task Run([TimerTrigger("*/1 * * * * ")] TimerInfo myTimer, ILogger log,
            [Table("logs",Connection = "AzureWebJobsStorage")] IAsyncCollector<TableStorage> table,
            [Blob("sample-images/{rand-guid}​​", FileAccess.Write, Connection = "AzureWebJobsStorage")] Stream image)
        {
            var message = await GetResponse();
            if (message.IsSuccessStatusCode)
            {
                string strResult = await message.Content.ReadAsStringAsync();
                await image.WriteAsync(Encoding.UTF8.GetBytes(strResult));
            }

            await table.AddAsync(new TableStorage { PartitionKey = "log", RowKey = Guid.NewGuid().ToString(), Log = message.StatusCode.ToString()});
        }

        private static async Task<HttpResponseMessage> GetResponse()
        {
            string url = "https://api.publicapis.org/random?auth=null";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.BaseAddress = new Uri(url);

                HttpResponseMessage response = await client.GetAsync(url);

                return response;
            }

        }
    }
}
