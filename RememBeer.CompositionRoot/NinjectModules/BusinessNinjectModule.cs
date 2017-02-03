﻿using System;
using System.Collections;
using System.Linq;

using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

using Ninject;
using Ninject.Activation;
using Ninject.Extensions.Factory;
using Ninject.Modules;
using Ninject.Parameters;

using RememBeer.Business.Account.Auth;
using RememBeer.Business.MvpPresenter;
using RememBeer.Data;

using WebFormsMvp;
using WebFormsMvp.Binder;

namespace RememBeer.CompositionRoot.NinjectModules
{
    public class BusinessNinjectModule : NinjectModule
    {
        /// <summary>Loads the module into the kernel.</summary>
        public override void Load()
        {
            this.Bind<IPresenterFactory>().To<MvpPresenterFactory>().InSingletonScope();

            this.Bind<IMvpPresenterFactory>().ToFactory().InSingletonScope();

            //this.Bind<ICustomEventArgsFactory>().ToFactory().InSingletonScope();

            this.Bind<IPresenter>()
                .ToMethod(GetPresenter)
                .NamedLikeFactoryMethod(
                                        (IMvpPresenterFactory factory) => factory.GetPresenter(null, null)
                );

            this.Bind<IIdentityFactory>().ToFactory();

            this.Rebind<ApplicationUserManager>()
                .ToMethod(ctx =>
                          {
                              var parameters = ctx.Parameters.ToList();
                              var options = (IdentityFactoryOptions<ApplicationUserManager>)parameters[0]
                                  .GetValue(ctx, null);

                              var owinContext = (IOwinContext)parameters[1].GetValue(ctx, null);

                              return ApplicationUserManager.Create(options, owinContext);
                          })
                .NamedLikeFactoryMethod((IIdentityFactory f) => f.GetApplicationUserManager(null, null));

            this.Rebind<IIdentityHelper>().To<IdentityHelper>().InSingletonScope();
        }

        private static IPresenter GetPresenter(IContext context)
        {
            var parameters = context.Parameters.ToList();

            var presenterType = (Type)parameters[0].GetValue(context, null);
            var viewInstance = (IView)parameters[1].GetValue(context, null);

            var ctorParamter = new ConstructorArgument("view", viewInstance);

            return (IPresenter)context.Kernel.Get(presenterType, ctorParamter);
        }
    }
}
