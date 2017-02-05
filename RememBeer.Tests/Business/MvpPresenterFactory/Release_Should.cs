﻿using Moq;

using NUnit.Framework;

using RememBeer.Business.MvpPresenterFactory;
using RememBeer.Tests.Business.Mocks;

namespace RememBeer.Tests.Business.MvpPresenterFactory
{
    [TestFixture]
    public class Release_Should
    {
        [Test]
        public void CallDisposeOnPassedPresenter()
        {
            var mockedPresenter = new Mock<IDisposablePresenter>();
            var mockedFactory = new Mock<IMvpPresenterFactory>();

            var sut = new RememBeer.Business.MvpPresenterFactory.MvpPresenterFactory(mockedFactory.Object);

            sut.Release(mockedPresenter.Object);

            mockedPresenter.Verify(p => p.Dispose(), Times.Once());
        }
    }
}
