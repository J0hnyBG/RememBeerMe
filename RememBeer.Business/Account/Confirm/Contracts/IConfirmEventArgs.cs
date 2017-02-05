﻿using RememBeer.Business.Account.Common.EventArcs.Contracts;

namespace RememBeer.Business.Account.Confirm.Contracts
{
    public interface IConfirmEventArgs
    {
        string UserId { get; set; }

        string Code { get; set; }
    }
}