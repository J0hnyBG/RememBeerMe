﻿using System.Collections.Generic;

using RememBeer.Business.Logic.Admin.Breweries;
using RememBeer.Models.Contracts;

namespace RememBeer.Tests.Business.Mocks
{
    public class MockedBreweriesViewModel : BreweriesViewModel
    {
        public override IEnumerable<IBrewery> Breweries { get; set; }
    }
}