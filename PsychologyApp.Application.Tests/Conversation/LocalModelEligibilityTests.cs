using PsychologyApp.Application.Conversation.Companion;
using Xunit;

namespace PsychologyApp.Application.Tests.Conversation;

public class LocalModelEligibilityTests
{
    private const long Gb = 1024L * 1024 * 1024;

    private sealed record Device(long TotalMemoryBytes, long FreeStorageBytes) : IDeviceCapabilities;

    private static readonly LocalModelManifest Manifest = new(
        "m", "M", "x", [new LocalModelFile("a", "https://x/a", 4 * Gb)], ["en"], MinTotalMemoryBytes: 7 * Gb);

    [Fact]
    public void Capable_device_is_eligible() =>
        Assert.Equal(LocalModelEligibility.Eligible, LocalModelEligibilityChecker.Check(Manifest, new Device(8 * Gb, 10 * Gb)));

    [Fact]
    public void Too_little_memory_is_reported_before_storage() =>
        Assert.Equal(LocalModelEligibility.NotEnoughMemory, LocalModelEligibilityChecker.Check(Manifest, new Device(4 * Gb, 0)));

    [Fact]
    public void Unknown_memory_counts_as_not_capable() =>
        Assert.Equal(LocalModelEligibility.NotEnoughMemory, LocalModelEligibilityChecker.Check(Manifest, new Device(0, 100 * Gb)));

    [Theory]
    [InlineData(4 * Gb)]
    [InlineData(4 * Gb + 200 * 1024 * 1024)]
    public void Storage_must_cover_the_model_with_headroom(long free) =>
        Assert.Equal(LocalModelEligibility.NotEnoughStorage, LocalModelEligibilityChecker.Check(Manifest, new Device(8 * Gb, free)));

    [Fact]
    public void Exactly_enough_storage_with_headroom_is_eligible() =>
        Assert.Equal(LocalModelEligibility.Eligible, LocalModelEligibilityChecker.Check(Manifest, new Device(8 * Gb, (long)(4 * Gb * 1.15) + 1)));
}
