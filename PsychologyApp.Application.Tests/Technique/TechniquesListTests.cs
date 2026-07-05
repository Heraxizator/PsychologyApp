using Moq;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Technique;
using DomainTechnique = PsychologyApp.Domain.Entities.Technique;
using Xunit;

namespace PsychologyApp.Application.Tests.Technique;

/// <summary>
/// Covers dynamic technique list loading used by TechniquesViewModel.InitAsync.
/// </summary>
public class TechniquesListTests
{
    [Fact]
    public async Task GetTechniquesListAsync_ReturnsMappedDtos()
    {
        var entity = DomainTechnique.Create(5, "1", "2026-01-01", "Header", "Desc", "Subj", "Author", "alg", "img");
        var repository = new Mock<ITechniqueRepository>();
        repository.Setup(r => r.GetLatestPageAsync(0, 500, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { entity });

        var service = new TechniqueService(repository.Object);

        var result = (await service.GetTechniquesListAsync(500)).ToList();

        Assert.Single(result);
        Assert.Equal(5, result[0].TechniqueId);
        Assert.Equal("Header", result[0].Header);
    }

    [Fact]
    public async Task GetTechniquesPageAsync_ReturnsMappedDtosWithOffset()
    {
        var entities = new[]
        {
            DomainTechnique.Create(5, "1", "2026-01-01", "Header", "Desc", "Subj", "Author", "alg", "img"),
            DomainTechnique.Create(4, "1", "2026-01-01", "Older", "Desc", "Subj", "Author", "alg", "img")
        };
        var repository = new Mock<ITechniqueRepository>();
        repository.Setup(r => r.GetLatestPageAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { entities[1] });

        var service = new TechniqueService(repository.Object);

        var result = (await service.GetTechniquesPageAsync(1, 1)).ToList();

        Assert.Single(result);
        Assert.Equal(4, result[0].TechniqueId);
        Assert.Equal("Older", result[0].Header);
    }
}
