using PsychologyApp.Application.Conversation.Companion;

namespace PsychologyApp.Presentation.Shared.Platform;

/// <summary>Reads total RAM and free storage. Only Android reports real values; elsewhere the on-device model is not offered.</summary>
public sealed class DeviceCapabilities : IDeviceCapabilities
{
    public long TotalMemoryBytes
    {
        get
        {
#if ANDROID
            try
            {
                if (Android.App.Application.Context.GetSystemService(Android.Content.Context.ActivityService) is Android.App.ActivityManager manager)
                {
                    Android.App.ActivityManager.MemoryInfo info = new();
                    manager.GetMemoryInfo(info);
                    return info.TotalMem;
                }
            }
            catch (Exception)
            {
                // Unknown memory is treated as "not capable".
            }
#endif
            return 0;
        }
    }

    public long FreeStorageBytes
    {
        get
        {
            try
            {
                return new DriveInfo(FileSystem.AppDataDirectory).AvailableFreeSpace;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
