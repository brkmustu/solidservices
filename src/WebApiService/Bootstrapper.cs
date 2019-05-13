﻿namespace WebApiService
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Security.Principal;
    using System.Threading;
    using System.Web;
    using BusinessLayer;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;

    public static class Bootstrapper
    {
        public static IEnumerable<Type> GetKnownCommandTypes() => BusinessLayerBootstrapper.GetCommandTypes();

        public static IEnumerable<QueryInfo> GetKnownQueryTypes() => BusinessLayerBootstrapper.GetQueryTypes();

        public static Container Bootstrap()
        {
            var container = new Container();

            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            BusinessLayerBootstrapper.Bootstrap(container);

            container.RegisterSingleton<IPrincipal>(new HttpContextPrincipal());
            container.RegisterSingleton<ILogger>(new DebugLogger());

            return container;
        }

        private sealed class HttpContextPrincipal : IPrincipal
        {
            public IIdentity Identity => this.Principal.Identity;
            private IPrincipal Principal => HttpContext.Current.User ?? Thread.CurrentPrincipal;
            public bool IsInRole(string role) => this.Principal.IsInRole(role);
        }

        private sealed class DebugLogger : ILogger
        {
            public void Log(string message)
            {
                Debug.WriteLine(message);
            }
        }
    }
}