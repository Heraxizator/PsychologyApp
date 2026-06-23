using Moq;
using Xunit;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Exceptions;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Technique;
using DomainTechnique = PsychologyApp.Domain.Entities.Technique;

namespace PsychologyApp.Application.Tests.Technique;

public class TechniqueServiceTests
{
    [Fact]
    public async Task GetTechniqueByIdAsync_WhenMissing_ThrowsNotFound()
    {
        var repository = new Mock<ITechniqueRepository>();
        repository.Setup(r => r.GetByIdAsync(42, It.IsAny<CancellationToken>())).ReturnsAsync((DomainTechnique?)null);

        var service = new TechniqueService(repository.Object);

        await Assert.ThrowsAsync<TechniqueNotFoundException>(() => service.GetTechniqueByIdAsync(42));
    }

    [Fact]
    public async Task UpdateTechniqueAsync_WhenEditFails_ThrowsNotFound()
    {
        var technique = DomainTechnique.Create(1, "1", "d", "h", "desc", "s", "a", "alg", "img");
        var repository = new Mock<ITechniqueRepository>();
        repository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(technique);
        repository.Setup(r => r.EditAsync(It.IsAny<DomainTechnique>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new PersistenceException("not updated"));

        var service = new TechniqueService(repository.Object);
        var dto = TechniqueMapper.GetTechniqueDTO(technique);

        await Assert.ThrowsAsync<PersistenceException>(() => service.UpdateTechniqueAsync(dto));
    }

    [Fact]
    public async Task DeleteTechniqueAsync_WhenDeleteFails_ThrowsNotFound()
    {
        var technique = DomainTechnique.Create(1, "1", "d", "h", "desc", "s", "a", "alg", "img");
        var repository = new Mock<ITechniqueRepository>();
        repository.Setup(r => r.DeleteAsync(It.IsAny<DomainTechnique>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new PersistenceException("not deleted"));

        var service = new TechniqueService(repository.Object);
        var dto = TechniqueMapper.GetTechniqueDTO(technique);

        await Assert.ThrowsAsync<PersistenceException>(() => service.DeleteTechniqueAsync(dto));
    }

    [Fact]
    public async Task UpdateTechniqueAsync_WhenCompleted_PreservesIsCompleted()
    {
        var technique = DomainTechnique.Create(1, "1", "d", "h", "desc", "s", "a", "alg", "img");
        technique.MarkAsCompleted();

        var repository = new Mock<ITechniqueRepository>();
        repository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(technique);
        repository.Setup(r => r.EditAsync(It.IsAny<DomainTechnique>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var service = new TechniqueService(repository.Object);
        var dto = TechniqueMapper.GetTechniqueDTO(technique) with { Header = "updated header" };

        await service.UpdateTechniqueAsync(dto);

        Assert.True(technique.IsCompleted);
        repository.Verify(r => r.EditAsync(It.Is<DomainTechnique>(t => t.IsCompleted), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void GetTechniqueDTO_MapsIsCompleted()
    {
        var technique = DomainTechnique.Create(1, "1", "d", "h", "desc", "s", "a", "alg", "img");
        technique.MarkAsCompleted();

        TechniqueDTO dto = TechniqueMapper.GetTechniqueDTO(technique);

        Assert.True(dto.IsCompleted);
    }

    [Fact]
    public async Task AddNewTechniqueAsync_PersistsEntity()
    {
        var technique = DomainTechnique.Create(0, "1", "2026-01-01", "h", "d", "s", "a", "alg", "img");
        var repository = new Mock<ITechniqueRepository>();
        var service = new TechniqueService(repository.Object);
        var dto = TechniqueMapper.GetTechniqueDTO(technique);

        await service.AddNewTechniqueAsync(dto);

        repository.Verify(r => r.AddAsync(It.IsAny<DomainTechnique>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
