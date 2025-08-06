using System.Diagnostics;
using System.Reflection;
using System.Windows;
using VSLibrary.Common.MVVM.Interfaces;
using VSLibrary.Common.MVVM.Locators;

namespace VSLibrary.Common.MVVM.Core;

/// <summary>
/// <c>VSContainer</c> is a lightweight dependency injection and service locator framework  
/// designed to manage service registration, instance resolution, and region-based ViewModel binding.
/// </summary>
public class VSContainer : IContainer
{
    /// <summary>
    /// Stores cached instances for each resolved type.  
    /// Used to prevent redundant instantiation and enable reuse across the application.
    /// </summary>
    private readonly Dictionary<Type, object> _instances = new();

    /// <summary>
    /// Stores registered service factories for each service type.  
    /// Multiple factories can be registered per type, each accepting a <c>regionName</c> parameter.
    /// </summary>
    private readonly Dictionary<Type, List<Func<string, object>>> _services = new();

    /// <summary>
    /// Stores mappings between View types and their corresponding ViewModel types.  
    /// Used primarily for automatic ViewModel resolution and binding.
    /// </summary>
    private readonly Dictionary<Type, Type> _viewModelMappings = new();

    /// <summary>
    /// Stores named instances identified by a combination of <see cref="Type"/> and a unique string key.  
    /// Used for resolving dependencies with explicit names.
    /// </summary>
    private readonly Dictionary<(Type, string), object> _namedInstances = new();

    /// <summary>
    /// Holds the internal <see cref="IRegionManager"/> instance managed by the container.
    /// </summary>
    private IRegionManager _regionManager = null!;

    /// <summary>
    /// Holds the singleton instance of <c>VSContainer</c>.  
    /// Initialized lazily through the <see cref="Instance"/> property.
    /// </summary>
    private static VSContainer? _instance;

    /// <summary>
    /// Gets the singleton instance of <c>VSContainer</c>.  
    /// If not already initialized, it will be created with a default <see cref="RegionManager"/>.
    /// </summary>
    public static VSContainer Instance => _instance ??= new VSContainer(new RegionManager());

    /// <summary>
    /// Gets or privately sets the <see cref="IRegionManager"/> associated with this container.  
    /// Throws an exception if a <c>null</c> value is assigned.
    /// </summary>
    public IRegionManager RegionManager
    {
        get => _regionManager;
        private set => _regionManager = value ?? throw new ArgumentNullException(nameof(value), "RegionManager cannot be null.");
    }

    /// <summary>
    /// Initializes a new instance of <c>VSContainer</c> with the specified <see cref="IRegionManager"/>.  
    /// Also sets up the <see cref="ViewModelLocator"/> with this container instance.
    /// </summary>
    /// <param name="regionManager">The region manager used for initialization.</param>
    private VSContainer(IRegionManager regionManager)
    {
        RegionManager = regionManager;

        ViewModelLocator.Initialize(this);
    }

    /// <summary>
    /// Initializes the singleton instance of <c>VSContainer</c> with the specified <see cref="IRegionManager"/>.  
    /// This method can only be called once; subsequent calls will throw an exception.
    /// </summary>
    /// <param name="regionManager">The region manager used to initialize the container.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the container has already been initialized.
    /// </exception>
    public static void InitializeInstance(IRegionManager regionManager)
    {
        if (_instance == null)
        {
            _instance = new VSContainer(regionManager);
        }
        else
        {
            throw new InvalidOperationException("VSContainer has already been initialized.");
        }
    }

    /// <summary>
    /// Registers a service by mapping an interface type to its concrete implementation.  
    /// An instance of <typeparamref name="TImplementation"/> will be created when resolving <typeparamref name="TInterface"/>.
    /// </summary>
    /// <typeparam name="TInterface">The interface type to register.</typeparam>
    /// <typeparam name="TImplementation">
    /// The concrete class that implements <typeparamref name="TInterface"/>.  
    /// Must be a reference type.
    /// </typeparam>
    public void Register<TInterface, TImplementation>() where TImplementation : class, TInterface
    {
        var interfaceType = typeof(TInterface);
        var implementationType = typeof(TImplementation);

        if (!_services.ContainsKey(interfaceType))
        {
            _services[interfaceType] = new List<Func<string, object>>();
        }

        _services[interfaceType].Add(regionName => CreateInstance(implementationType, regionName));
    }

    /// <summary>
    /// Registers a custom factory method for the specified interface type.  
    /// The factory is responsible for creating instances when the type is resolved.
    /// </summary>
    /// <typeparam name="TInterface">The interface type to register.</typeparam>
    /// <param name="factory">A user-defined factory function that creates an instance based on region name.</param>
    public void Register<TInterface>(Func<string, TInterface> factory)
    {
        var interfaceType = typeof(TInterface);

        if (!_services.ContainsKey(interfaceType))
        {
            _services[interfaceType] = new List<Func<string, object>>();
        }

        _services[interfaceType].Add(regionName => factory(regionName ?? string.Empty) ?? throw new InvalidOperationException("Factory returned a null instance."));
    }

    /// <summary>
    /// Registers a concrete (non-interface) class type directly without interface mapping.  
    /// An instance will be created via constructor when resolved.
    /// </summary>
    /// <param name="concreteType">The concrete class type to register.</param>
    public void Register(Type concreteType)
    {
        if (!_services.ContainsKey(concreteType))
        {
            _services[concreteType] = new List<Func<string, object>>();
        }

        _services[concreteType].Add(regionName => CreateInstance(concreteType, regionName));
    }

    /// <summary>
    /// Registers an instance under the specified generic interface type.  
    /// The instance must implement <typeparamref name="TInterface"/>.
    /// </summary>
    /// <typeparam name="TInterface">The interface type to register the instance under.</typeparam>
    /// <param name="instance">The instance to register.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the provided <paramref name="instance"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the provided instance does not implement <typeparamref name="TInterface"/>.
    /// </exception>
    public void RegisterInstance<TInterface>(TInterface instance)
    {
        if (instance == null)
        {
            throw new ArgumentNullException(nameof(instance), "Instance to register cannot be null.");
        }

        var interfaceType = typeof(TInterface);
        var instanceType = instance.GetType();

        if (!interfaceType.IsAssignableFrom(instanceType))
        {
            throw new InvalidOperationException($"{instanceType.FullName} does not implement {interfaceType.FullName}.");
        }

        if (!_services.ContainsKey(interfaceType))
        {
            _services[interfaceType] = new List<Func<string, object>>();
        }

        _services[interfaceType].Add(_ => (TInterface)instance);
    }

    /// <summary>
    /// Registers an instance under the specified interface <see cref="Type"/>.  
    /// The instance must implement the provided interface type.
    /// </summary>
    /// <param name="interfaceType">The interface type to register the instance under.</param>
    /// <param name="instance">The instance implementing the specified interface type.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="interfaceType"/> or <paramref name="instance"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the provided instance does not implement <paramref name="interfaceType"/>.
    /// </exception>
    public void RegisterInstance(Type interfaceType, object instance)
    {
        if (interfaceType == null) throw new ArgumentNullException(nameof(interfaceType));
        if (instance == null) throw new ArgumentNullException(nameof(instance));

        if (!interfaceType.IsAssignableFrom(instance.GetType()))
        {
            throw new InvalidOperationException($"{instance.GetType().FullName} does not implement {interfaceType.FullName}.");
        }

        if (!_services.ContainsKey(interfaceType))
        {
            _services[interfaceType] = new List<Func<string, object>>();
        }

        _services[interfaceType].Add(_ => instance);
    }

    /// <summary>
    /// Registers an instance under the specified generic interface type with a unique key.  
    /// The instance must implement the given interface type.
    /// </summary>
    /// <typeparam name="TInterface">The interface type to register the instance under.</typeparam>
    /// <param name="key">A unique key used to distinguish the instance.</param>
    /// <param name="instance">The instance implementing <typeparamref name="TInterface"/>.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the provided <paramref name="instance"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the provided instance does not implement <typeparamref name="TInterface"/>.
    /// </exception>
    public void RegisterInstance<TInterface>(string key, TInterface instance)
    {
        if (instance == null)
        {
            throw new ArgumentNullException(nameof(instance), "Instance to register cannot be null.");
        }

        var interfaceType = typeof(TInterface);
        var instanceType = instance.GetType();

        if (!interfaceType.IsAssignableFrom(instanceType))
        {
            throw new InvalidOperationException($"{instanceType.FullName} does not implement {interfaceType.FullName}.");
        }

        if (!_services.ContainsKey(interfaceType))
        {
            _services[interfaceType] = new List<Func<string, object>>();
        }

        _services[interfaceType].Add(_ => instance);
    }

    /// <summary>
    /// Resolves an instance of the specified type.  
    /// If a cached instance exists and <paramref name="createNew"/> is <c>false</c>, it will be reused.  
    /// Otherwise, a new instance is created using a registered factory or by invoking the constructor.
    /// 
    /// Region name injection and activation logic are applied after instantiation,  
    /// including optional support for <see cref="IActivatable"/>.
    /// </summary>
    /// <param name="type">The type of the object to resolve.</param>
    /// <param name="createNew">
    /// Whether to force creation of a new instance.  
    /// Defaults to <c>false</c>, meaning cached instance (if any) is reused.
    /// </param>
    /// <param name="regionName">
    /// The region name to inject into the resolved instance.  
    /// Defaults to <c>null</c>.
    /// </param>
    /// <returns>The resolved instance of the specified type.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the instance cannot be created (e.g., missing factory or constructor failure).
    /// </exception>
    public object Resolve(Type type, bool createNew = false, string regionName = null!)
    {
        if (!createNew)
        {
            if (_instances.TryGetValue(type, out var cachedInstance))
            {
                return cachedInstance;
            }

            _instances[type] = null!;
        }

        object instance = null!;

        if (_services.TryGetValue(type, out var factories) && factories.Count > 0)
        {
            var factory = factories.Last();
            instance = factory(regionName ?? string.Empty);
        }
        else
        {
            instance = CreateInstance(type, regionName);
        }

        if (!createNew)
        {
            _instances[type] = instance;
        }

        if (!string.IsNullOrEmpty(regionName))
        {
            InjectRegionName(instance, regionName);
        }

        if (instance is IActivatable activatable)
        {
            activatable.Activate();
        }

        return instance;
    }

    /// <summary>
    /// Injects the specified region name into the given instance.  
    /// If the instance implements <see cref="IRegionAware"/>, the value is set directly.  
    /// Otherwise, the method attempts to set a public writable <c>RegionName</c> property via reflection.
    /// </summary>
    /// <param name="instance">The target object to inject the region name into.</param>
    /// <param name="regionName">The region name to inject.</param>
    private void InjectRegionName(object instance, string regionName)
    {
        if (instance == null || regionName == null) return;

        if (instance is IRegionAware regionAware)
        {
            regionAware.RegionName = regionName;
        }
        else
        {
            var property = instance.GetType().GetProperty("RegionName");
            if (property != null && property.CanWrite)
            {
                property.SetValue(instance, regionName);
            }
        }
    }

    /// <summary>
    /// Resolves a single instance of the specified type.  
    /// You can optionally force creation of a new instance or inject a region name.
    /// </summary>
    /// <typeparam name="T">The type of the instance to resolve.</typeparam>
    /// <param name="createNew">
    /// If set to <c>true</c>, a new instance will be created even if one is cached.  
    /// Defaults to <c>false</c>.
    /// </param>
    /// <param name="regionName">
    /// An optional region name to be injected during creation.  
    /// Defaults to <c>null</c>.
    /// </param>
    /// <returns>The resolved instance of type <typeparamref name="T"/>.</returns>
    public T Resolve<T>(bool createNew = false, string regionName = null!)
    {
        return (T)Resolve(typeof(T), createNew, regionName);
    }

    /// <summary>
    /// Resolves all registered instances of the specified type.  
    /// Returns a list of instances created using the associated factories.
    /// </summary>
    /// <typeparam name="T">The type of the instances to resolve.</typeparam>
    /// <returns>A list containing all resolved instances of type <typeparamref name="T"/>.</returns>
    public List<T> ResolveAll<T>()
    {
        var type = typeof(T);
        var results = new List<T>();

        if (_services.TryGetValue(type, out var factories))
        {
            foreach (var factory in factories)
            {
                var instance = factory(null!);
                if (instance is T t)
                    results.Add(t);
            }
        }

        return results;
    }

    /// <summary>
    /// Registers an instance under the specified interface type and unique key.  
    /// This allows multiple named instances for the same interface type.
    /// </summary>
    /// <typeparam name="TInterface">The interface type to register the instance under.</typeparam>
    /// <param name="key">A unique key to identify the instance.</param>
    /// <param name="instance">The instance to register.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the provided <paramref name="instance"/> is <c>null</c>.
    /// </exception>
    public void RegisterNamedInstance<TInterface>(string key, TInterface instance)
    {
        if (instance == null)
            throw new ArgumentNullException(nameof(instance), "Instance to register cannot be null.");

        var type = typeof(TInterface);
        _namedInstances[(type, key)] = instance!;
    }

    /// <summary>
    /// Resolves a previously registered instance by interface type and a unique key.  
    /// The key allows multiple instances to be registered for the same interface type.
    /// </summary>
    /// <typeparam name="TInterface">The interface type of the instance to resolve.</typeparam>
    /// <param name="key">The unique key that identifies the instance.</param>
    /// <returns>The instance associated with the given interface type and key.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if no instance is found for the specified type and key combination.
    /// </exception>
    public TInterface ResolveNamedInstance<TInterface>(string key)
    {
        var type = typeof(TInterface);
        if (_namedInstances.TryGetValue((type, key), out var value))
            return (TInterface)value;

        throw new InvalidOperationException($"No instance found for key '{key}' and type [{type.Name}].");
    }

    /// <summary>
    /// Creates an instance of the specified type by resolving its constructor parameters automatically.  
    /// The instance is cached for future use.
    /// </summary>
    /// <param name="type">The type of the object to create.</param>
    /// <param name="regionName">
    /// The region name to inject into constructor parameters, if applicable.  
    /// Defaults to <c>null</c>.
    /// </param>
    /// <returns>The newly created instance.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if no usable constructor is found, or if the instance cannot be created.
    /// </exception>
    private object CreateInstance(Type type, string regionName = null!)
    {
        var constructor = type.GetConstructors()
            .OrderByDescending(c => c.GetParameters().Length)
            .FirstOrDefault();

        if (constructor == null)
        {
            throw new InvalidOperationException($"No usable constructor found for type '{type.Name}'.");
        }

        var parameters = constructor.GetParameters()
            .Select(p => ResolveParameter(p, regionName))
            .ToArray();

        var instance = Activator.CreateInstance(type, parameters);

        if (instance == null)
        {
            throw new InvalidOperationException($"Failed to create an instance of type '{type.Name}'.");
        }

        _instances[type] = instance;

        return instance; // Ensure instance is not null due to the above check.
    }

    /// <summary>
    /// Resolves a constructor parameter by type.  
    /// If the parameter is of type <see cref="string"/>, the provided region name is returned.  
    /// If the parameter is of type <see cref="Lazy{T}"/>, this method builds a deferred factory:  
    /// - Uses a cached instance if available  
    /// - Otherwise wraps the factory registered for the service type  
    /// - If no factory is found, falls back to creating a new instance via Resolve  
    /// For all other types, it delegates resolution to <c>Resolve()</c>.
    /// </summary>
    /// <param name="parameter">The constructor parameter to resolve.</param>
    /// <param name="regionName">The region name passed to factories or used as a string value.</param>
    /// <returns>The resolved value to inject into the constructor.</returns>
    private object ResolveParameter(ParameterInfo parameter, string regionName)
    {
        if (parameter.ParameterType == typeof(string) && regionName != null)
            return regionName;

        if (parameter.ParameterType.IsGenericType &&
            parameter.ParameterType.GetGenericTypeDefinition() == typeof(Lazy<>))
        {
            var serviceType = parameter.ParameterType.GetGenericArguments().First();

            if (_instances.TryGetValue(serviceType, out var existing) && existing != null)
            {
                object? rtn = Activator.CreateInstance(
                    typeof(Lazy<>).MakeGenericType(serviceType),
                    new Func<object>(() => existing));

                return rtn!;
            }

            if (_services.TryGetValue(serviceType, out var factories) && factories.Count > 0)
            {
                var factory = factories.Last();
                var funcType = typeof(Func<>).MakeGenericType(serviceType);

                var methodInfo = typeof(VSContainer).GetMethod(nameof(WrapFactory), BindingFlags.NonPublic | BindingFlags.Static)
                    ?.MakeGenericMethod(serviceType)
                    ?? throw new InvalidOperationException("WrapFactory method not found.");

                var typedFunc = methodInfo.Invoke(null, new object[] { factory, regionName ?? string.Empty });

                var lazyCtor = typeof(Lazy<>).MakeGenericType(serviceType)
                    .GetConstructor(new[] { funcType })
                    ?? throw new InvalidOperationException($"Constructor not found for Lazy<{serviceType.Name}>.");

                return lazyCtor.Invoke(new[] { typedFunc });
            }

            object? activatorReturn = Activator.CreateInstance(
                typeof(Lazy<>).MakeGenericType(serviceType),
                new Func<object>(() => Resolve(serviceType, true, regionName ?? string.Empty)));

            return activatorReturn!;
        }

        return Resolve(parameter.ParameterType, regionName: regionName ?? string.Empty);
    }

    /// <summary>
    /// Creates a <see cref="Func{T}"/> delegate suitable for use with <see cref="Lazy{T}"/>,  
    /// using the specified factory and region name.  
    /// This enables deferred instantiation of the object while preserving type safety.
    /// </summary>
    /// <typeparam name="T">The type of object to be created.</typeparam>
    /// <param name="factory">The factory delegate that takes a region name and returns an object.</param>
    /// <param name="regionName">The region name to be passed into the factory.</param>
    /// <returns>A strongly typed <see cref="Func{T}"/> that can be used for lazy instantiation.</returns>
    /// <exception cref="InvalidCastException">
    /// Thrown if the factory returns an object that cannot be cast to type <typeparamref name="T"/>.
    /// </exception>
    private static Func<T> WrapFactory<T>(Func<string, object> factory, string regionName)
    {
        return () =>
        {
            var result = factory(regionName);
            if (result is not T typed)
                throw new InvalidCastException($"Cannot cast the factory result to type {typeof(T).Name}.");
            return typed;
        };
    }

    /// <summary>
    /// Removes the cached instance associated with the specified type.  
    /// This allows a new instance to be created the next time it is resolved.
    /// </summary>
    /// <param name="type">The type whose cached instance should be removed.</param>
    public void ClearCache(Type type)
    {
        _instances.Remove(type);
    }

    /// <summary>
    /// Clears all cached instances that were registered for reuse.  
    /// This effectively resets the instance container managed internally.
    /// </summary>
    public void ClearAllCaches()
    {
        _instances.Clear();
    }

    /// <summary>
    /// Performs a series of automatic initialization tasks through a single entry point.  
    /// 
    /// This method includes:
    /// - Registering core services related to region and view management
    /// - Automatically registering Views and ViewModels based on naming conventions
    /// - (Optional) Debugging the current region-view mappings
    /// 
    /// If <paramref name="assembly"/> is not provided, the calling assembly is used by default.
    /// </summary>
    /// <param name="assembly">
    /// The assembly to scan for View and ViewModel types.  
    /// If null, <see cref="Assembly.GetCallingAssembly"/> is used.
    /// </param>
    public void AutoInitialize(Assembly? assembly = null)
    {
        Register<IViewManager, ViewManager>();
        RegisterInstance<IRegionManager>(RegionManager);
        RegisterInstance<INavigator>(RegionManager);

        AutoRegisterViewsAndViewModels(assembly);

        // DebugRegionMappings(); // Optional: for debugging view-region registration
    }

    /// <summary>
    /// Automatically registers Views and ViewModels by scanning the specified assembly.  
    /// 
    /// The method applies the following conventions:
    /// - A View is any non-abstract class that inherits from <see cref="FrameworkElement"/>.
    /// - A ViewModel is any non-abstract class whose name ends with "ViewModel".
    /// - For each View type, it looks for a matching ViewModel named "{ViewName}ViewModel".
    /// - When a match is found, it registers the View and ViewModel pair.
    ///
    /// If <paramref name="assembly"/> is not provided, the calling assembly will be used.
    /// </summary>
    /// <param name="assembly">
    /// The assembly to scan for View and ViewModel types.  
    /// If null, <see cref="Assembly.GetCallingAssembly"/> is used.
    /// </param>
    public void AutoRegisterViewsAndViewModels(Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();

        var types = assembly.GetTypes();

        var viewTypes = types.Where(t =>
            t.IsClass &&
            !t.IsAbstract &&
            typeof(FrameworkElement).IsAssignableFrom(t)).ToList();

        var viewModelTypes = types.Where(t =>
            t.IsClass &&
            !t.IsAbstract &&
            t.Name.EndsWith("ViewModel")).ToList();

        foreach (var viewType in viewTypes)
        {
            var viewModelName = $"{viewType.Name}ViewModel";
            var viewModelType = viewModelTypes.FirstOrDefault(vm => vm.Name == viewModelName);

            if (viewModelType != null)
            {
                RegisterView(viewType, viewModelType);
            }
        }
    }

    /// <summary>
    /// Registers the specified View and ViewModel types by mapping them together.  
    /// This overload accepts <see cref="Type"/> arguments instead of generic parameters.  
    /// 
    /// The method performs the following:
    /// - Maps the View type to its associated ViewModel type (if not already registered).
    /// - Registers a ViewModel factory:
    ///   - If an instance already exists, it is reused.
    ///   - Otherwise, a new instance is created.
    /// - Registers a View factory:
    ///   - Ensures that duplicate factory entries are not added.
    /// </summary>
    /// <param name="viewType">The View type to register.</param>
    /// <param name="viewModelType">The ViewModel type to be associated with the View.</param>
    public void RegisterView(Type viewType, Type viewModelType)
    {
        if (!_viewModelMappings.ContainsKey(viewType))
        {
            _viewModelMappings[viewType] = viewModelType;
        }

        if (!_services.TryGetValue(viewModelType, out var viewModelFactories))
        {
            viewModelFactories = new List<Func<string, object>>();
            _services[viewModelType] = viewModelFactories;
        }

        if (_instances.TryGetValue(viewModelType, out var instance) && instance != null)
        {
            viewModelFactories.Add(_ => instance);
            _instances[viewModelType] = instance;
        }
        else
        {
            viewModelFactories.Add(regionName => CreateInstance(viewModelType, regionName));
        }

        if (!_services.TryGetValue(viewType, out var viewFactories))
        {
            viewFactories = new List<Func<string, object>>();
            _services[viewType] = viewFactories;
        }

        if (!viewFactories.Any(factory =>
            Delegate.Equals(factory, (Func<string, object>)(regionName => CreateViewInstance(viewType)!))))
        {
            if (!viewFactories.Any(factory =>
                factory != null && Delegate.Equals(factory, (Func<string, object>)(regionName => CreateViewInstance(viewType)!))))
            {
                viewFactories.Add(regionName =>
                {
                    var viewInstance = CreateViewInstance(viewType);
                    return viewInstance!;
                });
            }
            viewFactories.Add(regionName =>
            {
                var viewInstance = CreateViewInstance(viewType);
                return viewInstance!;
            });
        }
    }

    /// <summary>
    /// Registers the specified View and ViewModel types.  
    /// This method performs the following tasks:
    /// - Maps the View type to its corresponding ViewModel type.
    /// - Registers a factory for the ViewModel:
    ///   - If an instance already exists, it will be reused.
    ///   - Otherwise, a new instance will be created per region.
    /// - Registers a factory for the View:
    ///   - Always creates a new instance when requested.
    /// </summary>
    /// <typeparam name="TView">The View type to register.</typeparam>
    /// <typeparam name="TViewModel">The ViewModel type to be mapped to the View.</typeparam>
    /// <exception cref="InvalidOperationException">Thrown if an error occurs during ViewModel registration.</exception>
    public void RegisterView<TView, TViewModel>() where TViewModel : class
    {
        var viewType = typeof(TView);
        var viewModelType = typeof(TViewModel);

        if (!_viewModelMappings.ContainsKey(viewType))
        {
            _viewModelMappings[viewType] = viewModelType;
        }

        if (!_services.TryGetValue(viewModelType, out var viewModelFactories))
        {
            viewModelFactories = new List<Func<string, object>>();
            _services[viewModelType] = viewModelFactories;
        }

        if (_instances.TryGetValue(viewModelType, out var instance) && instance != null)
        {
            viewModelFactories.Add(_ => instance);
        }
        else
        {
            viewModelFactories.Add(regionName => CreateInstance(viewModelType, regionName));
        }

        if (!_services.TryGetValue(viewType, out var viewFactories))
        {
            viewFactories = new List<Func<string, object>>();
            _services[viewType] = viewFactories;
        }

        viewFactories.Add(regionName =>
        {
            var viewInstance = CreateViewInstance(viewType);
            return viewInstance!;
        });
    }

    /// <summary>
    /// Assigns the corresponding ViewModel to the DataContext of the given View,  
    /// establishing the binding between the View and its ViewModel.
    /// </summary>
    /// <param name="view">The View whose DataContext will be assigned.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when there is no registered ViewModel for the specified View type.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the provided <paramref name="view"/> is not of type <see cref="FrameworkElement"/>.
    /// </exception>
    public void ResolveView(object view)
    {
        if (view is FrameworkElement frameworkElement)
        {
            var viewType = frameworkElement.GetType();

            if (_viewModelMappings.TryGetValue(viewType, out var viewModelType))
            {
                string regionName = null!;
                if (frameworkElement is IRegionAware regionAwareView)
                {
                    regionName = regionAwareView.RegionName;
                }

                var viewModel = Resolve(viewModelType, regionName: regionName);
                if (viewModel != null)
                    frameworkElement.DataContext = viewModel;
            }
            else
            {
                throw new InvalidOperationException($"No ViewModel registered for View type '{viewType.Name}'.");
            }
        }
        else
        {
            throw new ArgumentException("The provided view must be of type FrameworkElement.", nameof(view));
        }
    }

    /// <summary>
    /// Returns the ViewModel type that is mapped to the specified View type.
    /// </summary>
    /// <param name="viewType">The View type to look up the corresponding ViewModel for.</param>
    /// <returns>
    /// The ViewModel type mapped to the given View type.  
    /// Returns <c>null</c> if no mapping exists.
    /// </returns>
    public Type? GetViewModelType(Type viewType)
    {
        _viewModelMappings.TryGetValue(viewType, out var viewModelType);
        return viewModelType;
    }

    /// <summary>
    /// Creates an instance of the specified view type and binds its corresponding ViewModel.
    /// Throws if the view cannot be instantiated.
    /// </summary>
    /// <param name="viewType">The type of the view to create.</param>
    /// <returns>The created view instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the view instance could not be created.</exception>
    private FrameworkElement? CreateViewInstance(Type viewType)
    {
        if (Activator.CreateInstance(viewType) is FrameworkElement viewInstance)
        {
            ResolveView(viewInstance);
            return viewInstance;
        }

        throw new InvalidOperationException($"'{viewType.Name}' view could not be created.");
    }

    /// <summary>
    /// Returns the ViewModel instance mapped to the given view name. 
    /// Returns null if no mapping is found.
    /// </summary>
    /// <param name="viewName">The name of the view.</param>
    /// <returns>The ViewModel instance, or null if not mapped.</returns>
    public object? GetViewModel(string viewName)
    {
        var viewType = _viewModelMappings.Keys.FirstOrDefault(v => v.Name == viewName);

        if (viewType != null && _viewModelMappings.TryGetValue(viewType, out var viewModelType))
        {
            return Resolve(viewModelType);
        }

        return null;
    }

    /// <summary>
    /// Retrieves the view instance by its name. Returns null if not registered.
    /// </summary>
    /// <param name="viewName">The name of the view.</param>
    /// <returns>The resolved view instance, or null if not mapped.</returns>
    public object? GetView(string viewName)
    {
        var viewType = _viewModelMappings.Keys.FirstOrDefault(v => v.Name == viewName);
        if (viewType == null) return null;

        if (_instances.TryGetValue(viewType, out var cachedView))
            return cachedView;

        var newView = Resolve(viewType);
        _instances[viewType] = newView;
        return newView;
    }

    /// <summary>
    /// Outputs the current Region-to-View mapping and ContentControl status for debugging purposes.
    /// </summary>
    public void DebugRegionMappings()
    {
        Debug.WriteLine("Registered RegionName and View mappings:");

        if (RegionManager is RegionManager rm)
        {
            foreach (var region in rm.Regions)
            {
                Debug.WriteLine($"RegionName: {region.Key}, ViewType: {region.Value.Name}");
            }

            Debug.WriteLine("Current ContentControl status:");
            foreach (var regionControl in rm.RegionControls)
            {
                Debug.WriteLine($"RegionName: {regionControl.Key}, Has ContentControl: {regionControl.Value != null}");
            }
        }
    }
}
