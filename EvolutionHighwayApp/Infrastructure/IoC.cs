using Castle.Windsor;

namespace EvolutionHighwayApp.Infrastructure
{
    public static class IoC
    {
        static IoC()
        {
            Container = new WindsorContainer();
        }

        public static IWindsorContainer Container { get; private set; }
    }
}
