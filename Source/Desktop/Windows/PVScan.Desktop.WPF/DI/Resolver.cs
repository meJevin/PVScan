using Autofac;
using System;

namespace PVScan.Desktop.WPF.DI
{
    public static class Resolver
    {
        public static IContainer Container;

        public static void Initialize(IContainer container)
        {
            Container = container;
        }

        public static T Resolve<T>()
        {
            return Container.Resolve<T>();
        }

        public static object Resolve(Type type)
        {
            return Container.Resolve(type);
        }
    }
}
