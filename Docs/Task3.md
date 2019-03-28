# Zadanie 3

W ramach tego zadania należy stworzyć potok służący do szyfrowania wszystkich przesłanych plików tekstowych. W ramach potoku należy:

- skonfigurować wyzwalacz uruchamiający potok, gdy na magazynie danych pojawi się nowy plik tekstowy (o rozszerzeniu *.txt) wewnątrz kontenera **to_encrypt**,

- dodać activity zapisujące do stworzonej wcześniej bazy danych informacje o nazwie pliku do zaszyfrowania, czasie jego spłynięcia do magazynu (stored procedure). Ponadto procedura powinna ustawić dwa wartości dwóch dodatkowych atrybutów mianowicie **IN_PROGRESS** (na **1**) oraz **COMPLETED** (na **0**) w celu zwizualizowania aktualnego stanu szyfrowanych plików,

- dodać kolejne activity uruchamiające przedstawioną poniżej funkcję szyfrującą (kod znajduje się na samym dole strony), za pośrednictwem usługi **Azure Functions**,

- dodać kolejne activity aktualizujące dodany wcześniej wpis ustawiający flagę **IN_PROGRESS** na **0** oraz **COMPLETED** na **1**,

```
public async static Task<JObject> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            // getting data from request's body
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string directory = data.directory;
            string filename = data.filename;
            string destination = "encrypted";
            string ciphertext;

            log.Info($"Source directory: {directory}");
            log.Info($"Filename to encrypt: {filename}");

            // downloading blobs content
            string storageConnection = "DefaultEndpointsProtocol=https;AccountName=adfworkshopsasm;AccountKey=N/0QJpS6vmpNk6M2oyPjQNTBME8cQyj9tgVCsFylLEgRKhdXvZXRRBi8JSJSrQsRc0GkWijWQrNv6spbIBbpAA==;EndpointSuffix=core.windows.net";
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnection);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(directory);
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(filename);

            string plaintext = await cloudBlockBlob.DownloadTextAsync();

            // encrypting message using AES
            using (RijndaelManaged aes = new RijndaelManaged())
            {
                aes.GenerateKey();
                aes.GenerateIV();
                byte[] encrypted = EncryptStringToBytes(plaintext, aes.Key, aes.IV);
                StringBuilder sb = new StringBuilder();
                foreach (byte item in encrypted)
                {
                    sb.Append(item.ToString("X2") + " ");
                }

                ciphertext = sb.ToString();
            }

            // saving ciphertexts in blob
            CloudBlobContainer container = cloudBlobClient.GetContainerReference(destination);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(filename);
            
            await container.CreateIfNotExistsAsync();
            await blockBlob.UploadTextAsync(ciphertext);

            var result = new JObject();
            result.Add(new JProperty("Status", "OK"));
            return result;
        }

        static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            byte[] encrypted;
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }

                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;
        }
```
