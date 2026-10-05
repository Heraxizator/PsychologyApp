# One-off migration, second step: methods of the form
#   public static string Name(args) => T($"ru {a}", $"en {a}");
# become  => F(nameof(Name), a)  with "ru {0}" / "en {0}" in StringsRu.resx / StringsEn.resx.
# Only literals whose holes are plain identifiers are moved; anything else stays in code.
param([string]$Dir = "$PSScriptRoot\..\PsychologyApp.Presentation.Core\Common")
$ErrorActionPreference = 'Stop'

$bs = [string][char]92
$ilit = '\$"(?:[^"{}' + $bs + $bs + ']|' + $bs + $bs + '.|\{\w+\})*"'
$rx = New-Object Text.RegularExpressions.Regex("public static string (\w+)\(([^)]*)\)\s*=>\s*T\(\s*($ilit)\s*,\s*($ilit)\s*\)\s*;")
$esc = New-Object Text.RegularExpressions.Regex($bs + $bs + '(u[0-9A-Fa-f]{4}|.)')
$hole = New-Object Text.RegularExpressions.Regex('\{(\w+)\}')

function Body([string]$quoted) {
    $s = $quoted.Substring(2, $quoted.Length - 3)
    $esc.Replace($s, [Text.RegularExpressions.MatchEvaluator]{
        param($m)
        $c = $m.Groups[1].Value
        if ($c.Length -eq 5) { return [string][char][Convert]::ToInt32($c.Substring(1), 16) }
        switch ($c) { 'n' { "`n" } 't' { "`t" } 'r' { "`r" } default { $c } }
    })
}

$ruPath = Join-Path $Dir 'StringsRu.resx'; $enPath = Join-Path $Dir 'StringsEn.resx'
$ruDoc = New-Object Xml.XmlDocument; $ruDoc.PreserveWhitespace = $true; $ruDoc.Load($ruPath)
$enDoc = New-Object Xml.XmlDocument; $enDoc.PreserveWhitespace = $true; $enDoc.Load($enPath)
$existing = @{}
foreach ($d in $ruDoc.root.data) { $existing[$d.name] = $true }
$seen = @{}
$added = 0

function AddData($doc, $name, $value) {
    $data = $doc.CreateElement('data')
    $data.SetAttribute('name', $name)
    $data.SetAttribute('xml:space', 'preserve')
    $v = $doc.CreateElement('value'); $v.InnerText = $value
    [void]$data.AppendChild($doc.CreateWhitespace("`n    ")); [void]$data.AppendChild($v); [void]$data.AppendChild($doc.CreateWhitespace("`n  "))
    [void]$doc.root.AppendChild($doc.CreateWhitespace("  ")); [void]$doc.root.AppendChild($data); [void]$doc.root.AppendChild($doc.CreateWhitespace("`n"))
}

foreach ($file in Get-ChildItem $Dir -Filter 'AppStrings*.cs') {
    $text = [IO.File]::ReadAllText($file.FullName)
    $new = $rx.Replace($text, [Text.RegularExpressions.MatchEvaluator]{
        param($m)
        $name = $m.Groups[1].Value
        if ($existing.ContainsKey($name) -or $seen.ContainsKey($name)) { return $m.Value }
        $ruBody = Body $m.Groups[3].Value; $enBody = Body $m.Groups[4].Value
        $order = New-Object Collections.Generic.List[string]
        foreach ($b in @($ruBody, $enBody)) { foreach ($h in $hole.Matches($b)) { if (-not $order.Contains($h.Groups[1].Value)) { $order.Add($h.Groups[1].Value) } } }
        $params = $m.Groups[2].Value
        foreach ($id in $order) { if ($params -notmatch "\b$id\b") { return $m.Value } }
        $fmt = { param($b) $hole.Replace($b, [Text.RegularExpressions.MatchEvaluator]{ param($h) '{' + $order.IndexOf($h.Groups[1].Value) + '}' }) }
        $seen[$name] = $true
        AddData $ruDoc $name (& $fmt $ruBody)
        AddData $enDoc $name (& $fmt $enBody)
        $script:added++
        $argList = ($order | ForEach-Object { $_ }) -join ', '
        if ($order.Count -eq 0) { return "public static string $name($params) => R(nameof($name));" }
        "public static string $name($params) => F(nameof($name), $argList);"
    })
    if ($new -ne $text) { [IO.File]::WriteAllText($file.FullName, $new, (New-Object Text.UTF8Encoding $false)) }
}
$ruDoc.Save($ruPath); $enDoc.Save($enPath)
"Moved $added format strings"
