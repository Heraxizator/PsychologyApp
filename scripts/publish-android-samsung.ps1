# Release arm64 APK for Samsung phones (sideload). Requires AndroidSigning.local.props.
$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$proj = Join-Path $root "PsychologyApp.Presentation\PsychologyApp.Presentation.csproj"
$signingProps = Join-Path $root "PsychologyApp.Presentation\AndroidSigning.local.props"

if (-not (Test-Path $signingProps)) {
  Write-Error "Missing $signingProps — copy AndroidSigning.local.props.example and set keystore/passwords."
}

dotnet clean $proj
dotnet publish $proj `
  -f net10.0-android `
  -c Release `
  -p:RuntimeIdentifier=android-arm64 `
  -p:AndroidPackageFormat=apk

$apk = Join-Path $root "PsychologyApp.Presentation\bin\Release\net10.0-android\android-arm64\com.subconscious.calm-Signed.apk"
if (-not (Test-Path $apk)) {
  $apk = Get-ChildItem (Join-Path $root "PsychologyApp.Presentation\bin\Release\net10.0-android") -Recurse -Filter "com.subconscious.calm-Signed.apk" | Select-Object -First 1 -ExpandProperty FullName
}
Write-Host ""
Write-Host "APK: $apk"
Write-Host "Install: adb install `"$apk`""
