namespace PsychologyApp.Presentation.Shared.Common;

/// <summary>
/// The country the phone is in, as a two-letter code, for choosing help numbers. The mobile network says where the phone
/// actually is (a Russian SIM roaming in Spain needs 112 and 024); without a network the SIM, then the phone's region
/// setting. Never the app's language. Read on the device only; nothing leaves it.
/// </summary>
public static class DeviceCountry
{
    /// <summary>Set by tests to stand in for the phone.</summary>
    internal static string? Override { get; set; }

    public static string? Code => Override ?? Read();

    private static string? Read()
    {
        try
        {
#if ANDROID
            var context = Android.App.Application.Context;
            if (context.GetSystemService(Android.Content.Context.TelephonyService) is Android.Telephony.TelephonyManager telephony)
            {
                foreach (string? iso in new[] { telephony.NetworkCountryIso, telephony.SimCountryIso })
                {
                    if (!string.IsNullOrWhiteSpace(iso))
                    {
                        return iso.ToUpperInvariant();
                    }
                }
            }

            // The system locale, not the app's: the app switches its own culture to the chosen interface language.
            string? region = Android.Content.Res.Resources.System?.Configuration?.Locales.Get(0)?.Country;
            return string.IsNullOrWhiteSpace(region) ? null : region.ToUpperInvariant();
#else
            return System.Globalization.RegionInfo.CurrentRegion.TwoLetterISORegionName;
#endif
        }
        catch (Exception bestEffortError)
        {
            // Without a country the screen still offers 112 and the international directory.
            BestEffort.Report(bestEffortError);
            return null;
        }
    }
}
