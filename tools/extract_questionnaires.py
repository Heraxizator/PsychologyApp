"""Regenerate questionnaires.json from C# source (legacy). Prefer editing JSON directly."""

import json
import re
from pathlib import Path

root = Path(__file__).resolve().parents[1]
path = root / "PsychologyApp.Presentation/Modules/Tester/Collection"
source = path / "TestsListCatalog.cs"
if not source.exists():
    raise SystemExit(f"Source not found: {source}. Edit questionnaires.json manually.")

text = source.read_text(encoding="utf-8")
blocks = text.split("TestItem.CreateBuilder()")[1:]
tests = []

for block in blocks:
    def grab(pattern: str) -> str | None:
        match = re.search(pattern, block, re.DOTALL)
        return match.group(1) if match else None

    title = grab(r'\.SetTitle\("([^"]+)"\)')
    if not title:
        continue

    analyzer_ids = {
        "Опросник Хекка и Хесса": "heck_hess",
        "Тест Хаэра": "haer",
        "Опросник Л.Г. Почебут": "pochebut",
    }

    algo_match = re.search(r"\.SetAlgorithm\(\s*\[\s*(.*?)\s*\]\s*\)", block, re.DOTALL)
    algorithm = re.findall(r'"([^"]+)"', algo_match.group(1)) if algo_match else []

    answer_match = re.search(
        r'\}, \["([^"]+)", "([^"]+)"\], \[(\d+), (\d+)\],\s*\[\s*(.*?)\s*\],',
        block,
        re.DOTALL,
    )
    if not answer_match:
        continue

    questions = re.findall(r'"([^"]+)"', answer_match.group(5))

    tests.append(
        {
            "title": title,
            "subtitle": grab(r'\.SetSubtitle\("([^"]+)"\)'),
            "description": grab(r'\.SetDescription\("([^"]+)"\)'),
            "algorithm": algorithm,
            "comment": grab(r'\.SetComment\("([^"]+)"\)'),
            "analyzerId": analyzer_ids.get(title, title),
            "answers": [answer_match.group(1), answer_match.group(2)],
            "balls": [int(answer_match.group(3)), int(answer_match.group(4))],
            "questions": questions,
            "singleAnswer": True,
        }
    )

out = root / "PsychologyApp.Presentation/Resources/Raw/tests/questionnaires.json"
out.write_text(json.dumps(tests, ensure_ascii=False, indent=2), encoding="utf-8")
print(f"{len(tests)} tests -> {out}")
