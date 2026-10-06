# One-off migration, fifth step: `T($"ru {expr}", $"en {expr}")` pairs (holes may hold any expression without quotes) become
# `F("Method.N", expr...)` with `{0}` / `{0:fmt}` placeholders in StringsRu.resx / StringsEn.resx. Pairs whose two texts are identical
# (nothing to translate) and pairs with quotes inside a hole stay in code.
param([string]$Dir = "$PSScriptRoot\..\PsychologyApp.Presentation.Core\Common")
$ErrorActionPreference = 'Stop'
$meth = New-Object Text.RegularExpressions.Regex("(?m)^\s*(?:public|private|internal) static (?:string|IReadOnlyList<string>) (\w+)")

# Reads an interpolated literal starting at $i (pointing at the $). Returns @{ Text; Holes; End } or $null.
function ReadInterpolated([string]$s, [int]$i) {
    if ($s[$i] -ne '$' -or $s[$i + 1] -ne '"') { return $null }
    $p = $i + 2
    $sb = New-Object Text.StringBuilder
    $holes = New-Object Collections.Generic.List[object]
    while ($p -lt $s.Length) {
        $c = $s[$p]
        if ($c -eq '"') { return @{ Text = $sb.ToString(); Holes = $holes; End = $p + 1 } }
        if ($c -eq [char]92) { [void]$sb.Append($c).Append($s[$p + 1]); $p += 2; continue }
        if ($c -eq '{') {
            if ($s[$p + 1] -eq '{') { [void]$sb.Append('{{'); $p += 2; continue }
            $depth = 1; $q = $p + 1
            while ($q -lt $s.Length -and $depth -gt 0) {
                if ($s[$q] -eq '"') { return $null }
                if ($s[$q] -eq '{') { $depth++ } elseif ($s[$q] -eq '}') { $depth-- }
                $q++
            }
            $inner = $s.Substring($p + 1, $q - $p - 2)
            $format = ''
            $expr = $inner
            $colon = $inner.LastIndexOf(':')
            if ($colon -gt 0 -and $inner.IndexOf('?') -lt 0 -and $inner.IndexOf('(') -lt 0) { $expr = $inner.Substring(0, $colon); $format = $inner.Substring($colon) }
            $holes.Add(@{ Expr = $expr.Trim(); Format = $format })
            [void]$sb.Append([char]0).Append([string]($holes.Count - 1)).Append([char]1)
            $p = $q
            continue
        }
        if ($c -eq '}' -and $s[$p + 1] -eq '}') { [void]$sb.Append('}}'); $p += 2; continue }
        [void]$sb.Append($c); $p++
    }
    return $null
}

function UnescapeText([string]$t) {
    $bs = [string][char]92
    $r = [regex]::Replace($t, [regex]::Escape($bs) + '(.)', [Text.RegularExpressions.MatchEvaluator]{
        param($m)
        switch ($m.Groups[1].Value) { 'n' { "`n" } 't' { "`t" } 'r' { "`r" } default { $m.Groups[1].Value } }
    })
    return $r.Replace('{{', '{{').Replace('}}', '}}')
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
$added = 0

foreach ($file in Get-ChildItem $Dir -Filter 'AppStrings*.cs') {
    $text = [IO.File]::ReadAllText($file.FullName)
    $methods = @($meth.Matches($text) | ForEach-Object { [pscustomobject]@{ Pos = $_.Index; Name = $_.Groups[1].Value } })
    $out = New-Object Text.StringBuilder
    $pos = 0
    $callRx = New-Object Text.RegularExpressions.Regex('\bT\(\s*(?=\$")')
    foreach ($m in $callRx.Matches($text)) {
        if ($m.Index -lt $pos) { continue }
        $first = ReadInterpolated $text ($m.Index + $m.Length)
        if ($null -eq $first) { continue }
        $k = $first.End
        while ($text[$k] -match '\s') { $k++ }
        if ($text[$k] -ne ',') { continue }
        $k++
        while ($text[$k] -match '\s') { $k++ }
        $second = ReadInterpolated $text $k
        if ($null -eq $second) { continue }
        $e = $second.End
        while ($text[$e] -match '\s') { $e++ }
        if ($text[$e] -ne ')') { continue }
        $e++

        # Same text in both languages: nothing to translate.
        $a = $first.Text; $b = $second.Text
        if ($a -ceq $b) { continue }
        $owner = $methods | Where-Object { $_.Pos -lt $m.Index } | Select-Object -Last 1
        if ($null -eq $owner -or $owner.Name -eq 'T') { continue }

        $exprs = New-Object Collections.Generic.List[string]
        foreach ($lit in @($first, $second)) { foreach ($h in $lit.Holes) { if (-not $exprs.Contains($h.Expr)) { $exprs.Add($h.Expr) } } }
        function Render($lit) {
            $t = UnescapeText $lit.Text
            for ($i = 0; $i -lt $lit.Holes.Count; $i++) {
                $h = $lit.Holes[$i]
                $t = $t.Replace([string][char]0 + [string]$i + [string][char]1, '{' + $exprs.IndexOf($h.Expr) + $h.Format + '}')
            }
            return $t
        }
        $n = 1
        while ($used.ContainsKey($owner.Name + '.' + $n)) { $n++ }
        $key = $owner.Name + '.' + $n
        $used[$key] = $true
        AddData $ruDoc $key (Render $first)
        AddData $enDoc $key (Render $second)
        $added++
        [void]$out.Append($text.Substring($pos, $m.Index - $pos))
        [void]$out.Append('F("' + $key + '", ' + ($exprs -join ', ') + ')')
        $pos = $e
    }
    [void]$out.Append($text.Substring($pos))
    if ($out.ToString() -ne $text) { [IO.File]::WriteAllText($file.FullName, $out.ToString(), (New-Object Text.UTF8Encoding $false)) }
}
$ruDoc.Save($ruPath); $enDoc.Save($enPath)
"Moved $added interpolated texts"
