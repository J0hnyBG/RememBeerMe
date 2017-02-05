﻿using System;

using Microsoft.AspNet.Identity.Owin;

using RememBeer.Business.Account.Auth;
using RememBeer.Business.Account.Common.Presenters;
using RememBeer.Business.Account.Login.Contracts;
using RememBeer.Common.Identity.Contracts;

namespace RememBeer.Business.Account.Login
{
    public class LoginPresenter : AuthenticationPresenter<ILoginView>
    {
        private readonly IIdentityHelper identityHelper;

        public LoginPresenter(IAuthProvider provider, IIdentityHelper identityHelper, ILoginView view)
            : base(provider, view)
        {
            if (identityHelper == null)
            {
                throw new ArgumentNullException(nameof(identityHelper));
            }

            this.identityHelper = identityHelper;
            this.View.OnLogin += this.OnLogin;
        }

        private void OnLogin(object sender, ILoginEventArgs args)
        {
            var ctx = this.AuthProvider.CreateOwinContext(this.HttpContext);
            var pass = args.Password;
            var email = args.Email;
            var isPersistentLogin = args.RememberMe;

            var signinManager = this.AuthProvider.CreateApplicationSignInManager(ctx);

            var result = signinManager.PasswordSignIn(email, pass, isPersistentLogin);
            switch (result)
            {
                case SignInStatus.Success:
                    var url = this.Request.QueryString["ReturnUrl"];
                    this.identityHelper.RedirectToReturnUrl(url, this.Response);
                    break;
                case SignInStatus.LockedOut:
                    this.Response.Redirect("/Account/Lockout");
                    break;
                case SignInStatus.RequiresVerification:
                    var returnUrl = string.Format(
                                                  "/Account/TwoFactorAuthenticationSignIn?ReturnUrl={0}&RememberMe={1}",
                                                  this.Request.QueryString["ReturnUrl"],
                                                  isPersistentLogin);
                    this.Response.Redirect(returnUrl, true);
                    break;
                case SignInStatus.Failure:
                default:
                    this.View.FailureMessage = "Username or password is incorrect!";
                    this.View.ErrorMessageVisible = true;
                    break;
            }
        }
    }
}
