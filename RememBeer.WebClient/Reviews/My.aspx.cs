﻿using System;
using System.Collections.Generic;
using System.Linq;

using RememBeer.Business.Reviews.My;
using RememBeer.Business.Reviews.My.Contracts;
using RememBeer.Models;
using RememBeer.WebClient.BasePages;

using WebFormsMvp;

namespace RememBeer.WebClient.Reviews
{
    [PresenterBinding(typeof(MyReviewsPresenter))]
    public partial class My : BaseMvpPage<ReviewsViewModel>, IMyReviewsView
    {
        public event EventHandler<EventArgs> OnInitialise;

        public event EventHandler<IBeerReviewInfoEventArgs> ReviewUpdate;

        public string SuccessMessageText
        {
            get { return this.Notifier.SuccessMessageText; }
            set { this.Notifier.SuccessMessageText = value; }
        }

        public bool SuccessMessageVisible
        {
            get { return this.Notifier.SuccessMessageVisible; }
            set { this.Notifier.SuccessMessageVisible = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.OnInitialise?.Invoke(this, EventArgs.Empty);
        }

        public IEnumerable<BeerReview> Select(int startRowIndex, int maximumRows, out int totalRowCount)
        {
            totalRowCount = this.Model.Reviews.Count;

            return this.Model.Reviews.Skip(startRowIndex).Take(maximumRows);
        }

        public void UpdateReview(BeerReview newReview)
        {
            var args = this.EventArgsFactory.CreateBeerReviewInfoEventArgs(newReview);
            this.ReviewUpdate?.Invoke(this, args);
        }
    }
}