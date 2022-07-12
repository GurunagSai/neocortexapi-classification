using ImageBinarizerLib;
using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace MyExperiment
{
    /// <summary>
    /// Main class for testing of Image Binarization algorithm using IPipeline
    /// </summary>

    public class Unitest
    {
        /// <summary>
        /// This method is used to Test the Algorithm by taking the Input Image and setting the parameters
        /// Bitmap image will be loaded from Image Folder and converted into double[,,].
        /// After that Image Binarization will be executed and hence the Binarized Image will be formed.
        /// </summary>
        public string TestMethod1(string imageLocalPath)
        {

            Dictionary<String, int> imageParams = new Dictionary<string, int>();
            imageParams.Add("imageWidth", 100);
            imageParams.Add("imageHeight", 200);
            imageParams.Add("redThreshold", 10);
            imageParams.Add("greenThreshold", 0);
            imageParams.Add("blueThreshold", 30);

            var api = new LearningApi();

            api.UseActionModule<double[,,], double[,,]>((input, ctx) =>
            {
                string path = imageLocalPath;

                Bitmap bitmap = new Bitmap(path);

                int imgWidth = bitmap.Width;
                int imgHeight = bitmap.Height;
                double[,,] data = new double[imgWidth, imgHeight, 3];

                for (int i = 0; i < imgWidth; i++)
                {
                    for (int j = 0; j < imgHeight; j++)
                    {
                        Color color = bitmap.GetPixel(i, j);
                        data[i, j, 0] = color.R;
                        data[i, j, 1] = color.G;
                        data[i, j, 2] = color.B;
                    }
                }
                return data;
            });

            api.UseImageBinarizer(imageParams);
            var result = api.Run() as double[,,];

            StringBuilder stringArray = new StringBuilder();
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    stringArray.Append(result[i, j, 0]);
                }
                stringArray.AppendLine();
            }

            string resultFilePath = imageLocalPath.Replace(".png", ".txt");

            using (StreamWriter writer = File.CreateText(resultFilePath))
            {
                writer.Write(stringArray.ToString());
            }

            return resultFilePath;
        }

    }
}

