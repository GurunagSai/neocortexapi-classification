
* Arinze Okpagu 

* Uche John Emenike 

Masters Information Technology \
Frankfurt University of Applied Sciences \
Email: bryancoins@yahoo.co.uk, jonemenike2000@gmail.com  

# SEIVX GROUP: ML19/20-3.31 - Implementation of Image Binarizer

## Experiment Description
The image Binarization deals with converting pixel image to binary image. It is a feature extraction process that provides sharper and clearer contours of various objects present in the image. The accuracy of the process are affected by factors such as shadows, non-uniform illumination, low contrast, large signal-dependent noise.
The binary image is produced by quantization of the image grey levels to only two values, usually 0 and 1. Please find the reference to the [Image Binarization Project][1]

The experiment is designed to receive and process image data from any local, intranet, or Internet resource identified by a URI alongside object storage solution for the cloud. The duration of the experiment is less than a minute.

The experiment is designed to explore the following processes:

1. Process image resource identified by a URI:
	```
	WebClient myWebClient = new WebClient();
	Uri imageUrl = new Uri(fileToDownload);
	```

2. Upload a copy of the image resoure  to the Azure Blob Storage:
	```
	string trainingImageUrl = await this.storageProvider.UploadTestFileAsync(TestcontainerName, localFilePath, localfilename);
	```

3. Download image blob from the Azure Blob storage for Binarization

4. Binarize the sample image and save the output file as a blob to Azure Blob storage

5. Save the Experiment result using Azure Table Storage 



## How to run an Experiment

### Experiment Request Message

For instance, an Azure Queue experiment request message, which will trigger the experiment, has the following format:

~~~json
{
"ExperimentId" : "123",
"InputFile" : "https://cdn.pixabay.com/photo/2020/10/03/11/08/girl-5623231_960_720.jpg",
"Name" : "Arinze_Okpagu",
"Description" : "Coloured_Picture",
}
~~~
\
\
``ExperimentId`` 	: The Reference tag or identifier of the experiment. User could change or edit this reference accordingly.\
``InputFile``		: The Url of the image file. This provides for  any standard image identifiable with URI to be submitted for binarization.\
``Name``			: The name of the user requesting the experiment. \
``Description``	: This serves for remarks specific to the testing environment or subject image instances 
\

### "appsettings.json"

~~~json
{
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  },

  "MyConfig": {
    "GroupId": "SEIVX",
    "StorageConnectionString" : "UseDevelopmentStorage=true",
    "TrainingContainer": "seivx-training-files",
    "ResultContainer": "seivx-result-files",
    "ResultTable": "sevixresult",
    "Queue": "seivx-trigger-queue"
  }
}
~~~

The `UseDevelopmentStorage=true` value allows the use of the Microsoft Azure Storage Emulator tool which emulates the Azure Blob, Queue, and Table services for local development purposes. The experiment was tested successfully on the cloud and local environment.

## Experiment Result

For instance, the sample of the experiment will be given as below;

~~~json

{
"ExperimentId" : "123",
"Name" : "Uche Arinze",
"Description" : "Coloured_Picture",
"StartTimeUtc" : "10/5/2020 6:07:15 PM",
"EndTimeUtc" : "10/5/2020 6:07:17 PM",
"trainingFileUri" : "https://blobxstorage.blob.core.windows.net/seivx-training-files/ImageFile67c9f68a-f707-432e-9c4d-c7ebe7cafe5f.png",
"inputFileUri" : "https://blobxstorage.blob.core.windows.net/seivx-training-files/ImageFile67c9f68a-f707-432e-9c4d-c7ebe7cafe5f.png",
"resultFile" : "ImageFile67c9f68a-f707-432e-9c4d-c7ebe7cafe5f.txt",
"outputFileUri" : "https://blobxstorage.blob.core.windows.net/seivx-result-files/ImageFile67c9f68a-f707-432e-9c4d-c7ebe7cafe5f.txt",
"PartitionKey" : "SEIVX",
"RowKey" : "123",
"Timestamp" : "1/1/0001 12:00:00 AM +00:00",
"ETag" : "W/"datetime'2020-10-05T18%3A07%3A17.8201463Z'""
}
~~~

The `inputFileUri` and `resultFile` respectively represents the original image file and the corresponding binarized image interpreted in a text file.
A snapshot of the containers used for the experiment could be viewed [here][2].

[1]: <https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/image-binarizer/MyProject>
[2]: <https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2019-2020/tree/seivx/Source/MyCloudProject/StorageAccount_Snapshots>
