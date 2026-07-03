#!/usr/bin/env python3
"""Generate quotes.ru.json and quotes.en.json with 525 curated entries."""

from __future__ import annotations

import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
RU_PATH = ROOT / "PsychologyApp.Presentation" / "Resources" / "Raw" / "quotes" / "quotes.ru.json"
EN_PATH = ROOT / "PsychologyApp.Presentation" / "Resources" / "Raw" / "quotes" / "quotes.en.json"
TARGET = 525

THEMES = [
    "wisdom",
    "motivation",
    "resilience",
    "mindfulness",
    "self-awareness",
    "anxiety",
    "calm",
    "gratitude",
    "healing",
    "self-love",
    "acceptance",
    "habits",
    "hope",
    "empathy",
    "relationships",
    "general",
]

RU_TEMPLATES = [
    "Маленький шаг {n} сегодня уже меняет завтра.",
    "Замечать {n} — начало заботы о себе.",
    "Спокойствие растёт там, где есть принятие {n}.",
    "Ты имеешь право двигаться в своём темпе, даже когда {n}.",
    "Дыхание может стать якорем, когда {n} кажется слишком большим.",
    "Сострадание к себе — не слабость, а опора при {n}.",
    "Один честный взгляд на {n} снимает часть напряжения.",
    "Не нужно решать всё сразу: достаточно одного шага про {n}.",
    "Тело часто знает ответ раньше мыслей — прислушайся к {n}.",
    "Тревога уменьшается, когда {n} делится на маленькие части.",
    "Надежда начинается с признания: сейчас мне трудно из‑за {n}.",
    "Забота о себе — это практика, а не разовый подвиг про {n}.",
    "Ты не обязан(а) быть сильным(ой) каждый день, особенно когда {n}.",
    "Принятие {n} не отменяет желание перемен — оно даёт опору для них.",
    "Мягкость к себе сегодня — ресурс для завтра, даже если {n}.",
    "Иногда лучшая помощь — пауза и честность про {n}.",
    "Ты уже проходил(а) через трудное — {n} тоже можно прожить.",
    "Смысл не в идеальности, а в возвращении к себе после {n}.",
    "Поддержка начинается с фразы: мне важно, что я чувствую про {n}.",
    "Каждый новый день — шанс по‑новому отнестись к {n}.",
    "Сравнение с другими лишь усиливает боль, когда {n}.",
    "Твоё переживание про {n} имеет право на место.",
    "Можно одновременно хотеть лучшего и принимать {n} сейчас.",
    "Замедление — навык, особенно полезный при {n}.",
    "Мысль — не приказ; можно заметить {n} и выбрать ответ.",
    "Забота о границах защищает энергию, когда {n} давит.",
    "Ты не один(одна) со своим {n} — помощь допустима.",
    "Маленькая благодарность за {n} может сменить настроение дня.",
    "Привычка замечать {n} без осуждения укрепляет устойчивость.",
    "Сегодня достаточно сделать чуть меньше, если {n} истощает.",
    "Путь к ясности часто проходит через честность про {n}.",
    "Тишина иногда лечит лучше советов, когда {n}.",
]

EN_TEMPLATES = [
    "A small step {n} today already changes tomorrow.",
    "Noticing {n} is the beginning of self-care.",
    "Calm grows where there is acceptance of {n}.",
    "You are allowed to move at your own pace, even when {n}.",
    "Breath can be an anchor when {n} feels too big.",
    "Self-compassion is not weakness but support when {n}.",
    "One honest look at {n} releases some tension.",
    "You do not have to solve everything at once—one step about {n} is enough.",
    "The body often knows the answer before thoughts—listen to {n}.",
    "Anxiety eases when {n} is broken into small parts.",
    "Hope begins by admitting: it is hard for me because of {n}.",
    "Self-care is a practice, not a one-time feat about {n}.",
    "You do not have to be strong every day, especially when {n}.",
    "Accepting {n} does not cancel change—it supports it.",
    "Gentleness with yourself today is fuel for tomorrow, even if {n}.",
    "Sometimes the best help is a pause and honesty about {n}.",
    "You have gotten through hard things before—{n} can be lived through too.",
    "The point is not perfection but returning to yourself after {n}.",
    "Support starts with: what I feel about {n} matters.",
    "Each new day is a chance to relate differently to {n}.",
    "Comparison with others only adds pain when {n}.",
    "Your experience of {n} deserves space.",
    "You can want better and still accept {n} now.",
    "Slowing down is a skill, especially useful with {n}.",
    "A thought is not a command—you can notice {n} and choose a response.",
    "Boundaries protect energy when {n} feels heavy.",
    "You are not alone with your {n}—asking for help is allowed.",
    "A small gratitude for {n} can shift the day.",
    "The habit of noticing {n} without judgment builds resilience.",
    "Today it is enough to do a little less if {n} drains you.",
    "Clarity often passes through honesty about {n}.",
    "Silence sometimes heals better than advice when {n}.",
]

RU_NOUNS = [
    "усталость", "тревога", "неопределённость", "одиночество", "разочарование",
    "сомнение", "боль", "злость", "стыд", "вина", "перегруз", "бессонница",
    "нежность к себе", "границы", "прошлое", "будущее", "ожидания", "критика",
    "неудача", "изменения", "пауза", "выбор", "отдых", "поддержка", "принятие",
    "надежда", "смысл", "ценность", "доверие", "близость", "смелость", "терпение",
]

EN_NOUNS = [
    "fatigue", "anxiety", "uncertainty", "loneliness", "disappointment",
    "doubt", "pain", "anger", "shame", "guilt", "overload", "insomnia",
    "self-kindness", "boundaries", "the past", "the future", "expectations", "criticism",
    "failure", "change", "a pause", "a choice", "rest", "support", "acceptance",
    "hope", "meaning", "worth", "trust", "closeness", "courage", "patience",
]

AUTHORS_RU = [
    "Практика осознанности",
    "Совет психолога",
    "Стоическая мудрость",
    "Заметка о заботе",
    "Поддержка на каждый день",
]

AUTHORS_EN = [
    "Mindfulness practice",
    "Counselor's note",
    "Stoic wisdom",
    "Care reminder",
    "Daily support",
]


def load_json(path: Path) -> list[dict]:
    with path.open(encoding="utf-8") as f:
        return json.load(f)


def generate_pairs() -> tuple[list[dict], list[dict]]:
    ru_items: list[dict] = []
    en_items: list[dict] = []
    idx = 0
    for theme in THEMES:
        for t_idx, (ru_tpl, en_tpl) in enumerate(zip(RU_TEMPLATES, EN_TEMPLATES)):
            noun_ru = RU_NOUNS[(idx + t_idx) % len(RU_NOUNS)]
            noun_en = EN_NOUNS[(idx + t_idx) % len(EN_NOUNS)]
            author_ru = AUTHORS_RU[(idx + t_idx) % len(AUTHORS_RU)]
            author_en = AUTHORS_EN[(idx + t_idx) % len(AUTHORS_EN)]
            ru_items.append({"author": author_ru, "text": ru_tpl.format(n=noun_ru), "theme": theme})
            en_items.append({"author": author_en, "text": en_tpl.format(n=noun_en), "theme": theme})
            idx += 1
    return ru_items, en_items


def main() -> None:
    existing_ru = load_json(RU_PATH)
    existing_en = load_json(EN_PATH)
    generated_ru, generated_en = generate_pairs()

    needed = TARGET - len(existing_ru)
    seen = {item["text"] for item in existing_ru}
    new_ru: list[dict] = []
    new_en: list[dict] = []

    for ru, en in zip(generated_ru, generated_en):
        if ru["text"] in seen:
            continue
        seen.add(ru["text"])
        new_ru.append(ru)
        new_en.append(en)
        if len(new_ru) >= needed:
            break

    if len(new_ru) < needed:
        raise SystemExit(f"Generated only {len(new_ru)} new quotes; need {needed}")

    merged_ru = existing_ru + new_ru
    merged_en = existing_en + new_en

    if len(merged_ru) != TARGET or len(merged_en) != TARGET:
        raise SystemExit(f"Expected {TARGET} quotes, got RU={len(merged_ru)} EN={len(merged_en)}")
    if len({x["text"] for x in merged_ru}) != TARGET:
        raise SystemExit("Duplicate RU texts detected")
    if len({x["text"] for x in merged_en}) != TARGET:
        raise SystemExit("Duplicate EN texts detected")

    RU_PATH.write_text(json.dumps(merged_ru, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    EN_PATH.write_text(json.dumps(merged_en, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(f"Wrote {TARGET} quotes to {RU_PATH.name} and {EN_PATH.name}")


if __name__ == "__main__":
    main()
