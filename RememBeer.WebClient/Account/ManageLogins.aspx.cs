﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using RememBeer.Data.Identity;

namespace RememBeer.WebClient.Account
{
    public partial class ManageLogins : Page
    {
        protected string SuccessMessage
        {
            get;
            private set;
        }
        protected bool CanRemoveExternalLogins
        {
            get;
            private set;
        }

        private bool HasPassword(ApplicationUserManager manager)
        {
            return manager.HasPassword(this.User.Identity.GetUserId());
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var manager = this.Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            this.CanRemoveExternalLogins = manager.GetLogins(this.User.Identity.GetUserId()).Count() > 1;

            this.SuccessMessage = string.Empty;
            this.successMessage.Visible = !string.IsNullOrEmpty(this.SuccessMessage);
        }

        public IEnumerable<UserLoginInfo> GetLogins()
        {
            var manager = this.Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var accounts = manager.GetLogins(this.User.Identity.GetUserId());
            this.CanRemoveExternalLogins = accounts.Count() > 1 || this.HasPassword(manager);
            return accounts;
        }

        public void RemoveLogin(string loginProvider, string providerKey)
        {
            var manager = this.Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var signInManager = this.Context.GetOwinContext().Get<ApplicationSignInManager>();
            var result = manager.RemoveLogin(this.User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            string msg = string.Empty;
            if (result.Succeeded)
            {
                var user = manager.FindById(this.User.Identity.GetUserId());
                signInManager.SignIn(user, isPersistent: false, rememberBrowser: false);
                msg = "?m=RemoveLoginSuccess";
            }
            this.Response.Redirect("~/Account/ManageLogins" + msg);
        }
    }
}