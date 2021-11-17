using System.Reflection;

namespace EHunter.Controls
{
    public class ReflectionPageLocator : IPageLocator
    {
        private readonly Dictionary<Type, Type> _map;

        public ReflectionPageLocator(Assembly assembly)
        {
            _map = assembly
                .GetTypes()
                .Where(t => !t.IsAbstract && !t.IsGenericType)
                .Select(t => (Type: t, Interface: t
                    .GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IViewFor<>))))
                .Where(x => x.Interface is not null)
                .ToDictionary(x => x.Interface!.GenericTypeArguments[0], x => x.Type);
        }

        public Type? MapPageType(object viewModel)
            => _map.TryGetValue(viewModel.GetType(), out var type)
            ? type
            : null;
    }
}
