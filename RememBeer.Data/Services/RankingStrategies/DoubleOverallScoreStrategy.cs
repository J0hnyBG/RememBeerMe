﻿using System;
using System.Collections.Generic;
using System.Linq;

using RememBeer.Data.Services.RankingStrategies.Base;
using RememBeer.Models.Contracts;
using RememBeer.Models.Dtos;
using RememBeer.Models.Factories;

namespace RememBeer.Data.Services.RankingStrategies
{
    public class DoubleOverallScoreStrategy : BeerRankCalculationStrategy
    {
        private const int OverallScoreMultiplier = 2;

        public DoubleOverallScoreStrategy(IBeerRankFactory factory)
            :base(factory)
        {
        }

        public override IBeerRank GetRank(IEnumerable<IBeerReview> reviews, IBeer beer)
        {
            if (reviews == null)
            {
                throw new ArgumentNullException(nameof(reviews));
            }

            if (beer == null)
            {
                throw new ArgumentNullException(nameof(beer));
            }

            var beerReviews = reviews as IBeerReview[] ?? reviews.ToArray();
            var reviewsCount = beerReviews.Length;
            if (reviewsCount == 0)
            {
                throw new ArgumentException("Reviews cannot be empty!");
            }

            decimal aggregateScore = beerReviews.Sum(beerReview =>
                                                         (decimal)(OverallScoreMultiplier * beerReview.Overall
                                                                   + beerReview.Look
                                                                   + beerReview.Smell
                                                                   + beerReview.Taste)
                                                         / (OverallScoreMultiplier + 3));

            var overallAverage = GetAverageScore(beerReviews, r => r.Overall);
            var tasteAverage = GetAverageScore(beerReviews, r => r.Taste);
            var smellAverage = GetAverageScore(beerReviews, r => r.Smell);
            var lookverage = GetAverageScore(beerReviews, r => r.Look);

            var finalScore = aggregateScore / reviewsCount;
            var rank = this.Factory.CreateBeerRank(overallAverage,
                                                   tasteAverage,
                                                   lookverage,
                                                   smellAverage,
                                                   beer,
                                                   finalScore,
                                                   reviewsCount);
            return rank;
        }

        private static decimal GetAverageScore(ICollection<IBeerReview> beerReviews, Func<IBeerReview, decimal> action)
        {
            return beerReviews.Sum(action) / beerReviews.Count;
        }
    }
}
