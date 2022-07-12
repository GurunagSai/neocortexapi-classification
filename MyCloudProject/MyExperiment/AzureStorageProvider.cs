using Microsoft.Extensions.Configuration;
using MyCloudProject.Common;
using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.Cosmos.Table;
using Azure.Storage.Blobs.Models;

namespace MyExperiment
{
    public class AzureStorageProvider : IStorageProvider
    {
        private string connectionString;
        private Microsoft.Azure.Storage.CloudStorageAccount storageAccount;


        /// <summary>
        /// Construct a new instance of <see cref="AzureStorageProvider"/> by connection string from parameter
        /// </summary>
        /// <param name="configSection"><see cref="IConfigurationSection"/> instance containing the connection string</param>
        public AzureStorageProvider(IConfigurationSection configSection)
        {

            connectionString = configSection.GetValue<string>("StorageConnectionString");

            // Create handle for the Azure Storage Account as well as check if the connection string is legit
            try
            {
                storageAccount = Microsoft.Azure.Storage.CloudStorageAccount.Parse(connectionString);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                Console.ReadLine();
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                Console.ReadLine();
                throw;
            }
        }

        /// <summary>
        /// Upload the test file to Azure Blob Storage Container
        /// </summary>
        /// <param name="trainingContainerName">Name of the container, to which the image file is uploaded</param>
        /// <param name="LocalfilePath">Local Path to the image file</param>
        /// <param name="trainingblobToUpload">Sample image file for experiment</param>
        /// <returns>Return the URI of the blob (uploaded test file)</returns>
        public async Task<string> UploadTestFileAsync(string trainingContainerName, string LocalfilePath, string trainingblobToUpload)
        {
            // Create client for Azure Blob Storage Container Service
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(trainingContainerName);
            if (!containerClient.Exists())
            {
                Console.WriteLine("Creating new test blob container: {0}", trainingContainerName);
            }
            await containerClient.CreateIfNotExistsAsync();

            BlobClient blobClient = containerClient.GetBlobClient(trainingblobToUpload);

            // Upload the file to the blob container
            Console.WriteLine("\nUploading image as blob: \"{0}\"\n(URI: {1})\n--------------\n", trainingblobToUpload, blobClient.Uri.ToString());
            using FileStream uploadFileStream = File.OpenRead(LocalfilePath);
            await blobClient.UploadAsync(uploadFileStream, true);
            uploadFileStream.Close();

            return blobClient.Uri.ToString();
        }

        /// <summary>
        /// Download a blob from Azure Blob Storage Service
        /// </summary>
        /// <param name="trainingContainerName">Name of the container to download blob from</param>
        /// <param name="LocalfilePath">Local Path to the image file</param>
        /// <param name="blobToDownload">Path to the blob to download</param>
        /// <returns>Return the URI of the downloaded blob</returns>
        public async Task<string> DownloadInputFileAsync(string trainingContainerName, string LocalfilePath, string blobToDownload)
        {
            // Create client for Azure Blob Storage Service
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(trainingContainerName);

            try
            {
                // Check if the specified container exists
                if (!containerClient.Exists())
                {
                    throw new Exception("The specified \"TrainingContainer\" in \"appsettings.json\" does not exist in the current Azure Storage Account");
                }

                // Download the blob's content

                BlobClient blobClient = containerClient.GetBlobClient(blobToDownload);
                Console.WriteLine("\nDownloading input blob:\"{0}\"\n(URI: {1})\n--------------\n", blobToDownload, blobClient.Uri.ToString());
                BlobDownloadInfo download = await blobClient.DownloadAsync();

                // Check if the specified blob exists
                if (download.ContentLength < 1)
                {
                    throw new Exception("The specified blob does not exist in the given Azure Blob Container!");
                }

                // Save the downloaded blob to a file
                using (FileStream downloadFileStream = File.OpenWrite(LocalfilePath))
                {
                    await download.Content.CopyToAsync(downloadFileStream);
                    downloadFileStream.Close();
                }
                return blobClient.Uri.ToString();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        /// <summary>
        /// Upload a file to a Azure Blob Storage Container
        /// </summary>
        /// <param name="resultBlobContainerName">Name of the container, to which the file is uploaded</param>
        /// <param name="resultfilePath">Local Path to the image file</param>
        /// <param name="resultblobToUpload">Path to the file to upload</param>
        /// <returns>Return the URI of the blob (uploaded file)</returns>
        public async Task<string> UploadResultFileAsync(string resultBlobContainerName, string resultfilePath, string resultblobToUpload)
        {
            // Create client for Azure Blob Storage Container Service
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(resultBlobContainerName);
            if (!containerClient.Exists())
            {
                Console.WriteLine("Creating new result blob container: {0}", resultBlobContainerName);
            }
            await containerClient.CreateIfNotExistsAsync();
            BlobClient blobClient = containerClient.GetBlobClient(resultblobToUpload);

            // Upload the file to the blob container
            Console.WriteLine("\nUploading the binarized image as a blob: \"{0}\"\n(URI: {1})\n--------------\n", resultblobToUpload, blobClient.Uri.ToString());
            using FileStream uploadFileStream = File.OpenRead(resultfilePath);
            await blobClient.UploadAsync(uploadFileStream, true);
            uploadFileStream.Close();



            return blobClient.Uri.ToString();
        }

        /// <summary>
        /// Upload result of the Experiment to Azure Table Storage
        /// </summary>
        /// <param name="entity">The <see cref="ExperimentResult"/> object to upload to the Azure Table Storage</param>
        /// <param name="resultTableName">Name of the Azure Table, to which the result is uploaded</param>
        /// <returns></returns>
        public async Task UploadExperimentResultAsync(string resultTableName, ExperimentResult entity)
        {
            // Create client for the Azure Table Storage Service
            Microsoft.Azure.Cosmos.Table.CloudStorageAccount cosmos_storageAccount = Microsoft.Azure.Cosmos.Table.CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = cosmos_storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            CloudTable table = tableClient.GetTableReference(resultTableName);

            // Create the specified table if it does not already exist
            if (await table.CreateIfNotExistsAsync())
            {
                Console.WriteLine("Created Table named: {0}\n--------------\n", resultTableName);
            }
            else
            {
                Console.WriteLine("Table {0} already exists\n--------------\n", resultTableName);
            }

            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            try
            {
                // Create the InsertOrReplace table operation
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

                // Execute the operation.
                TableResult result = await table.ExecuteAsync(insertOrMergeOperation);

                Console.WriteLine("Uploaded the following result:\n");
                Console.WriteLine(entity.AsString() + "\n--------------\n");

            }
            catch (Microsoft.Azure.Cosmos.Table.StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        /// <summary>
        /// Create an Azure Queue Storage 
        /// </summary>
        /// <param name="queueName">Name of the queue</param>
        /// <returns>Return a <see cref="CloudQueue"/> object</returns>
        public async Task<CloudQueue> CreateQueueAsync(string queueName)
        {
            // Create client for Azure Queue Storage Service
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(queueName);

            Console.WriteLine("Creating Azure Queue: {0} ...\n--------------\n", queueName);
            try
            {
                if (await queue.CreateIfNotExistsAsync())
                {
                    Console.WriteLine("Created queue: {0}\n--------------\n", queueName);
                }
                else
                {
                    Console.WriteLine("Queue {0} already exists\n--------------\n", queueName);
                }
            }
            catch
            {
                Console.WriteLine("If you are running with the default configuration please make sure you have started the storage emulator. Press the Windows key and type Azure Storage to select and run it from the list of applications - then restart the sample.");
                Console.ReadLine();
                throw;
            }


            return queue;
        }
    }
}