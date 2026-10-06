# One-off migration, fourth step: the remaining plain `T("ru", "en")` calls (the two branches of a condition) become
# `R("Method.1")`, `R("Method.2")` ... in order of appearance, with the texts in StringsRu.resx / StringsEn.resx.
param([string]$Dir = "$PSScriptRoot\..\PsychologyApp.Presentation.Core\Common")
$ErrorActionPreference = 'Stop'
$bs = [string][char]92
$lit = '"(?:[^"' + $bs + $bs + ']|' + $bs + $bs + '.)*"'
$call = New-Object Text.RegularExpressions.Regex("\bT\(\s*($lit)\s*,\s*($lit)\s*\)")
$meth = New-Object Text.RegularExpressions.Regex("(?m)^\s*(?:public|private|internal) static (?:string|IReadOnlyList<string>) (\w+)")
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
function AddData($doc, $name, $value) {
    $data = $doc.CreateElement('data'); $data.SetAttribute('name', $name); $data.SetAttribute('xml:space', 'preserve')
    $v = $doc.CreateElement('value'); $v.InnerText = $value
    [void]$data.AppendChild($doc.CreateWhitespace("`n    ")); [void]$data.AppendChild($v); [void]$data.AppendChild($doc.CreateWhitespace("`n  "))
    [void]$doc.root.AppendChild($doc.CreateWhitespace("  ")); [void]$doc.root.AppendChild($data); [void]$doc.root.AppendChild($doc.CreateWhitespace("`n"))
}

$ruPath = Join-Path $Dir 'StringsRu.resx'; $enPath = Join-Path $Dir 'StringsEn.resx'
$ruDoc = New-Object Xml.XmlDocument; $ruDoc.PreserveWhitespace = $true; $ruDoc.Load($ruPath)
$enDoc = New-Object Xml.XmlDocument; $enDoc.PreserveWhitespace = $true; $enDoc.Load($enPath)
$used = @{}; foreach ($d in $ruDoc.root.data) { $used[$d.name] = $true }
$script:added = 0
$counters = @{}

foreach ($file in Get-ChildItem $Dir -Filter 'AppStrings*.cs') {
    $text = [IO.File]::ReadAllText($file.FullName)
    $methods = @($meth.Matches($text) | ForEach-Object { [pscustomobject]@{ Pos = $_.Index; Name = $_.Groups[1].Value } })
    $new = $call.Replace($text, [Text.RegularExpressions.MatchEvaluator]{
        param($m)
        $owner = $methods | Where-Object { $_.Pos -lt $m.Index } | Select-Object -Last 1
        if ($null -eq $owner) { return $m.Value }
        if ($owner.Name -eq 'T') { return $m.Value }
        $counters[$owner.Name] = 1 + [int]$counters[$owner.Name]
        $key = $owner.Name + '.' + $counters[$owner.Name]
        if ($used.ContainsKey($key)) { return $m.Value }
        $used[$key] = $true
        AddData $ruDoc $key (Unescape $m.Groups[1].Value)
        AddData $enDoc $key (Unescape $m.Groups[2].Value)
        $script:added++
        'R("' + $key + '")'
    })
    if ($new -ne $text) { [IO.File]::WriteAllText($file.FullName, $new, (New-Object Text.UTF8Encoding $false)) }
}
$ruDoc.Save($ruPath); $enDoc.Save($enPath)
"Moved $script:added texts"
