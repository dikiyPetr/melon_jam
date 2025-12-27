using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    public static GameLifetimeScope instance;

    protected override void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        base.Awake();
    }

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<GlobalConfigs>().AsSelf();
        builder.RegisterComponentInHierarchy<InputManager>().AsSelf();
        builder.RegisterComponentInHierarchy<GameModeManager>().AsSelf();
        builder.RegisterComponentInHierarchy<MapNavigationController>().AsSelf();
        builder.RegisterComponentInHierarchy<AIMotivationController>().AsSelf();
        builder.RegisterComponentInHierarchy<AIMotivationPathController>().AsSelf();
        builder.RegisterComponentInHierarchy<PlayerTurnController>().AsSelf();

        builder.Register<AIMotivationData>(Lifetime.Singleton).AsSelf();
    }
}