﻿using System;
using System.Collections.Generic;
using System.Linq;

using Moq;

using NUnit.Framework;

using Ploeh.AutoFixture;

using RememBeer.Data.Services.RankingStrategies;
using RememBeer.Models.Contracts;
using RememBeer.Models.Dtos;
using RememBeer.Models.Factories;
using RememBeer.Tests.Common;

namespace RememBeer.Tests.Data.Services.RankingStrategies.DoubleOverallScoreStrategyTests
{
    [TestFixture]
    public class GetRank_Should : TestClassBase
    {
        [Test]
        public void ThrowArgumentNullException_WhenReviewsArgumentIsNull()
        {
            var factory = new Mock<IModelFactory>();
            var beer = new Mock<IBeer>();
            var strategy = new DoubleOverallScoreStrategy(factory.Object);

            Assert.Throws<ArgumentNullException>(() => strategy.GetRank(null, beer.Object));
        }

        [Test]
        public void ThrowArgumentNullException_WhenBeerArgumentIsNull()
        {
            var factory = new Mock<IModelFactory>();
            var reviews = new Mock<IEnumerable<IBeerReview>>();

            var strategy = new DoubleOverallScoreStrategy(factory.Object);

            Assert.Throws<ArgumentNullException>(() => strategy.GetRank(reviews.Object, null));
        }

        [Test]
        public void ThrowArgumentException_WhenReviewsAreEmpty()
        {
            var factory = new Mock<IModelFactory>();

            var beer = new Mock<IBeer>();
            var reviews = new List<IBeerReview>();
            var strategy = new DoubleOverallScoreStrategy(factory.Object);

            Assert.Throws<ArgumentException>(() => strategy.GetRank(reviews, beer.Object));
        }

        [Test]
        public void CallFactoryCreateBeerRankMethod_WithCorrectParamsOnce()
        {
            var overallScore = this.Fixture.Create<int>();
            var tasteScore = this.Fixture.Create<int>();
            var smellScore = this.Fixture.Create<int>();
            var looksScore = this.Fixture.Create<int>();
            var factory = new Mock<IModelFactory>();

            var mockedReview = new Mock<IBeerReview>();
            mockedReview.Setup(r => r.Overall).Returns(overallScore);
            mockedReview.Setup(r => r.Taste).Returns(tasteScore);
            mockedReview.Setup(r => r.Smell).Returns(smellScore);
            mockedReview.Setup(r => r.Look).Returns(looksScore);

            var mockedReview2 = new Mock<IBeerReview>();
            mockedReview.Setup(r => r.Overall).Returns(overallScore + this.Fixture.Create<int>());
            mockedReview.Setup(r => r.Taste).Returns(tasteScore + this.Fixture.Create<int>());
            mockedReview.Setup(r => r.Smell).Returns(smellScore + this.Fixture.Create<int>());
            mockedReview.Setup(r => r.Look).Returns(looksScore + this.Fixture.Create<int>());

            var beer = new Mock<IBeer>();
            var reviews = new List<IBeerReview>()
                          {
                              mockedReview.Object,
                              mockedReview2.Object
                          };
            var expectedAggregateScore = reviews.Sum(beerReview =>
                                                         (decimal)(2 * beerReview.Overall
                                                                   + beerReview.Look
                                                                   + beerReview.Smell
                                                                   + beerReview.Taste)
                                                         / 5) / reviews.Count;
            var expectedOverall = (decimal)reviews.Sum(r => r.Overall) / reviews.Count;
            var expectedTaste = (decimal)reviews.Sum(r => r.Taste) / reviews.Count;
            var expectedSmell = (decimal)reviews.Sum(r => r.Smell) / reviews.Count;
            var expectedLook = (decimal)reviews.Sum(r => r.Look) / reviews.Count;

            var strategy = new DoubleOverallScoreStrategy(factory.Object);

            var result = strategy.GetRank(reviews, beer.Object);

            factory.Verify(
                           f =>
                               f.CreateBeerRank(expectedOverall,
                                                expectedTaste,
                                                expectedLook,
                                                expectedSmell,
                                                beer.Object,
                                                expectedAggregateScore,
                                                reviews.Count),
                           Times.Once);
        }

        [Test]
        public void ReturnResultFromFactory()
        {
            var expectedRank = new Mock<IBeerRank>();
            var overallScore = this.Fixture.Create<int>();
            var tasteScore = this.Fixture.Create<int>();
            var smellScore = this.Fixture.Create<int>();
            var looksScore = this.Fixture.Create<int>();
            var expectedAggregateScore = (decimal)((overallScore * 2) + tasteScore + smellScore + looksScore) / 5;

            var mockedReview = new Mock<IBeerReview>();
            mockedReview.Setup(r => r.Overall).Returns(overallScore);
            mockedReview.Setup(r => r.Taste).Returns(tasteScore);
            mockedReview.Setup(r => r.Smell).Returns(smellScore);
            mockedReview.Setup(r => r.Look).Returns(looksScore);

            var beer = new Mock<IBeer>();
            var reviews = new List<IBeerReview>()
                          {
                              mockedReview.Object
                          };
            var factory = new Mock<IModelFactory>();
            factory.Setup(
                          f =>
                              f.CreateBeerRank(overallScore,
                                               tasteScore,
                                               looksScore,
                                               smellScore,
                                               beer.Object,
                                               expectedAggregateScore,
                                               1))
                   .Returns(expectedRank.Object);

            var strategy = new DoubleOverallScoreStrategy(factory.Object);

            var result = strategy.GetRank(reviews, beer.Object);

            Assert.IsNotNull(result);
            Assert.AreSame(expectedRank.Object, result);
        }
    }
}
