﻿// This file was auto-generated by ML.NET Model Builder.
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
namespace BeatSaberTools_Core
{
    public partial class ScoreEstimateMLModel
    {
        /// <summary>
        /// model input class for ScoreEstimateMLModel.
        /// </summary>
        #region model input class
        public class ModelInput
        {
            [LoadColumn(0)]
            [ColumnName(@"PP")]
            public float PP { get; set; }

            [LoadColumn(1)]
            [ColumnName(@"StarDifficulty")]
            public float StarDifficulty { get; set; }

            [LoadColumn(2)]
            [ColumnName(@"Accuracy")]
            public float Accuracy { get; set; }

            [LoadColumn(3)]
            [ColumnName(@"TimeSet")]
            public DateTime TimeSet { get; set; }

        }

        #endregion

        /// <summary>
        /// model output class for ScoreEstimateMLModel.
        /// </summary>
        #region model output class
        public class ModelOutput
        {
            [ColumnName(@"PP")]
            public float PP { get; set; }

            [ColumnName(@"StarDifficulty")]
            public float StarDifficulty { get; set; }

            [ColumnName(@"Accuracy")]
            public float Accuracy { get; set; }

            [ColumnName(@"TimeSet")]
            public float TimeSet { get; set; }

            [ColumnName(@"Features")]
            public float[] Features { get; set; }

            [ColumnName(@"Score")]
            public float Score { get; set; }

        }

        #endregion

        private static string MLNetModelPath = Path.GetFullPath("C:\\dev\\BeatSaberTools\\BeatSaberTools.Core\\ScoreEstimateMLModel.zip");

        public static readonly Lazy<PredictionEngine<ModelInput, ModelOutput>> PredictEngine = new Lazy<PredictionEngine<ModelInput, ModelOutput>>(() => CreatePredictEngine(), true);

        /// <summary>
        /// Use this method to predict on <see cref="ModelInput"/>.
        /// </summary>
        /// <param name="input">model input.</param>
        /// <returns><seealso cref=" ModelOutput"/></returns>
        public static ModelOutput Predict(ModelInput input)
        {
            var predEngine = PredictEngine.Value;
            return predEngine.Predict(input);
        }

        private static PredictionEngine<ModelInput, ModelOutput> CreatePredictEngine()
        {
            var mlContext = new MLContext();
            ITransformer mlModel = mlContext.Model.Load(MLNetModelPath, out var _);
            return mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
        }
    }
}
