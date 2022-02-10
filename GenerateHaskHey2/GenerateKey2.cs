using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace GenerateHaskHey2
{
    public static class GenerateKey2
    {
        [FunctionName("GenerateKey2")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Generate MD5Hash Started");

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            // parse api key parameter
            string apiKey = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "apikey", true) == 0)
                .Value;

            if (apiKey == null)
            {
                apiKey = data?.apikey;
            }

            // parse api secret parameter
            string apiSecret = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "apisecret", true) == 0)
                .Value;

            if (apiSecret == null)
            {
                apiSecret = data?.apisecret;
            }

            // parse Time parameter
            string timeStamp = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "timestamp", true) == 0)
                .Value;

            if (timeStamp == null)
            {
                timeStamp = data?.timestamp;
            }

            //var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            //var timestamp = "2022-02-08T03:35:09.831Z";


            var input = $"{apiKey}/{timeStamp}/{apiSecret}";

            var hash = MD5Hash(input);

            return hash == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Empty")
                : req.CreateResponse(HttpStatusCode.OK, hash);
        }

        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
    }
}
