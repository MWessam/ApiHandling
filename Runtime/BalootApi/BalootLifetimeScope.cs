using ApiHandling.Generated.Facade;
using ApiHandling.Runtime;
using ApiHandling.Runtime.Utilities;
using BalootApi;
using UnityEngine;
using VContainer;
using VContainer.Unity;

[DefaultExecutionOrder(-100)]
public class BalootLifetimeScope : LifetimeScope
{
    [SerializeField] protected ApiConfigSO _apiConfig;
    [SerializeField] protected WebSocketConnection _webSocketConnection;

    public new static IObjectResolver Resolver;
    protected override void Awake()
    {
        base.Awake();
        Resolver = Container;
    }

    protected override void Configure(IContainerBuilder builder)
    {
        base.Configure(builder);
        if (_apiConfig)
        {
            builder.RegisterInstance(_apiConfig);
        }
        if (_webSocketConnection)
        {
            builder.RegisterInstance(_webSocketConnection);
        }
        builder.Register<IApiHandler, BalootApiHandler>(Lifetime.Singleton);
        builder.Register<IApiRequest, ApiRequest>(Lifetime.Singleton);
        builder.Register<IAuthService, AuthService>(Lifetime.Singleton);
        builder.Register<ApiFacade>(Lifetime.Singleton);
        MapperLayer.InitializeMappers();
    }
}
