// Store screenshots: raw phone captures -> 1440x2560 slides for Google Play and RuStore, plus the 1024x500 feature graphic.
//
//   node tools/store-screenshots/compose.mjs <rawDir> <outDir> <ru|en>
//
// <rawDir> holds the captures named as in SLIDES (1080x2400, a debug copy filled with tools/PsychologyApp.StoreAssets demo data).
// Each slide is an HTML page rendered by headless Microsoft Edge, so the type, gradients and shadows are the browser's. The
// phone's own status bar is covered by a clean one in the colour of the screen under it. The background is one wide picture
// cut into slides, so the slides read as one strip in the store. Captions promise only what the screens show.
import { execFileSync } from 'node:child_process';
import { mkdirSync, writeFileSync, existsSync, rmSync } from 'node:fs';
import { join, resolve } from 'node:path';
import { pathToFileURL, fileURLToPath } from 'node:url';

const [rawDir, outDir, lang] = process.argv.slice(2);
if (!rawDir || !outDir || !['ru', 'en'].includes(lang)) {
  console.error('Usage: node compose.mjs <rawDir> <outDir> <ru|en>');
  process.exit(1);
}

const EDGE = process.env.EDGE ?? 'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe';
const here = fileURLToPath(new URL('.', import.meta.url));
const fonts = resolve(here, '../../PsychologyApp.Presentation/Resources/Fonts');
const W = 1440;
const H = 2560;
const RAW_W = 1080;
const RAW_STATUS = 64; // the emulator's status bar, px of the raw capture

// *word* in a title is drawn in the accent gradient.
const SLIDES = [
  {
    raw: 'chat.png', out: '01-chat.png',
    ru: { eyebrow: 'ИИ-психолог', title: 'Собеседник, который *слышит*', sub: 'Помогает назвать чувство и предлагает практику', badge: 'Без интернета · всё на телефоне' },
    en: { eyebrow: 'AI psychologist', title: 'A chat that *listens*', sub: 'Helps you name the feeling and suggests a practice', badge: 'Offline · stays on your phone' },
  },
  {
    raw: 'breathing.png', out: '02-breathing.png',
    ru: { eyebrow: 'Дыхание', title: 'Дышите вместе *с кругом*', sub: 'Круг подсказывает, когда вдыхать, ждать и выдыхать' },
    en: { eyebrow: 'Breathing', title: 'Breathe *with the circle*', sub: 'The circle shows when to breathe in, hold and breathe out' },
  },
  {
    raw: 'practices.png', out: '03-practices.png',
    ru: { eyebrow: 'Практики', title: 'Пара минут — *и легче*', sub: 'Дыхание, заземление, якорь ресурса — по 3–4 минуты' },
    en: { eyebrow: 'Practices', title: 'A few minutes, *some relief*', sub: 'Breathing, grounding, a resource anchor — 3–4 minutes each' },
  },
  {
    raw: 'journal.png', out: '04-journal.png',
    ru: { eyebrow: 'Дневник', title: 'Настроение — *в одно касание*', sub: 'Утром и вечером, с заметкой, если хочется' },
    en: { eyebrow: 'Journal', title: 'Your mood *in one tap*', sub: 'Morning and evening, with a note if you like' },
  },
  {
    raw: 'chart.png', out: '05-month.png',
    ru: { eyebrow: 'Обзор', title: 'Месяц *как на ладони*', sub: 'Календарь настроения, средние и что на него влияло' },
    en: { eyebrow: 'Overview', title: 'Your month *at a glance*', sub: 'A mood calendar, averages and what affected your days' },
  },
  {
    raw: 'insights.png', out: '06-what-helps.png',
    ru: { eyebrow: 'Что мне помогает', title: 'Видно, что помогает *именно вам*', sub: 'Выводы только из ваших собственных отметок' },
    en: { eyebrow: 'What helps me', title: 'See what helps *you*', sub: 'Drawn only from your own entries' },
  },
  {
    raw: 'anchors.png', out: '07-anchors.png',
    ru: { eyebrow: 'Опорные мысли', title: 'Мысли, на которые *можно опереться*', sub: 'Короткие фразы для трудной минуты' },
    en: { eyebrow: 'Anchor thoughts', title: 'Thoughts *to lean on*', sub: 'Short lines for a hard moment' },
  },
  {
    raw: 'tests.png', out: '08-tests.png',
    ru: { eyebrow: 'Тесты', title: 'Тесты с *понятным итогом*', sub: 'PSS-10, шкала Розенберга, опросник Бека — ориентир, не диагноз' },
    en: { eyebrow: 'Tests', title: 'Tests with *clear results*', sub: 'PSS-10, Rosenberg, Beck — a guide, not a diagnosis' },
  },
];

const FEATURE = {
  ru: { title: 'Спокойствие *рядом*', sub: 'Практики, ИИ-психолог и дневник настроения. Всё — на вашем телефоне.' },
  en: { title: 'Calm *within reach*', sub: 'Practices, an AI psychologist and a mood journal. All on your phone.' },
};

const esc = (s) => s.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
// A short word (a preposition, an article) stays with the next word, and a dash with the previous one, instead of a line
// ending or starting on them.
const glue = (s) => s
  .replace(/ —/g, '\u00A0—')
  .replace(/(^|[\s*])(с|в|и|к|о|у|а|на|не|по|за|из|от|до|без|для|a|an|the|of|to|in|on|with|from|your)\s/giu, '$1$2\u00A0');
const accent = (s) => esc(glue(s)).replace(/\*(.+?)\*/g, '<span class="accent">$1</span>');
const url = (p) => pathToFileURL(resolve(p)).href;

// One wide background for all slides: pale pastel glows on ivory that drift from slide to slide.
function backdrop(index, count) {
  const glows = [
    ['#9CC4FF', 0.55], ['#A8E6D4', 0.55], ['#FFD2B8', 0.5], ['#C9C1FF', 0.5], ['#9CC4FF', 0.45], ['#A8E6D4', 0.45],
  ];
  const spots = [];
  for (let i = 0; i < count * 2 + 2; i++) {
    const [color, alpha] = glows[i % glows.length];
    const x = i * (W / 2) - W * 0.25 + (i % 3) * 140;
    const y = [300, 1600, 900, 2200, 480, 1900][i % 6];
    const r = [1000, 1150, 900, 1050, 1000, 900][i % 6];
    spots.push(`<div class="glow" style="left:${x - r / 2}px;top:${y - r / 2}px;width:${r}px;height:${r}px;background:${color};opacity:${alpha}"></div>`);
  }
  return `<div class="backdrop" style="width:${W * count}px;transform:translateX(${-index * W}px)">${spots.join('')}</div>`;
}

// The phone: a polished titanium band (light to dark bands, like a lit metal edge), a black bezel, and glass with one soft reflection.
const STYLE = `
@font-face { font-family: Inter; font-weight: 400; src: url('${url(join(fonts, 'Inter-Regular.ttf'))}'); }
@font-face { font-family: Inter; font-weight: 500; src: url('${url(join(fonts, 'Inter-Medium.ttf'))}'); }
@font-face { font-family: Inter; font-weight: 600; src: url('${url(join(fonts, 'Inter-SemiBold.ttf'))}'); }
* { box-sizing: border-box; margin: 0; padding: 0; }
html, body { width: ${W}px; height: ${H}px; overflow: hidden; }
body { font-family: Inter, 'Segoe UI', sans-serif; color: #0E1B33; position: relative;
  background: linear-gradient(172deg, #FBFAF7 0%, #F3F5FA 55%, #EEF2F8 100%); -webkit-font-smoothing: antialiased; }
.backdrop { position: absolute; top: 0; left: 0; height: 100%; }
.glow { position: absolute; border-radius: 50%; filter: blur(170px); }
.grain { position: absolute; inset: 0; opacity: .035; mix-blend-mode: multiply;
  background-image: url("data:image/svg+xml;utf8,<svg xmlns='http://www.w3.org/2000/svg' width='240' height='240'><filter id='n'><feTurbulence type='fractalNoise' baseFrequency='.9' numOctaves='2' stitchTiles='stitch'/></filter><rect width='100%' height='100%' filter='url(%23n)'/></svg>"); }
.copy { position: absolute; left: 110px; right: 110px; top: 128px; text-align: center; }
.eyebrow { display: inline-block; font-weight: 600; font-size: 30px; letter-spacing: .14em; text-transform: uppercase; color: #1F5FCC;
  padding: 14px 30px; border-radius: 999px; background: rgba(255,255,255,.75); border: 1.5px solid rgba(31,95,204,.16);
  box-shadow: 0 10px 30px rgba(40,70,130,.08); }
h1, .sub { text-wrap: balance; }
h1 { margin-top: 40px; font-weight: 600; font-size: 104px; line-height: 1.07; letter-spacing: -.03em; }
.accent { background: linear-gradient(100deg, #1F6FEB 0%, #3B82F6 40%, #12A38F 100%); -webkit-background-clip: text; background-clip: text; color: transparent; }
.sub { margin-top: 30px; font-size: 44px; line-height: 1.32; color: #4A566B; letter-spacing: -.005em; }
.phone { position: absolute; left: 50%; transform: translateX(-50%); width: var(--pw); border-radius: 132px; padding: 10px;
  background: linear-gradient(120deg, #FDFDFE 0%, #C7CDD6 14%, #8A93A1 27%, #EEF1F5 44%, #B4BBC6 60%, #F7F8FA 78%, #9AA2AE 92%, #E4E8ED 100%);
  box-shadow: 0 120px 180px -40px rgba(28,44,82,.38), 0 50px 80px -20px rgba(28,44,82,.22), 0 0 0 1.5px rgba(70,80,96,.35),
    inset 0 0 0 1.5px rgba(255,255,255,.9), inset 0 -3px 4px rgba(60,70,85,.25); }
.bezel { border-radius: 122px; padding: 16px; background: #07080A; box-shadow: inset 0 0 0 2px #1A1D22; }
.phone::before, .phone::after { content: ''; position: absolute; right: -8px; width: 8px; border-radius: 0 6px 6px 0;
  background: linear-gradient(90deg, #8F98A5, #E6E9EE 55%, #A3AAB5); }
.phone::before { top: 380px; height: 120px; }
.phone::after { top: 560px; height: 220px; }
.screen { position: relative; border-radius: 106px; overflow: hidden; width: 100%; aspect-ratio: ${RAW_W} / 2400; background: #000; }
.screen img { position: absolute; left: 0; top: 0; width: 100%; display: block; }
.screen::after { content: ''; position: absolute; inset: 0; pointer-events: none;
  background: linear-gradient(118deg, rgba(255,255,255,.20) 0%, rgba(255,255,255,.07) 30%, rgba(255,255,255,0) 31%, rgba(255,255,255,0) 100%); }
.statusbar { position: absolute; left: 0; right: 0; top: 0; height: var(--sb); display: flex; align-items: center; justify-content: space-between;
  padding: 0 64px; font-weight: 600; font-size: 32px; letter-spacing: -.01em; }
.statusbar .icons { display: flex; gap: 14px; align-items: center; }
.camera { position: absolute; top: calc(var(--sb) / 2 - 15px); left: 50%; width: 30px; height: 30px; margin-left: -15px; border-radius: 50%;
  background: radial-gradient(circle at 40% 35%, #2C3340 0%, #050608 60%); }
.badge { position: absolute; display: flex; align-items: center; gap: 16px; padding: 22px 34px; border-radius: 999px; font-weight: 600; font-size: 34px;
  white-space: nowrap; color: #0E2A57; background: rgba(255,255,255,.96); border: 1.5px solid rgba(31,95,204,.12);
  box-shadow: 0 30px 60px rgba(28,44,82,.16); }
`;

const ICONS = (c) => `
<svg width="34" height="30" viewBox="0 0 34 30"><path d="M2 28h5v-6H2zM10 28h5V16h-5zM18 28h5V9h-5zM26 28h5V2h-5z" fill="${c}"/></svg>
<svg width="38" height="30" viewBox="0 0 38 30"><path d="M19 28l5.5-6.6a8.6 8.6 0 00-11 0zM6.6 13.1l3.6 4.3a13.7 13.7 0 0117.6 0l3.6-4.3a19.3 19.3 0 00-24.8 0zM.5 5.8l3.6 4.3a23.5 23.5 0 0129.8 0l3.6-4.3a29.2 29.2 0 00-37 0z" fill="${c}"/></svg>
<svg width="60" height="30" viewBox="0 0 60 30"><rect x="1.5" y="4.5" width="50" height="21" rx="6" fill="none" stroke="${c}" stroke-opacity=".45" stroke-width="3"/><rect x="5" y="8" width="39" height="14" rx="3.5" fill="${c}"/><rect x="54" y="11" width="4" height="8" rx="2" fill="${c}" fill-opacity=".45"/></svg>`;

// The status bar takes the colour of the screen right under it, and dark or light text by its brightness.
const SCRIPT = `
const img = document.querySelector('.screen img');
const bar = document.querySelector('.statusbar');
function paint() {
  const c = document.createElement('canvas'); c.width = 1; c.height = 1;
  const g = c.getContext('2d');
  g.drawImage(img, 540, ${RAW_STATUS + 6}, 1, 1, 0, 0, 1, 1);
  const [r, gr, b] = g.getImageData(0, 0, 1, 1).data;
  bar.style.background = 'rgb(' + r + ',' + gr + ',' + b + ')';
  const dark = (0.299 * r + 0.587 * gr + 0.114 * b) < 140;
  const ink = dark ? '#FFFFFF' : '#14171D';
  bar.style.color = ink;
  bar.querySelector('.icons').innerHTML = bar.querySelector('.icons').innerHTML.replaceAll('#INK', ink);
}
img.complete ? paint() : img.addEventListener('load', paint);
`;

function slideHtml(slide, index, count) {
  const t = slide[lang];
  const pw = 940; // phone width with its frame
  const screenW = pw - 52;
  const scale = screenW / RAW_W;
  const sb = Math.round(RAW_STATUS * scale);
  const phoneTop = 760;
  const raw = url(join(rawDir, slide.raw));
  // Between the caption and the phone, so it covers nothing on the screen.
  const badge = t.badge
    ? `<div class="badge" style="left:50%;transform:translateX(-50%);top:${phoneTop - 150}px">
      <svg width="34" height="38" viewBox="0 0 34 38"><path d="M17 1L3 7v11c0 9 6 16.5 14 19 8-2.5 14-10 14-19V7z" fill="#0062BD"/><path d="M10.5 19l4.5 4.5 8.5-9" stroke="#fff" stroke-width="3.6" fill="none" stroke-linecap="round" stroke-linejoin="round"/></svg>${esc(t.badge)}</div>`
    : '';
  return `<!doctype html><html><head><meta charset="utf-8"><style>${STYLE}</style></head><body>
${backdrop(index, count)}<div class="grain"></div>
<div class="copy"><div class="eyebrow">${esc(t.eyebrow)}</div><h1>${accent(t.title)}</h1><div class="sub">${esc(glue(t.sub))}</div></div>
<div class="phone" style="--pw:${pw}px;--sb:${sb}px;top:${phoneTop}px"><div class="bezel">
  <div class="screen"><img src="${raw}">
    <div class="statusbar"><span>9:41</span><span class="icons">${ICONS('#INK')}</span></div><div class="camera"></div>
  </div>
</div></div>
${badge}
<script>${SCRIPT}</script></body></html>`;
}

function featureHtml() {
  const t = FEATURE[lang];
  const shots = ['chat.png', 'journal.png'].map((f) => url(join(rawDir, f)));
  const phone = (src, left, top, rot) => `<div class="phone" style="--pw:300px;--sb:0px;left:${left}px;top:${top}px;transform:rotate(${rot}deg);border-radius:48px;padding:4px">
    <div class="bezel" style="border-radius:44px;padding:6px"><div class="screen" style="border-radius:38px"><img src="${src}" style="top:${-RAW_STATUS * (280 / RAW_W)}px"></div></div></div>`;
  return `<!doctype html><html><head><meta charset="utf-8"><style>${STYLE}
html, body { width: 1024px; height: 500px; }
.copy { left: 64px; right: auto; width: 520px; top: 128px; text-align: left; }
h1 { margin-top: 0; font-size: 70px; }
.sub { font-size: 26px; margin-top: 22px; }
.phone::before, .phone::after { display: none; }
.phone { box-shadow: 0 40px 60px -16px rgba(28,44,82,.35), 0 0 0 1px rgba(70,80,96,.35), inset 0 0 0 1px rgba(255,255,255,.9); }
</style></head><body>
<div class="backdrop" style="width:1024px">
  <div class="glow" style="left:-200px;top:-260px;width:620px;height:620px;background:#9CC4FF;opacity:.6;filter:blur(110px)"></div>
  <div class="glow" style="left:560px;top:120px;width:560px;height:560px;background:#C9C1FF;opacity:.5;filter:blur(110px)"></div>
  <div class="glow" style="left:300px;top:300px;width:420px;height:420px;background:#A8E6D4;opacity:.55;filter:blur(110px)"></div>
</div><div class="grain"></div>
<div class="copy"><h1>${accent(t.title)}</h1><div class="sub">${esc(glue(t.sub))}</div></div>
${phone(shots[0], 600, 70, -6)}${phone(shots[1], 790, 40, 5)}
</body></html>`;
}

function render(html, out, width, height) {
  const tmp = join(resolve(outDir), `.slide-${process.pid}.html`);
  writeFileSync(tmp, html);
  execFileSync(EDGE, [
    '--headless=new', '--disable-gpu', '--hide-scrollbars', '--force-device-scale-factor=1', '--allow-file-access-from-files',
    '--default-background-color=00000000', `--window-size=${width},${height}`, '--virtual-time-budget=4000',
    `--screenshot=${resolve(out)}`, pathToFileURL(tmp).href,
  ], { stdio: 'ignore' });
  rmSync(tmp);
  console.log(`${out} (${width}x${height})`);
}

mkdirSync(outDir, { recursive: true });
SLIDES.forEach((slide, i) => {
  if (!existsSync(join(rawDir, slide.raw))) {
    console.log(`skip ${slide.raw}: not found`);
    return;
  }
  render(slideHtml(slide, i, SLIDES.length), join(outDir, slide.out), W, H);
});
render(featureHtml(), join(outDir, 'feature-graphic.png'), 1024, 500);
