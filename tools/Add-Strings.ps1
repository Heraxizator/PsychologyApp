# Adds new texts to StringsRu.resx / StringsEn.resx and the matching properties to AppStrings.cs.
# Usage: Add-Strings.ps1 -Entries @{ Key = @('русский', 'English') }   (see the call at the bottom of this file for the format)
param([string]$Dir = "$PSScriptRoot\..\PsychologyApp.Presentation.Core\Common", [string]$Marker = 'public static string DataBackupImportFailedToast')
$ErrorActionPreference = 'Stop'

$entries = [ordered]@{
    SkeletonLoadingLabel = @('Загрузка', 'Loading')
}

function AddData($path, $name, $value) {
    $doc = New-Object Xml.XmlDocument; $doc.PreserveWhitespace = $true; $doc.Load($path)
    foreach ($d in $doc.root.data) { if ($d.name -eq $name) { return $false } }
    $data = $doc.CreateElement('data'); $data.SetAttribute('name', $name); $data.SetAttribute('xml:space', 'preserve')
    $v = $doc.CreateElement('value'); $v.InnerText = $value
    [void]$data.AppendChild($doc.CreateWhitespace("`n    ")); [void]$data.AppendChild($v); [void]$data.AppendChild($doc.CreateWhitespace("`n  "))
    [void]$doc.root.AppendChild($doc.CreateWhitespace("  ")); [void]$doc.root.AppendChild($data); [void]$doc.root.AppendChild($doc.CreateWhitespace("`n"))
    $doc.Save($path)
    return $true
}

$cs = Join-Path $Dir 'AppStrings.cs'
$text = [IO.File]::ReadAllText($cs)
$props = ''
foreach ($k in $entries.Keys) {
    [void](AddData (Join-Path $Dir 'StringsRu.resx') $k $entries[$k][0])
    [void](AddData (Join-Path $Dir 'StringsEn.resx') $k $entries[$k][1])
    if ($text -notmatch "public static string $k ") { $props += "    public static string $k => R(nameof($k));`r`n" }
}
if ($props) {
    $i = $text.IndexOf($Marker)
    if ($i -lt 0) { throw "marker not found" }
    $lineStart = $text.LastIndexOf("`n", $i) + 1
    $text = $text.Insert($lineStart, $props)
    [IO.File]::WriteAllText($cs, $text, (New-Object Text.UTF8Encoding $false))
}
"done"
