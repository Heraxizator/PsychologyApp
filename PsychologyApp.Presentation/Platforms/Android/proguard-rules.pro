# MAUI Release + R8: keep Java glue that .NET handlers reflect into at startup.
# Without these, VisualElement/handlers fail static init (TypeInitializationException).

-keep class com.microsoft.maui.** { *; }
-keep class com.microsoft.maui.PlatformInterop { *; }
-keep class com.microsoft.maui.PlatformFontSpan { *; }

# Mono / generated JNI callable wrappers (MauiApplication, handlers, Shell).
-keep class mono.** { *; }
-keep class mono.android.** { *; }
-keep class crc* { *; }

-keepattributes *Annotation*
-keepclasseswithmembernames class * {
    native <methods>;
}

# Option B: umbrella keep for AndroidX — MAUI Shell, photo picker, Startup provider, MediaElement.
# Point -keep per package fails whack-a-mole; -keepclassmembers does not prevent class removal.
-keep class androidx.** { *; }
-keep class com.google.android.material.** { *; }
