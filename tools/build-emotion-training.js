// Rebuilds the training set that ships with the app (PsychologyApp.Application/Conversation/Companion/Data/emotion-training.json)
// from the labelled evaluation sets and the extra training messages in PsychologyApp.Application.Tests/Conversation/Data.
// Run after adding or changing messages there:  node tools/build-emotion-training.js
const fs = require("fs");
const dir = "PsychologyApp.Application.Tests/Conversation/Data/";
const files = ["nlu-dataset.json", "nlu-heldout.json", "nlu-fresh.json", "nlu-fresh2.json", "nlu-fresh3.json", "nlu-fresh4.json", "nlu-train-extra.json"];
const seen = new Set();
const out = [];
for (const f of files) {
  for (const item of JSON.parse(fs.readFileSync(dir + f, "utf8"))) {
    const key = item.label + "|" + item.text.toLowerCase();
    if (seen.has(key)) continue;
    seen.add(key);
    out.push({ label: item.label, text: item.text });
  }
}
fs.writeFileSync("PsychologyApp.Application/Conversation/Companion/Data/emotion-training.json", JSON.stringify(out, null, 1) + "\n");
console.log(out.length + " messages");
