# Adds new texts to StringsRu.resx / StringsEn.resx and the matching properties to AppStrings.cs.
# Usage: Add-Strings.ps1 -Entries @{ Key = @('русский', 'English') }   (see the call at the bottom of this file for the format)
param([string]$Dir = "$PSScriptRoot\..\PsychologyApp.Presentation.Core\Common", [string]$Marker = 'public static string DataBackupImportFailedToast')
$ErrorActionPreference = 'Stop'

$entries = [ordered]@{
    BreathInhale = @('Вдох', 'Inhale')
    BreathHold = @('Пауза', 'Hold')
    BreathExhale = @('Выдох', 'Exhale')
    BreathStart = @('Начать', 'Start')
    BreathStop = @('Остановить', 'Stop')
    BreathAgain = @('Ещё раз', 'Again')
    BreathCycleFormat = @('Цикл {0} из {1}', 'Cycle {0} of {1}')
    BreathReady = @('Сядьте удобно и нажмите «Начать». Дышите вместе с кругом.', 'Sit comfortably and press Start. Breathe with the circle.')
    BreathDone = @('Хорошо. Теперь оцените напряжение после практики.', 'Well done. Now rate your tension after the practice.')
    BreathCircleLabel = @('Дыхательный круг: расширяется на вдохе, сжимается на выдохе', 'Breathing circle: grows as you inhale, shrinks as you exhale')
    TensionPickHint = @('Проведите пальцем по полосе, чтобы выбрать от 0 до 10', 'Slide along the bar to pick from 0 to 10')
    TensionPickConfirm = @('Выбрать', 'Choose')
    TensionChangeFormat = @('Было {0} → стало {1}', 'Was {0} → now {1}')
    TensionCalmWord = @('спокойно', 'calm')
    TensionStrongWord = @('очень сильно', 'very strong')
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
