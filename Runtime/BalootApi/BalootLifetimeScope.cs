using ApiHandling.Generated.Commands;
using ApiHandling.Generated.Facade;
using ApiHandling.Runtime;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace BalootApi
{
    public class BalootLifetimeScope : LifetimeScope
    {
        public static IObjectResolver Container;
        [SerializeField] private ApiConfigSO _apiConfigSO;
        [SerializeField] private ApiRequest _apiRequest;
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            builder.Register<IApiHandler, BalootApiHandler>(Lifetime.Singleton);
            builder.RegisterInstance(_apiRequest).As<ApiRequest>();
            builder.RegisterInstance(_apiConfigSO).As<ApiConfigSO>();
            builder.Register<ApiFacade>(Lifetime.Singleton);
            builder.Register<GetUserCommand>(Lifetime.Singleton);
            // builder.Register<ApiRequest>(Lifetime.Singleton);
            // builder.Register<ApiConfigSO>(Lifetime.Singleton);
        }

        protected override void Awake()
        {
            base.Awake();
            Container = base.Container;
        }
    }
}