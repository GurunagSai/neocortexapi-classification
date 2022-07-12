using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Queue;

namespace MyCloudProject.Common
{
    public interface IStorageProvider
    {
        /// <summary>
        /// IStorageProvider Interface
        /// </summary>
        /// <param name="fileName">The name of the local file where the input is downloaded.</param>
        Task<string> UploadTestFileAsync(string trainingContainer, string LocalfilePath, string trainingblobToUpload);

        Task<string> DownloadInputFileAsync(string trainingContainerName, string LocalfilePath, string blobToDownload);

        Task<string> UploadResultFileAsync(string resultBlobContainerName, string resultfilePath, string resultblobToUpload);

        Task UploadExperimentResultAsync(string resultTableName, ExperimentResult entity);

        Task<CloudQueue> CreateQueueAsync(string queueName);
    }
}
