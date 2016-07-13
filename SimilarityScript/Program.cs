using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDotNet;
using System.IO;
using DotSpatial.Data;
using DotSpatial.Topology;
using System.Diagnostics;


namespace SimilarityScript
{
    class Program
    {
        public static REngine engine = REngine.GetInstance();
        static void Main(string[] args)
        {
                    
            int index;

            //path
            string path = args[0];

            //filename
            string shapeFileName = args[1];
            string shapeFullPath = path + shapeFileName;

            //coords
            int coordsAmount = Convert.ToInt32(args[2]);
            List<Polygon> pg = new List<Polygon>();
            Feature f = new Feature();
            FeatureSet fs = new FeatureSet(f.FeatureType);

            List<DotSpatial.Topology.Coordinate> coordShape = new List<DotSpatial.Topology.Coordinate>();

            LinearRing polygonShape = new LinearRing(coordShape);

            for (index = 3; index < coordsAmount + 3; index += 2)
            {
                coordShape.Add(new Coordinate(Convert.ToDouble(args[index]), Convert.ToDouble(args[index + 1])));
            }

            Polygon polygon = new Polygon(polygonShape);
            pg.Add(polygon);
            fs.Features.Add(pg[0]);
            fs.SaveAs(shapeFullPath, true);

            //boundingbox
            double xMin = Convert.ToDouble(args[index++]);
            double xMax = Convert.ToDouble(args[index++]);
            double yMin = Convert.ToDouble(args[index++]);
            double yMax = Convert.ToDouble(args[index++]);

            //2 outputnames
            string studyAreaName = args[index++];
            string similarityName = args[index++];
            string output = args[index++];

            FileStream stream = null;
           
            try
            {
                stream = File.OpenRead(path + "/1_Inputs/AfricaAction1_CF.R");
            }
            catch (Exception)
            {
            }

                                                                                                        //creating vectors, because R just support vectors
            var generalPath = engine.CreateCharacterVector(new string[] { path });
            var xMinVector = engine.CreateNumericVector(new double[] { xMin });
            var xMaxVector = engine.CreateNumericVector(new double[] { xMax });
            var yMinVector = engine.CreateNumericVector(new double[] { yMin });
            var yMaxVector = engine.CreateNumericVector(new double[] { yMax });
            var shapePath = engine.CreateCharacterVector(new string[] { shapeFileName });
            var studyAreaFileName = engine.CreateCharacterVector(new string[] { studyAreaName });
            var similarityFileName = engine.CreateCharacterVector(new string[] { similarityName });
            var outputPath = engine.CreateCharacterVector(new string[] { output });

            try
            {
                engine.Evaluate(stream);
                stream.Dispose();
                stream.Close();
            }
            catch (Exception)
            {                           
            }
                                                                                                                    //setting the variables for R, so its possible to call a function with them
            engine.SetSymbol("path", generalPath);
            engine.SetSymbol("xMin", xMinVector);
            engine.SetSymbol("xMax", xMaxVector);
            engine.SetSymbol("yMin", yMinVector);
            engine.SetSymbol("yMax", yMaxVector);
            engine.SetSymbol("shapePath", shapePath);
            engine.SetSymbol("studyAreaFileName", studyAreaFileName);
            engine.SetSymbol("similarityFileName", similarityFileName);
            engine.SetSymbol("outputPath", outputPath);

            try
            {                                                                                                       //calling the R script        
                engine.Evaluate("runScript(path,xMin, xMax, yMin, yMax, shapePath, studyAreaFileName, similarityFileName, outputPath)");
                engine.Dispose();
            }
            catch (Exception)
            {
            }

            Process p = Process.GetCurrentProcess();

            Environment.ExitCode = 0;
            p.CloseMainWindow();
            p.Close();
        }
    }
}
