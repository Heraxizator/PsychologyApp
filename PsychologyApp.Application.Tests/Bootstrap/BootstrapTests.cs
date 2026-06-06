using Microsoft.Extensions.DependencyInjection;
using PsychologyApp.Application.Services.TechniqueService;
using PsychologyApp.Bootstrap;
using Xunit;

namespace PsychologyApp.Application.Tests.Bootstrap;

public class BootstrapTests
{
    [Fact]
    public void AddPsychologyAppCore_RegistersTechniqueService()
    {
        ServiceCollection services = new();
        services.AddPsychologyAppCore();
        ServiceProvider provider = services.BuildServiceProvider();
        Assert.NotNull(provider.GetService<ITechniqueService>());
    }
}
