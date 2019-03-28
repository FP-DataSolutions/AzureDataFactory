using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.WindowsAzure.Storage;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace FunctionApp1
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<JObject> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            // getting data from request's body
            const string destination = "encrypted";
            const string storageConnection = "PUT_CONNECTIONSTRING_HERE";
            var requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string directory = data.directory;
            string filename = data.filename;
            string ciphertext;

            log.Info($"Source directory: {directory}");
            log.Info($"Filename to encrypt: {filename}");

            // downloading blobs content
            var cloudStorageAccount = CloudStorageAccount.Parse(storageConnection);
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            var cloudBlobContainer = cloudBlobClient.GetContainerReference(directory);
            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(filename);
            var plaintext = await cloudBlockBlob.DownloadTextAsync();

            // encrypting message using AES
            using (var aes = new RijndaelManaged())
            {
                aes.GenerateKey();
                aes.GenerateIV();
                var encrypted = EncryptStringToBytes(plaintext, aes.Key, aes.IV);
                var sb = new StringBuilder();
                foreach (var item in encrypted)
                {
                    sb.Append(item.ToString("X2") + " ");
                }

                ciphertext = sb.ToString();
            }

            // saving ciphertexts in blob
            var container = cloudBlobClient.GetContainerReference(destination);
            var blockBlob = container.GetBlockBlobReference(filename);
            
            await container.CreateIfNotExistsAsync();
            await blockBlob.UploadTextAsync(ciphertext);
            return new JObject { new JProperty("Status", "OK") };
        }

        private static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            byte[] encrypted;
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = key;
                rijAlg.IV = iv;

                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }

                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;
        }
    }
}
