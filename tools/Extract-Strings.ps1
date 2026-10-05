# One-off migration: moves the text of `public static string X => T("ru", "en");` properties of AppStrings into
# StringsRu.resx and StringsEn.resx and rewrites the property to `=> R(nameof(X))`.
param([string]$Dir = "$PSScriptRoot\..\PsychologyApp.Presentation.Core\Common")

$bs = [string][char]92
$ErrorActionPreference = 'Stop'
$lit = '"(?:[^"' + $bs + $bs + ']|' + $bs + $bs + '.)*"'
$rx = New-Object Text.RegularExpressions.Regex("public static string (\w+)\s*=>\s*T\(\s*($lit)\s*,\s*($lit)\s*\)\s*;")
$esc = New-Object Text.RegularExpressions.Regex($bs + $bs + '(u[0-9A-Fa-f]{4}|.)')

function Unescape([string]$quoted) {
    $s = $quoted.Substring(1, $quoted.Length - 2)
    $esc.Replace($s, [Text.RegularExpressions.MatchEvaluator]{
        param($m)
        $c = $m.Groups[1].Value
        if ($c.Length -eq 5) { return [string][char][Convert]::ToInt32($c.Substring(1), 16) }
        switch ($c) { 'n' { "`n" } 't' { "`t" } 'r' { "`r" } default { $c } }
    })
}

$ru = [ordered]@{}; $en = [ordered]@{}
foreach ($file in Get-ChildItem $Dir -Filter 'AppStrings*.cs') {
    $text = [IO.File]::ReadAllText($file.FullName)
    $new = $rx.Replace($text, [Text.RegularExpressions.MatchEvaluator]{
        param($m)
        $name = $m.Groups[1].Value
        $ru[$name] = Unescape $m.Groups[2].Value
        $en[$name] = Unescape $m.Groups[3].Value
        "public static string $name => R(nameof($name));"
    })
    if ($new -ne $text) { [IO.File]::WriteAllText($file.FullName, $new, (New-Object Text.UTF8Encoding $false)) }
}

function WriteResx($path, $map) {
    $settings = New-Object Xml.XmlWriterSettings
    $settings.Indent = $true
    $settings.Encoding = New-Object Text.UTF8Encoding $false
    $w = [Xml.XmlWriter]::Create($path, $settings)
    $w.WriteStartDocument()
    $w.WriteStartElement('root')
    $w.WriteStartElement('resheader'); $w.WriteAttributeString('name', 'resmimetype'); $w.WriteElementString('value', 'text/microsoft-resx'); $w.WriteEndElement()
    $w.WriteStartElement('resheader'); $w.WriteAttributeString('name', 'version'); $w.WriteElementString('value', '2.0'); $w.WriteEndElement()
    $w.WriteStartElement('resheader'); $w.WriteAttributeString('name', 'reader'); $w.WriteElementString('value', 'System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'); $w.WriteEndElement()
    $w.WriteStartElement('resheader'); $w.WriteAttributeString('name', 'writer'); $w.WriteElementString('value', 'System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'); $w.WriteEndElement()
    foreach ($k in $map.Keys) {
        $w.WriteStartElement('data'); $w.WriteAttributeString('name', $k)
        $w.WriteAttributeString('xml', 'space', $null, 'preserve')
        $w.WriteElementString('value', $map[$k]); $w.WriteEndElement()
    }
    $w.WriteEndElement(); $w.WriteEndDocument(); $w.Close()
}
WriteResx (Join-Path $Dir 'StringsRu.resx') $ru
WriteResx (Join-Path $Dir 'StringsEn.resx') $en
"Moved $($ru.Count) strings"
