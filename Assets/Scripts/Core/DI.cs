public class DI
{
    public static void Inject(object instance)
    {
        GameLifetimeScope.instance.Container.Inject(instance);
    }
}