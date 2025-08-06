using System.Reflection;

namespace VSLibrary.Common.MVVM.Interfaces;

/// <summary>
/// Interface for a container that manages dependency injection and View/ViewModel mapping
/// specifically designed for MVVM frameworks.
/// </summary>
public interface IContainer
{
    /// <summary>
    /// <summary>
    /// Registers a View and its corresponding ViewModel.
    /// </summary>
    /// <typeparam name="TView">The type of the View to register.</typeparam>
    /// <typeparam name="TViewModel">The type of the ViewModel associated with the View.</typeparam>
    void RegisterView<TView, TViewModel>() where TViewModel : class;

    /// <summary>
    /// Registers an interface and its implementation.
    /// </summary>
    /// <typeparam name="TInterface">The interface type to register.</typeparam>
    /// <typeparam name="TImplementation">The implementing class of the interface.</typeparam>
    void Register<TInterface, TImplementation>() where TImplementation : class, TInterface;

    /// <summary>
    /// Registers an already created instance.
    /// </summary>
    /// <typeparam name="TInterface">The interface type of the instance.</typeparam>
    /// <param name="instance">The instance to register.</param>
    void RegisterInstance<TInterface>(TInterface instance);

    /// <summary>
    /// Registers an instance with a specified interface type.
    /// </summary>
    /// <param name="interfaceType">The interface type to register.</param>
    /// <param name="instance">The instance to register.</param>
    void RegisterInstance(Type interfaceType, object instance);

    /// <summary>
    /// Registers a named instance with a specified key.
    /// </summary>
    /// <typeparam name="TInterface">The interface type of the instance.</typeparam>
    /// <param name="key">The key identifying the instance.</param>
    /// <param name="instance">The instance to register.</param>
    void RegisterInstance<TInterface>(string key, TInterface instance);

    /// <summary>
    /// Resolves an instance of the specified type.
    /// </summary>
    /// <typeparam name="T">The type to resolve.</typeparam>
    /// <param name="createNew">If true, creates a new instance; otherwise, returns a cached instance.</param>
    /// <param name="regionName">Optional region name for ViewModel mapping.</param>
    /// <returns>The resolved instance of the specified type.</returns>
    T Resolve<T>(bool createNew = false, string regionName = null!);

    /// <summary>
    /// Resolves all registered instances of the specified type.
    /// </summary>
    /// <typeparam name="T">The type to resolve.</typeparam>
    /// <returns>A list of all instances of the specified type.</returns>
    List<T> ResolveAll<T>();

    /// <summary>
    /// Resolves a named instance by its key.
    /// </summary>
    /// <typeparam name="TInterface">The interface type of the instance.</typeparam>
    /// <param name="key">The key identifying the instance.</param>
    /// <returns>The registered instance associated with the key.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no instance is registered with the specified key.</exception>
    TInterface ResolveNamedInstance<TInterface>(string key);

    /// <summary>
    /// Resolves an instance of the specified type.
    /// </summary>
    /// <param name="type">The type to resolve.</param>
    /// <param name="createNew">If true, creates a new instance; otherwise, returns a cached instance.</param>
    /// <param name="regionName">Optional region name for ViewModel mapping.</param>
    /// <returns>The resolved instance of the specified type.</returns>
    object Resolve(Type type, bool createNew = false, string regionName = null!);

    /// <summary>
    /// Returns the ViewModel type mapped to the specified View type.
    /// </summary>
    /// <param name="viewType">The View type.</param>
    /// <returns>The mapped ViewModel type, or null if none exists.</returns>
    Type? GetViewModelType(Type viewType);

    /// <summary>
    /// Automatically registers Views and ViewModels from the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly to scan for Views and ViewModels. If null, uses the calling assembly.</param>
    void AutoInitialize(Assembly? assembly = null);    
}
