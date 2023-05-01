// Taken with gratitude from sprectreconsole/sprectre.console

using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;

namespace TerevintoSoftware.AadAppRegistry.Tool.Services;

[ExcludeFromCodeCoverage]
internal sealed class TypeResolver : ITypeResolver, IDisposable
{
    private readonly IServiceProvider _provider;

    public TypeResolver(IServiceProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public object Resolve(Type type)
    {
        if (type == null)
        {
            return null;
        }

        return _provider.GetService(type);
    }

    public void Dispose()
    {
        if (_provider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}