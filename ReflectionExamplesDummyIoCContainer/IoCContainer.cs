namespace ReflectionExamplesDummyIoCContainer
{
    /// <summary>
    /// A dummy IoCContainer that stores the registrations as a dictionary 
    /// with Contract as Key and Implementation as Value.
    /// </summary>
    public class IoCContainer
    {
        private readonly Dictionary<Type, Type> _map = new Dictionary<Type, Type>();
        private MethodInfo? _resolveMethod;

        /// <summary>
        /// Registers a type mapping for the specified contract and implementation.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <typeparam name="TImplementation">The type that implements the contract.</typeparam>
        public void Register<TContract, TImplementation>()
        {
            // Register in the mapping dictionary
            _map.TryAdd(typeof(TContract), typeof(TImplementation));
        }

        /// <summary>
        /// Registers a type mapping for the specified contract and implementation.
        /// </summary>
        /// <param name="contract">The type of the contract.</param>
        /// <param name="implementation">The type that implements the contract.</param>
        public void Register(Type contract, Type implementation)
        {
            // Register in the mapping dictionary
            _map.TryAdd(contract, implementation);
        }

        /// <summary>
        /// Resolves an instance of the specified contract.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <returns>An instance of the implementation type.</returns>
        public TContract Resolve<TContract>()
        {
            // Check whether we're trying to resolve a generic type
            if (typeof(TContract).IsGenericType &&
                _map.ContainsKey(typeof(TContract).GetGenericTypeDefinition()))
            {
                var openImplementation = _map[typeof(TContract).GetGenericTypeDefinition()];
                var closedImplementation = openImplementation.MakeGenericType(
                    typeof(TContract).GetGenericArguments());
                return Create<TContract>(closedImplementation);
            }

            if (!_map.ContainsKey(typeof(TContract)))
            {
                throw new ArgumentException($"No registration found for {typeof(TContract)}");
            }

            // Create an instance and return it 
            return Create<TContract>(_map[typeof(TContract)]);
        }

        /// <summary>
        /// Creates an instance of the specified implementation type.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="implementationType">The type of the implementation.</param>
        /// <returns>An instance of the implementation type.</returns>
        private TContract Create<TContract>(Type implementationType)
        {
            // Get the resolve method. 
            if (_resolveMethod == null)
            {
                _resolveMethod = typeof(IoCContainer).GetMethod("Resolve")!;
            }

            var constructorParameters = implementationType.GetConstructors()
                 .OrderByDescending(c => c.GetParameters().Length)
                 .First()
                 .GetParameters()
                 .Select(p =>
                 {
                     // Make the resolve method generic and invoke it
                     var genericResolveMethod = _resolveMethod?.MakeGenericMethod(p.ParameterType);
                     return genericResolveMethod?.Invoke(this, null);
                 })
                 .ToArray();

            var ivan = new List<double>();

            return (TContract)Activator.CreateInstance(implementationType, constructorParameters);
        }
    }
}
