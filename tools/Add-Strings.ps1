# Adds new texts to StringsRu.resx / StringsEn.resx and the matching properties to AppStrings.cs.
# Usage: Add-Strings.ps1 -Entries @{ Key = @('русский', 'English') }   (see the call at the bottom of this file for the format)
param([string]$Dir = "$PSScriptRoot\..\PsychologyApp.Presentation.Core\Common", [string]$Marker = 'public static string DataBackupImportFailedToast')
$ErrorActionPreference = 'Stop'

$entries = [ordered]@{
    BackupProtectTitle = @('Защитить копию паролем?', 'Protect the backup with a passphrase?')
    BackupProtectBody = @('В копии чаты, заметки и план безопасности. Файл с паролем нельзя прочитать без него.', 'The backup holds chats, notes and the safety plan. A file with a passphrase cannot be read without it.')
    BackupProtectWith = @('С паролем (рекомендуется)', 'With a passphrase (recommended)')
    BackupProtectWithout = @('Без защиты', 'Without protection')
    BackupPassphraseTitle = @('Пароль для копии', 'Backup passphrase')
    BackupPassphraseNewBody = @('Придумайте пароль не короче 8 символов. Восстановить его нельзя: без него копию не открыть.', 'Choose a passphrase of at least 8 characters. It cannot be recovered: without it the backup cannot be opened.')
    BackupPassphraseConfirmBody = @('Повторите пароль.', 'Repeat the passphrase.')
    BackupPassphraseOpenBody = @('Копия защищена паролем. Введите его.', 'This backup is protected by a passphrase. Enter it.')
    BackupPassphrasePlaceholder = @('Пароль', 'Passphrase')
    BackupPassphraseTooShortToast = @('Пароль короче 8 символов', 'The passphrase is shorter than 8 characters')
    BackupPassphraseMismatchToast = @('Пароли не совпадают', 'The passphrases do not match')
    BackupPassphraseWrongToast = @('Неверный пароль или файл изменён', 'Wrong passphrase, or the file was changed')
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
