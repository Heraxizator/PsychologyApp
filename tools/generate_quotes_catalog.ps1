$ErrorActionPreference = "Stop"
$Root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
if (-not (Test-Path "$Root\PsychologyApp.Presentation")) {
    $Root = Split-Path -Parent $PSScriptRoot
}

$RuPath = Join-Path $Root "PsychologyApp.Presentation\Resources\Raw\quotes\quotes.ru.json"
$EnPath = Join-Path $Root "PsychologyApp.Presentation\Resources\Raw\quotes\quotes.en.json"
$Target = 525

$Themes = @(
    "wisdom", "motivation", "resilience", "mindfulness", "self-awareness", "anxiety",
    "calm", "gratitude", "healing", "self-love", "acceptance", "habits",
    "hope", "empathy", "relationships", "general"
)

$RuTemplates = @(
    "Маленький шаг {0} сегодня уже меняет завтра.",
    "Замечать {0} — начало заботы о себе.",
    "Спокойствие растёт там, где есть принятие {0}.",
    "Ты имеешь право двигаться в своём темпе, даже когда {0}.",
    "Дыхание может стать якорем, когда {0} кажется слишком большим.",
    "Сострадание к себе — не слабость, а опора при {0}.",
    "Один честный взгляд на {0} снимает часть напряжения.",
    "Не нужно решать всё сразу: достаточно одного шага про {0}.",
    "Тело часто знает ответ раньше мыслей — прислушайся к {0}.",
    "Тревога уменьшается, когда {0} делится на маленькие части.",
    "Надежда начинается с признания: сейчас мне трудно из‑за {0}.",
    "Забота о себе — это практика, а не разовый подвиг про {0}.",
    "Ты не обязан(а) быть сильным(ой) каждый день, особенно когда {0}.",
    "Принятие {0} не отменяет желание перемен — оно даёт опору для них.",
    "Мягкость к себе сегодня — ресурс для завтра, даже если {0}.",
    "Иногда лучшая помощь — пауза и честность про {0}.",
    "Ты уже проходил(а) через трудное — {0} тоже можно прожить.",
    "Смысл не в идеальности, а в возвращении к себе после {0}.",
    "Поддержка начинается с фразы: мне важно, что я чувствую про {0}.",
    "Каждый новый день — шанс по‑новому отнестись к {0}.",
    "Сравнение с другими лишь усиливает боль, когда {0}.",
    "Твоё переживание про {0} имеет право на место.",
    "Можно одновременно хотеть лучшего и принимать {0} сейчас.",
    "Замедление — навык, особенно полезный при {0}.",
    "Мысль — не приказ; можно заметить {0} и выбрать ответ.",
    "Забота о границах защищает энергию, когда {0} давит.",
    "Ты не один(одна) со своим {0} — помощь допустима.",
    "Маленькая благодарность за {0} может сменить настроение дня.",
    "Привычка замечать {0} без осуждения укрепляет устойчивость.",
    "Сегодня достаточно сделать чуть меньше, если {0} истощает.",
    "Путь к ясности часто проходит через честность про {0}.",
    "Тишина иногда лечит лучше советов, когда {0}."
)

$EnTemplates = @(
    "A small step {0} today already changes tomorrow.",
    "Noticing {0} is the beginning of self-care.",
    "Calm grows where there is acceptance of {0}.",
    "You are allowed to move at your own pace, even when {0}.",
    "Breath can be an anchor when {0} feels too big.",
    "Self-compassion is not weakness but support when {0}.",
    "One honest look at {0} releases some tension.",
    "You do not have to solve everything at once—one step about {0} is enough.",
    "The body often knows the answer before thoughts—listen to {0}.",
    "Anxiety eases when {0} is broken into small parts.",
    "Hope begins by admitting: it is hard for me because of {0}.",
    "Self-care is a practice, not a one-time feat about {0}.",
    "You do not have to be strong every day, especially when {0}.",
    "Accepting {0} does not cancel change—it supports it.",
    "Gentleness with yourself today is fuel for tomorrow, even if {0}.",
    "Sometimes the best help is a pause and honesty about {0}.",
    "You have gotten through hard things before—{0} can be lived through too.",
    "The point is not perfection but returning to yourself after {0}.",
    "Support starts with: what I feel about {0} matters.",
    "Each new day is a chance to relate differently to {0}.",
    "Comparison with others only adds pain when {0}.",
    "Your experience of {0} deserves space.",
    "You can want better and still accept {0} now.",
    "Slowing down is a skill, especially useful with {0}.",
    "A thought is not a command—you can notice {0} and choose a response.",
    "Boundaries protect energy when {0} feels heavy.",
    "You are not alone with your {0}—asking for help is allowed.",
    "A small gratitude for {0} can shift the day.",
    "The habit of noticing {0} without judgment builds resilience.",
    "Today it is enough to do a little less if {0} drains you.",
    "Clarity often passes through honesty about {0}.",
    "Silence sometimes heals better than advice when {0}."
)

$RuNouns = @(
    "усталость", "тревога", "неопределённость", "одиночество", "разочарование",
    "сомнение", "боль", "злость", "стыд", "вина", "перегруз", "бессонница",
    "нежность к себе", "границы", "прошлое", "будущее", "ожидания", "критика",
    "неудача", "изменения", "пауза", "выбор", "отдых", "поддержка", "принятие",
    "надежда", "смысл", "ценность", "доверие", "близость", "смелость", "терпение"
)

$EnNouns = @(
    "fatigue", "anxiety", "uncertainty", "loneliness", "disappointment",
    "doubt", "pain", "anger", "shame", "guilt", "overload", "insomnia",
    "self-kindness", "boundaries", "the past", "the future", "expectations", "criticism",
    "failure", "change", "a pause", "a choice", "rest", "support", "acceptance",
    "hope", "meaning", "worth", "trust", "closeness", "courage", "patience"
)

$AuthorsRu = @("Практика осознанности", "Совет психолога", "Стоическая мудрость", "Заметка о заботе", "Поддержка на каждый день")
$AuthorsEn = @("Mindfulness practice", "Counselor's note", "Stoic wisdom", "Care reminder", "Daily support")

$existingRu = Get-Content $RuPath -Raw -Encoding UTF8 | ConvertFrom-Json
$existingEn = Get-Content $EnPath -Raw -Encoding UTF8 | ConvertFrom-Json

$seen = [System.Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
foreach ($item in $existingRu) { [void]$seen.Add($item.text) }

$newRu = New-Object System.Collections.Generic.List[object]
$newEn = New-Object System.Collections.Generic.List[object]
$needed = $Target - $existingRu.Count
$idx = 0

foreach ($theme in $Themes) {
    for ($t = 0; $t -lt $RuTemplates.Count; $t++) {
        $nounRu = $RuNouns[($idx + $t) % $RuNouns.Count]
        $nounEn = $EnNouns[($idx + $t) % $EnNouns.Count]
        $authorRu = $AuthorsRu[($idx + $t) % $AuthorsRu.Count]
        $authorEn = $AuthorsEn[($idx + $t) % $AuthorsEn.Count]
        $textRu = $RuTemplates[$t] -f $nounRu
        if ($seen.Contains($textRu)) { continue }
        [void]$seen.Add($textRu)
        $newRu.Add([pscustomobject]@{ author = $authorRu; text = $textRu; theme = $theme })
        $newEn.Add([pscustomobject]@{ author = $authorEn; text = ($EnTemplates[$t] -f $nounEn); theme = $theme })
        if ($newRu.Count -ge $needed) { break }
        $idx++
    }
    if ($newRu.Count -ge $needed) { break }
    $idx++
}

if ($newRu.Count -lt $needed) { throw "Generated only $($newRu.Count) new quotes; need $needed" }

$mergedRu = @($existingRu) + @($newRu)
$mergedEn = @($existingEn) + @($newEn)

if ($mergedRu.Count -ne $Target -or $mergedEn.Count -ne $Target) {
    throw "Expected $Target quotes, got RU=$($mergedRu.Count) EN=$($mergedEn.Count)"
}

$mergedRu | ConvertTo-Json -Depth 5 | Set-Content $RuPath -Encoding UTF8
$mergedEn | ConvertTo-Json -Depth 5 | Set-Content $EnPath -Encoding UTF8
Write-Host "Wrote $Target quotes to quotes.ru.json and quotes.en.json"
