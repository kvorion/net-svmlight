﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetSVMLight;
using System.IO;

namespace NetSVMLightConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            //ENTER FULL PATH OF EXECUTABLES.

            SVMLearn learner = new SVMLearn();
            learner.mode = Mode.Classification;
            learner.kernelType = Kernel.Linear ;
            //learner.ParamC = 2;
            //learner.ParamD = 2;
            //learner.LeaveOneOutCrossValidation = true;
            //learner.ParamG = 5;
            //learner.TrainingErrorAndMarginTradeoff = 10;
            learner.Cost = 0.55; //high cost model
            learner.RemoveInconsistentTrainingExamples = true;
            //String keyword = "kv";
            //String logFileName = keyword + learner.kernelType.ToString() + learner.ParamC + learner.ParamD + ".log.txt";
            //learner.ExecuteLearner("svm_learn.exe",
            //    @"E:\kv\My Dropbox\code\SocialGraphLinking\" + keyword + @"\entitylinking-" +
            //    keyword + ".data",
            //        @"E:\kv\My Dropbox\code\SocialGraphLinking\" + keyword + "\\" + keyword + ".model",
            //        @"E:\kv\My Dropbox\code\SocialGraphLinking\" + keyword + "\\" + logFileName, false);

            //Utilities utility = new Utilities();
            //utility.ConstructTrainingAndTestSets
            //    (@"E:\kv\My Dropbox\code\SocialGraphLinking\kv-akshaya-varish\entitylinking-kv-akshaya-varish.data",
            //    @"E:\kv\My Dropbox\code\SocialGraphLinking\kv-akshaya-varish\kv-akshaya-varish-training.arff",
            //    @"E:\kv\My Dropbox\code\SocialGraphLinking\kv-akshaya-varish\kv-akshaya-varish-test.data.arff", 0.7, true, 
            //    line => line.StartsWith("@"), line => line.Split(',')[5].Trim().Equals("0"));
            
            
            //learner.ExecuteLearner("svm_learn.exe", @"E:\kv\My Dropbox\code\SocialGraphLinking\kv-akshaya-varish\kv-akshaya-varish-reduced-training.data",
            //    @"E:\kv\My Dropbox\code\SocialGraphLinking\kv-akshaya-varish\kv-akshaya-varish-reduced.model",
            //    @"E:\kv\My Dropbox\code\SocialGraphLinking\kv-akshaya-varish\kv-akshaya-varish-reduced-training.log.txt", false);


            

            //SVMClassify classifier = new SVMClassify();
            //classifier.ExecuteClassifier("svm_classify.exe", @"E:\kv\My Dropbox\code\SocialGraphLinking\kv-akshaya-varish\entitylinking-kv-akshaya-varish.data",
            //    @"E:\kv\My Dropbox\code\SocialGraphLinking\kv-akshaya-varish\combined-linear-highcost.model",
            //    @"E:\kv\My Dropbox\code\SocialGraphLinking\kv-akshaya-varish\whatever.output",
            //    @"E:\kv\My Dropbox\code\SocialGraphLinking\kv-akshaya-varish\whatever-log.txt",
            //    @"E:\kv\My Dropbox\code\SocialGraphLinking\kv-akshaya-varish\whatever-incorrect.txt",
            //    false);
            //classifier.FindIncorrectlyClassifiedInstances(@"E:\kv\My Dropbox\code\SocialGraphLinking\kv-akshaya-varish\kv-akshaya-varish-reduced-test.data",
            //    @"E:\kv\My Dropbox\code\SocialGraphLinking\kv-akshaya-varish\kv-akshaya-varish-reduced.output",
            //    @"E:\kv\My Dropbox\code\SocialGraphLinking\kv-akshaya-varish\kv-akshaya-varish-reduced-incorrect.txt");

            //IncorrectFileParser.ParseIncorrectFile(@"D:\kv\official\My Dropbox\code\SocialGraphLinking\kv-akshaya-varish\combined-linear-incorrect.txt");

            //Utilities.ConstructTrainingAndTestSets(@"D:\kv\official\My Dropbox\code\SocialGraphLinking\kv-akshaya-varish\entitylinking-kv-akshaya-varish.data",
            //    @"D:\kv\official\My Dropbox\code\SocialGraphLinking\kv-akshaya-varish\training1.data",
            //    @"D:\kv\official\My Dropbox\code\SocialGraphLinking\kv-akshaya-varish\test1.data", 0.7, false);


            //int pos, neg;
            //Utilities.GetNumberOfTrainingExamples(@"D:\kv\official\My Dropbox\code\SocialGraphLinking\kv-akshaya-varish\training.data",
            //    out pos, out neg);

            //Console.WriteLine("positive: {0}, negative:{1}", pos, neg);

            Utilities u = new Utilities();

            u.EntryPoint(@"D:\kv\official\My Dropbox\code\SocialGraphLinking\kv-akshaya-varish\combined.data",
                10, @"D:\kv\official\My Dropbox\code\SocialGraphLinking\kv-akshaya-varish\10foldcv",
                line => line.StartsWith("-"));

            //Random random = new Random();
            //for (int counter = 0; counter < 20; counter++)
            //{
            //    Console.WriteLine(random.Next(5));
            //    Console.ReadKey();
            //}


            
            //Console.ReadLine();
        }
    }
}
