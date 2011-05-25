﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace NetSVMLight
{
    public class SVMClassify
    {
        private StringBuilder svmClassifyOutput = new StringBuilder();
        
        private double precision;
        public Double Precision
        {
            get { return this.precision; }
            set { this.precision = value; }
        }
        
        private double recall;
        public Double Recall
        {
            get { return this.recall; }
            set { this.recall = value; }
        }

        private double accuracy;
        public Double Accuracy
        {
            get { return this.accuracy; }
            set { this.accuracy = value; }
        }

        /// <summary>
        /// Executes svm_classifier
        /// </summary>
        /// <param name="svmClassifyPath">Enter full path, including .exe extension</param>
        /// <param name="testFile">The test dataset</param>
        /// <param name="modelFile">The model generated by svm_learn</param>
        /// <param name="outputFile">The output to be generated by svm_classify</param>
        /// <param name="logFile">Name of log file to be generated</param>
        /// <param name="silent">Whether to be silent: i.e. no output on the console. Log file is still
        /// generated</param>
        public void ExecuteClassifier(String svmClassifyPath, String testFile, String modelFile, String outputFile, 
            String logFile, String incorrectFile, bool silent)
        {
            Trace.Listeners.Clear();

            if (!silent)
            {
                Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            }

            if (File.Exists(logFile))
            {
                Console.WriteLine("\n\nLog file " + logFile + " already exists. Deleting");
                try
                {
                    File.Delete(logFile);
                }
                catch (Exception)
                {
                    //ignore..don't do anything if u cant delete the log
                }
            }

            Trace.Listeners.Add(new TextWriterTraceListener(logFile));
            Trace.AutoFlush = true;

            if (!Utilities.ExistsInPath(svmClassifyPath) || !Utilities.ExistsInPath(testFile))
            {
                Trace.WriteLine("Invalid file path");
                Environment.Exit(0);
            }

            Trace.WriteLine("Classifying test instances....");

            Process svmClassifyProcess = new Process();
            svmClassifyProcess.StartInfo.FileName = svmClassifyPath;

            svmClassifyProcess.StartInfo.Arguments = "\"" + testFile + "\"" + " \"" + modelFile + "\" " + 
                "\"" + outputFile + "\"";

            svmClassifyProcess.StartInfo.UseShellExecute = false;
            svmClassifyProcess.StartInfo.RedirectStandardError = true;
            svmClassifyProcess.StartInfo.RedirectStandardOutput = true;
            svmClassifyProcess.OutputDataReceived += new DataReceivedEventHandler(svmClassifyProcess_OutputDataReceived);
            svmClassifyProcess.ErrorDataReceived += new DataReceivedEventHandler(svmClassifyProcess_ErrorDataReceived);

            svmClassifyProcess.Start();
            svmClassifyProcess.BeginErrorReadLine();
            svmClassifyProcess.BeginOutputReadLine();


            svmClassifyProcess.WaitForExit();
            svmClassifyProcess.Close();

            Trace.WriteLine(this.svmClassifyOutput.ToString());

            String logFileName = Path.GetFileNameWithoutExtension(logFile);
            String incorrectLogFile = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(logFile)), 
                logFileName + "-incorrect-log.txt");

            this.FindIncorrectlyClassifiedInstances(testFile, outputFile, incorrectFile, incorrectLogFile);
        }

        /// <summary>
        /// Finds all those instances which were incorrectly classified by svmlight
        /// </summary>
        /// <param name="testFile">The test datset</param>
        /// <param name="outputFile">The output file generated by svm_classify</param>
        /// <param name="incorrect">The file to be generated by this function: contains all the 
        /// features vectors that were incorrectly classified by svm_classify</param>
        public void FindIncorrectlyClassifiedInstances(String testFile, String outputFile, String incorrect,
            String logFile)
        {
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Trace.Listeners.Add(new TextWriterTraceListener(logFile));
            int incorrectClassificationCount = 0;

            int falsePositive = 0, falseNegative = 0;

            if (!Utilities.ExistsInPath(testFile) || !Utilities.ExistsInPath(outputFile))
            {
                Trace.WriteLine("Invalid file path");
                Environment.Exit(0);
            }

            using (StreamReader testFileReader = new StreamReader(testFile))
            {
                using (StreamReader outputFileReader = new StreamReader(outputFile))
                {
                    try
                    {
                        Trace.WriteLine("Writing incorrectly classified instances to disk....");
                        StreamWriter incorrectClassificationWriter = new StreamWriter(incorrect, false);
                        String testFileLine = testFileReader.ReadLine();
                        String outputFileLine = outputFileReader.ReadLine();

                        while (!testFileReader.EndOfStream && !outputFileReader.EndOfStream)
                        {
                            testFileLine = testFileReader.ReadLine();
                            outputFileLine = outputFileReader.ReadLine();

                            int testLabel = testFileLine[0].Equals('-') ? -1 : 1;
                            int outputLabel = Double.Parse(outputFileLine) > 0 ? 1 : -1;

                            if (testLabel != outputLabel)
                            {
                                incorrectClassificationCount++;

                                if (testLabel == 1 && outputLabel == -1)
                                {
                                    falseNegative++;
                                }

                                else if (testLabel == -1 && outputLabel == 1)
                                {
                                    falsePositive++;
                                }

                                incorrectClassificationWriter.WriteLine(testFileLine + "#"
                                    + outputFileLine);
                            }
                        }

                        if (falsePositive + falseNegative != incorrectClassificationCount)
                        {
                            throw new Exception("problem with counting incorrect classifications");
                        }

                        Trace.WriteLine(incorrectClassificationCount + " wrong instances instances...done");
                        Trace.WriteLine("False positives: " + falsePositive);
                        Trace.WriteLine("False negative: " + falseNegative);

                        incorrectClassificationWriter.WriteLine("#False positives: " + falsePositive);
                        incorrectClassificationWriter.WriteLine("#False negative: " + falseNegative);

                        incorrectClassificationWriter.Flush();
                        incorrectClassificationWriter.Close();

                        if (!testFileReader.EndOfStream || !outputFileReader.EndOfStream)
                        {
                            throw new Exception("Both files have different number of instances");
                        }
                    }
                    catch (Exception ex)
                    {                        
                        throw;
                    }
                }
            }

            
        }

        /// <summary>
        /// Event-handler for when error data is received from an invoked process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="errLine"></param>
        void svmClassifyProcess_ErrorDataReceived(object sender, DataReceivedEventArgs errLine)
        {
            if (!String.IsNullOrEmpty(errLine.Data))
            {
                this.svmClassifyOutput.Append(errLine.Data + Environment.NewLine);
            }
        }

        /// <summary>
        /// Event-handler for when data is received from the process that has been initiated
        /// </summary>
        /// <param name="sendingProcess"></param>
        /// <param name="outLine"></param>
        private void svmClassifyProcess_OutputDataReceived(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                this.svmClassifyOutput.Append(outLine.Data + Environment.NewLine);
                
                //temporary hack. do this better using delegates
                if (outLine.Data.ToLower().Contains("on test set"))
                {
                    if (outLine.Data.ToLower().Contains("precision"))
                    {
                        String[] precisionRecallPercentage = outLine.Data.Split(':')[1].Split('/');

                        if (!Double.TryParse(precisionRecallPercentage[0].Replace("%", String.Empty), out this.precision))
                        {
                            this.precision = Double.NaN;
                        }

                        if (!Double.TryParse(precisionRecallPercentage[1].Replace("%", String.Empty), out this.recall))
                        {
                            this.recall = Double.NaN;
                        }
                    }

                    else //accuracy
                    {
                        String accuracyPercentage = outLine.Data.Substring(outLine.Data.IndexOf(':') + 1, 
                            outLine.Data.IndexOf('%') - outLine.Data.IndexOf(':') - 1);

                        if (!Double.TryParse(accuracyPercentage, out this.accuracy))
                        {
                            this.accuracy = Double.NaN;
                        }
                    }
                }
            }
        }

    }
}
