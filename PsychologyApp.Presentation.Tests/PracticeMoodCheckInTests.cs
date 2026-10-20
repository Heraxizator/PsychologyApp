using Moq;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

/// <summary>The mood picked after a practice is one journal entry, however many times the face is changed.</summary>
public sealed class PracticeMoodCheckInTests
{
    private readonly Mock<IUserProgressService> _progress = new();

    public PracticeMoodCheckInTests() =>
        _progress
            .Setup(p => p.RecordMoodAsync(It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(42);

    [Fact]
    public async Task Changing_the_face_corrects_the_entry_made_by_the_first_one()
    {
        PracticeMoodCheckIn checkIn = new(_progress.Object);

        await checkIn.SaveAsync(3, null);
        await checkIn.SaveAsync(4, null);
        await checkIn.SaveAsync(5, "легче");

        _progress.Verify(p => p.RecordMoodAsync(3, null, null, It.IsAny<CancellationToken>()), Times.Once);
        _progress.Verify(p => p.RecordMoodAsync(It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()), Times.Once);
        _progress.Verify(p => p.UpdateMoodEntryAsync(42, 4, null, It.IsAny<CancellationToken>()), Times.Once);
        _progress.Verify(p => p.UpdateMoodEntryAsync(42, 5, "легче", It.IsAny<CancellationToken>()), Times.Once);
    }
}
