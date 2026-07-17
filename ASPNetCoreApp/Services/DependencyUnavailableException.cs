namespace ASPNetCoreApp.Services;

public class DependencyUnavailableException(string dependencyName, string message) : Exception(message)
{
    public string DependencyName { get; } = dependencyName;
}
