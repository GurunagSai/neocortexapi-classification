using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Extensions.Logging;
using MyCloudProject.Common;
using System;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace MyExperiment
{
    public class Experiment : IExperiment
    {

        public static string ExperimentData { get; private set; }

        public string localfilename { get; private set; }

        public string localFilePath { get; private set; }

        public string resultFilePath { get; set; }

        private IStorageProvider storageProvider;

        private ILogger logger;

        public MyConfig config;

        private ExperimentResult experiment = new ExperimentResult("null", "null");

        public Experiment(IConfigurationSection configSection, IStorageProvider storageProvider, ILogger log)
        {
            this.storageProvider = storageProvider;
            this.logger = log;
            config = new MyConfig();
            configSection.Bind(config);

            /// Creates local directory where the blob files will be downloaded and uploaded from
            ExperimentData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            localfilename = "image-" + Guid.NewGuid().ToString() + ".png";
            localFilePath = Path.Combine(ExperimentData, localfilename);

        }

        /// <summary>
        /// Run the experiment using the specified input file
        /// </summary>
        /// <param name="experimentImageFile">Input file</param>
        /// <returns></returns>
        public Task<ExperimentResult> Run(string experimentImageFile)
        {            

            ExperimentResult res = new ExperimentResult(this.config.GroupId, Guid.NewGuid().ToString());

            // Recording start time
            res.StartTimeUtc = DateTime.UtcNow;
            Console.WriteLine("Image Binarization Experiment Started...\n--------------\n");

            // Reference to the SE Project Experiment
            Unitest ImageBinarizerTest = new Unitest();
            resultFilePath = ImageBinarizerTest.TestMethod1(experimentImageFile);

            //  Recording stop time
            res.EndTimeUtc = DateTime.UtcNow;
            Console.WriteLine("Image Binarization Experiment Finished...\n--------------\n");

            //UpdateExperimentResult(res, res.StartTimeUtc, res.EndTimeUtc, experimentImageFile);
            return Task.FromResult<ExperimentResult>(res);
        }

        /// <inheritdoc/>
        public async Task RunQueueListener(CancellationToken cancelToken)
        {
            CloudQueue queue = await CreateQueueAsync(config);

            while (cancelToken.IsCancellationRequested == false)
            {
                CloudQueueMessage message = await queue.GetMessageAsync();
                if (message != null)
                {
                    try
                    {
                        this.logger?.LogInformation($"Received the message {message.AsString}");


                        // Read message from the Queue and Deserialize
                        ExperimentRequestMessage msg = JsonConvert.DeserializeObject<ExperimentRequestMessage>(message.AsString);

                        // Assign the input image Url to a string 
                        var fileToDownload = msg.InputFile;

                        // Use Webclient to accept Image from any Image Hosting Platform
                        WebClient myWebClient = new WebClient();
                        Uri imageUrl = new Uri(fileToDownload);

                        // Save the image file to local directory
                        myWebClient.DownloadFile(imageUrl, localFilePath);

                        // Create reference to the Training Container
                        string TestcontainerName = config.TrainingContainer;

                        // Store the sample image in Blobstorage
                        string trainingImageUri = await this.storageProvider.UploadTestFileAsync(TestcontainerName, localFilePath, localfilename);

                        // Create a specific reference for the blob to be downloaded
                        string downloadedFilePath = localFilePath.Replace(".png", "DOWNLOADED.png");

                        // Download the blob from BlobStorage for the experiment
                        string inputImageUri = await this.storageProvider.DownloadInputFileAsync(TestcontainerName, downloadedFilePath, localfilename);

                        // Run the experiment using downloaded blob
                        experiment = await this.Run(downloadedFilePath);

                        // Create a specific reference for the result file
                        string resultFilename = localfilename.Replace(".png", ".txt");

                        // Upload result file and update ExperimentResult object                        
                        experiment.RowKey = msg.ExperimentID;
                        experiment.ExperimentID = msg.ExperimentID;
                        experiment.Name = msg.Name;
                        experiment.Description = msg.Description;
                        experiment.TrainingImageUrl = msg.InputFile;
                        experiment.InputBlobUri = inputImageUri;
                        experiment.ResultFile = resultFilename;
                        experiment.ResultBlobUri = await this.storageProvider.UploadResultFileAsync(config.ResultContainer, resultFilePath, resultFilename);
                        this.logger?.LogInformation("Finished uploading Image Binarization result");

                        // Upload the ExperimentResult object to Azure Table Storage
                        await storageProvider.UploadExperimentResultAsync(config.ResultTable, experiment);
                        this.logger?.LogInformation("Uploaded result");

                        await queue.DeleteMessageAsync(message);
                    }
                    catch (Exception ex)
                    {
                        this.logger?.LogError(ex, "Caught an exception: {0}", ex.Message);
                    }
                }
                else
                    await Task.Delay(500);
            }

            this.logger?.LogInformation("Cancel pressed. Exiting the listener loop.");
        }


        #region Private Methods


        /// <summary>
        /// Validate the connection string information in app.config and throws an exception if it looks like 
        /// the user hasn't updated this to valid values. 
        /// </summary>
        /// <param name="storageConnectionString">The storage connection string</param>
        /// <returns>CloudStorageAccount object</returns>
        private static CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
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

            return storageAccount;
        }

        /// <summary>
        /// Create a queue for the sample application to process messages in. 
        /// </summary>
        /// <returns>A CloudQueue object</returns>
        private static async Task<CloudQueue> CreateQueueAsync(MyConfig config)
        {
            // Retrieve storage account information from connection string.
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(config.StorageConnectionString);

            // Create a queue client for interacting with the queue service
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            Console.WriteLine("... Created Azure Queue {0}", config.Queue);

            CloudQueue queue = queueClient.GetQueueReference(config.Queue);
            try
            {
                await queue.CreateIfNotExistsAsync();
            }
            catch
            {
                Console.WriteLine("If you are running with the default configuration please make sure you have started the storage emulator.  ess the Windows key and type Azure Storage to select and run it from the list of applications - then restart the sample.");
                Console.ReadLine();
                throw;
            }

            return queue;
        }
        #endregion
    }
}

