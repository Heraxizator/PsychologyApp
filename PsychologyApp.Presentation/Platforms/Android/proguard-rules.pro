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

# AndroidX / Material used during Shell and app startup.
-keep class androidx.appcompat.** { *; }
-keep class com.google.android.material.** { *; }
-keepclassmembers class androidx.startup.** {
    public <init>(...);
}
-keepclassmembers class androidx.core.app.** {
    public <init>(...);
}

# CommunityToolkit MediaElement / ExoPlayer (if R8 reaches app dex).
-keep class androidx.media3.** { *; }
