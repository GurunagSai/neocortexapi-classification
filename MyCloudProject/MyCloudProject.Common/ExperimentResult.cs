using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCloudProject.Common
{
    public class ExperimentResult : TableEntity
    {
        public ExperimentResult(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public string ExperimentID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartTimeUtc { get; set; }

        public DateTime EndTimeUtc { get; set; }

        public string TrainingImageUrl { get; set; }

        public string InputBlobUri { get; set; }

        public string ResultFile { get; set; }

        public string ResultBlobUri { get; set; }

        public string AsString()
        {
            string s = "{\n" +
                "\"ExperimentID\" : \"" + ExperimentID + "\",\n" +
                "\"Name\" : \"" + Name + "\",\n" +
                "\"Description\" : \"" + Description + "\",\n" +
                "\"StartTimeUtc\" : \"" + StartTimeUtc.ToString() + "\",\n" +
                "\"EndTimeUtc\" : \"" + EndTimeUtc.ToString() + "\",\n" +
                "\"trainingFileUri\" : \"" + TrainingImageUrl + "\",\n" +
                "\"inputFileUri\" : \"" + InputBlobUri + "\",\n" +
                "\"resultFile\" : \"" + ResultFile + "\",\n" +
                "\"outputFileUri\" : \"" + ResultBlobUri + "\",\n" +
                "\"PartitionKey\" : \"" + this.PartitionKey + "\",\n" +
                "\"RowKey\" : \"" + this.RowKey + "\",\n" +
                "\"Timestamp\" : \"" + this.Timestamp.ToString() + "\",\n" +
                "\"ETag\" : \"" + this.ETag + "\"\n}\n";

            return s;
        }

    }
}
