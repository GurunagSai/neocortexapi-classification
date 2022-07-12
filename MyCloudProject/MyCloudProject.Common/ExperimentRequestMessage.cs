using System;
using System.Collections.Generic;
using System.Text;

namespace MyCloudProject.Common
{
    public class ExperimentRequestMessage
    {
        // Reference Tag Per Experiment
        public string ExperimentID { get; set; }

        // Image url 
        public string InputFile { get; set; }

        // Name of the Person requesting Experiment
        public string Name { get; set; }

        // Experiment Remarks
        public string Description { get; set; }

    }
}
