using System.Text;
using PsychologyApp.Presentation.Shared.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public class AudioDownloadPolicyTests
{
    [Theory]
    [InlineData("https://azbyka.ru/audio/a.mp3", true)]
    [InlineData("http://azbyka.ru/audio/a.mp3", false)]
    [InlineData("file:///sdcard/a.mp3", false)]
    [InlineData("ftp://host/a.mp3", false)]
    [InlineData("a.mp3", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void Only_https_urls_are_allowed(string? url, bool expected) =>
        Assert.Equal(expected, AudioDownloadPolicy.IsAllowedUrl(url));

    [Theory]
    [InlineData("audio/mpeg", true)]
    [InlineData("AUDIO/MP3", true)]
    [InlineData("application/octet-stream", true)]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("text/html", false)]
    [InlineData("application/json", false)]
    [InlineData("image/png", false)]
    public void Html_and_other_non_audio_content_types_are_rejected(string? type, bool expected) =>
        Assert.Equal(expected, AudioDownloadPolicy.IsAcceptableContentType(type));

    [Fact]
    public void An_mp3_with_an_id3_tag_is_audio() =>
Assert.True(AudioDownloadPolicy.LooksLikeAudio([(byte)'I', (byte)'D', (byte)'3', 4, 0, 0, 0, 0]));

    [Fact]
    public void An_mp3_frame_without_a_tag_is_audio() =>
        Assert.True(AudioDownloadPolicy.LooksLikeAudio([0xFF, 0xFB, 0x90, 0x44, 0, 0, 0, 0]));

    [Fact]
    public void Ogg_flac_and_wav_containers_are_audio()
    {
        Assert.True(AudioDownloadPolicy.LooksLikeAudio("OggS"u8.ToArray()));
        Assert.True(AudioDownloadPolicy.LooksLikeAudio("fLaC"u8.ToArray()));
        Assert.True(AudioDownloadPolicy.LooksLikeAudio("RIFF"u8.ToArray()));
    }


    [Fact]
    public void An_mp4_container_is_audio()
    {
        byte[] head = [0, 0, 0, 0x20, (byte)'f', (byte)'t', (byte)'y', (byte)'p', (byte)'M', (byte)'4', (byte)'A', (byte)' '];

        Assert.True(AudioDownloadPolicy.LooksLikeAudio(head));
    }

    [Theory]
    [InlineData("<!DOCTYPE html>")]
    [InlineData("<html><head>")]
    [InlineData("{\"error\":\"not found\"}")]
    [InlineData("404 Not Found")]
    public void Error_pages_are_not_audio(string text) =>
        Assert.False(AudioDownloadPolicy.LooksLikeAudio(Encoding.ASCII.GetBytes(text)));

    [Fact]
    public void An_empty_or_tiny_file_is_not_audio()
    {
        Assert.False(AudioDownloadPolicy.LooksLikeAudio([]));
        Assert.False(AudioDownloadPolicy.LooksLikeAudio([0xFF, 0xFB]));
    }
}
