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
    ru: { eyebrow: 'ИИ-чат', title: 'Собеседник, который *слышит*', sub: 'Помогает назвать чувство и предлагает практику', badge: 'Без интернета · всё на телефоне' },
    en: { eyebrow: 'AI chat', title: 'A chat that *listens*', sub: 'Helps you name the feeling and suggests a practice', badge: 'Offline · stays on your phone' },
  },
  {
    raw: 'breathing.png', out: '02-breathing.png',
    ru: { eyebrow: 'Дыхание', title: 'Дышите вместе *с кругом*', sub: 'Квадратное дыхание на весь экран, без лишнего' },
    en: { eyebrow: 'Breathing', title: 'Breathe *with the circle*', sub: 'Full-screen box breathing, nothing else on screen' },
  },
  {
    raw: 'practices.png', out: '03-practices.png',
    ru: { eyebrow: 'Практики', title: 'Пара минут — *и легче*', sub: 'Дыхание, заземление, якорь ресурса — по 3–4 минуты' },
    en: { eyebrow: 'Practices', title: 'A few minutes, *some relief*', sub: 'Breathing, grounding, a resource anchor — 3–4 minutes each' },
  },
  {
    raw: 'insights.png', out: '04-what-helps.png',
    ru: { eyebrow: 'Что мне помогает', title: 'Видно, что помогает *именно вам*', sub: 'Выводы только из ваших собственных отметок' },
    en: { eyebrow: 'What helps me', title: 'See what helps *you*', sub: 'Drawn only from your own entries' },
  },
  {
    raw: 'journal.png', out: '05-journal.png',
    ru: { eyebrow: 'Дневник', title: 'Настроение — *в одно касание*', sub: 'Утром и вечером, с заметкой, если хочется' },
    en: { eyebrow: 'Journal', title: 'Your mood *in one tap*', sub: 'Morning and evening, with a note if you like' },
  },
  {
    raw: 'chart.png', out: '06-mood-trend.png',
    ru: { eyebrow: 'Динамика', title: 'Как меняется *настроение*', sub: 'График за месяц с отметками практик' },
    en: { eyebrow: 'Trend', title: 'Watch your mood *change*', sub: 'A month at a glance, with your practices marked' },
  },
  {
    raw: 'widget.png', out: '07-widget.png', widget: true,
    ru: { eyebrow: 'Виджет', title: 'Отметка *прямо с экрана*', sub: 'Настроение и практика дня — без открытия приложения' },
    en: { eyebrow: 'Widget', title: 'Check in *from your home screen*', sub: 'Mood and the practice of the day, one tap away' },
  },
  {
    raw: 'tests.png', out: '08-tests.png',
    ru: { eyebrow: 'Тесты', title: 'Тесты с *понятным итогом*', sub: 'PSS-10, шкала Розенберга, опросник Бека — ориентир, не диагноз' },
    en: { eyebrow: 'Tests', title: 'Tests with *clear results*', sub: 'PSS-10, Rosenberg, Beck — a guide, not a diagnosis' },
  },
];

const FEATURE = {
  ru: { title: 'Спокойствие *рядом*', sub: 'Практики, ИИ-чат и дневник настроения. Всё — на вашем телефоне.' },
  en: { title: 'Calm *within reach*', sub: 'Practices, an AI chat and a mood journal. All on your phone.' },
};

const esc = (s) => s.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
// A short word (a preposition, an article) stays with the next word, and a dash with the previous one, instead of a line
// ending or starting on them.
const glue = (s) => s
  .replace(/ —/g, '\u00A0—')
  .replace(/(^|[\s*])(с|в|и|к|о|у|а|на|не|по|за|из|от|до|без|для|a|an|the|of|to|in|on|with|from|your)\s/giu, '$1$2\u00A0');
const accent = (s) => esc(glue(s)).replace(/\*(.+?)\*/g, '<span class="accent">$1</span>');
const url = (p) => pathToFileURL(resolve(p)).href;

// One wide background for all slides: soft glows that drift from slide to slide.
function backdrop(index, count) {
  const glows = [
    ['#2D7FF9', 0.55], ['#22B8A7', 0.38], ['#7B6CF6', 0.45], ['#F2A07B', 0.22], ['#2D7FF9', 0.4], ['#22B8A7', 0.35],
  ];
  const spots = [];
  for (let i = 0; i < count * 2 + 2; i++) {
    const [color, alpha] = glows[i % glows.length];
    const x = i * (W / 2) - W * 0.25 + (i % 3) * 140;
    const y = [260, 1500, 760, 2150, 420, 1850][i % 6];
    const r = [900, 1100, 800, 1000, 950, 850][i % 6];
    spots.push(`<div class="glow" style="left:${x - r / 2}px;top:${y - r / 2}px;width:${r}px;height:${r}px;background:${color};opacity:${alpha}"></div>`);
  }
  return `<div class="backdrop" style="width:${W * count}px;transform:translateX(${-index * W}px)">${spots.join('')}</div>`;
}

const STYLE = `
@font-face { font-family: Inter; font-weight: 400; src: url('${url(join(fonts, 'Inter-Regular.ttf'))}'); }
@font-face { font-family: Inter; font-weight: 500; src: url('${url(join(fonts, 'Inter-Medium.ttf'))}'); }
@font-face { font-family: Inter; font-weight: 600; src: url('${url(join(fonts, 'Inter-SemiBold.ttf'))}'); }
* { box-sizing: border-box; margin: 0; padding: 0; }
html, body { width: ${W}px; height: ${H}px; overflow: hidden; }
body { font-family: Inter, 'Segoe UI', sans-serif; color: #fff; position: relative;
  background: linear-gradient(170deg, #0B1633 0%, #0F2457 48%, #0A1A3D 100%); -webkit-font-smoothing: antialiased; }
.backdrop { position: absolute; top: 0; left: 0; height: 100%; }
.glow { position: absolute; border-radius: 50%; filter: blur(160px); }
.grain { position: absolute; inset: 0; opacity: .07; mix-blend-mode: overlay;
  background-image: url("data:image/svg+xml;utf8,<svg xmlns='http://www.w3.org/2000/svg' width='240' height='240'><filter id='n'><feTurbulence type='fractalNoise' baseFrequency='.9' numOctaves='2' stitchTiles='stitch'/></filter><rect width='100%' height='100%' filter='url(%23n)'/></svg>"); }
.copy { position: absolute; left: 110px; right: 110px; top: 128px; text-align: center; }
.eyebrow { display: inline-block; font-weight: 600; font-size: 30px; letter-spacing: .14em; text-transform: uppercase; color: #A9CCFF;
  padding: 14px 28px; border-radius: 999px; background: rgba(255,255,255,.07); border: 1.5px solid rgba(255,255,255,.14); }
h1, .sub { text-wrap: balance; }
h1 { margin-top: 40px; font-weight: 600; font-size: 104px; line-height: 1.07; letter-spacing: -.028em; }
.accent { background: linear-gradient(100deg, #7FC8FF 0%, #A79BFF 60%, #F5B3D6 100%); -webkit-background-clip: text; background-clip: text; color: transparent; }
.sub { margin-top: 30px; font-size: 44px; line-height: 1.32; color: rgba(255,255,255,.72); letter-spacing: -.005em; }
.phone { position: absolute; left: 50%; transform: translateX(-50%); width: var(--pw); border-radius: 128px; padding: 22px;
  background: linear-gradient(150deg, #3A3F4B 0%, #14171D 38%, #0B0D11 70%, #2A2E37 100%);
  box-shadow: 0 90px 160px rgba(2,6,20,.55), 0 30px 60px rgba(2,6,20,.35), inset 0 0 0 2px rgba(255,255,255,.10), inset 0 2px 1px rgba(255,255,255,.18); }
.phone::after { content: ''; position: absolute; right: -7px; top: 420px; width: 7px; height: 170px; border-radius: 0 6px 6px 0; background: #22262E; box-shadow: 0 260px 0 #22262E; }
.screen { position: relative; border-radius: 106px; overflow: hidden; width: 100%; aspect-ratio: ${RAW_W} / 2400; background: #000; }
.screen img { position: absolute; left: 0; top: 0; width: 100%; display: block; }
.statusbar { position: absolute; left: 0; right: 0; top: 0; height: var(--sb); display: flex; align-items: center; justify-content: space-between;
  padding: 0 64px; font-weight: 600; font-size: 32px; letter-spacing: -.01em; }
.statusbar .icons { display: flex; gap: 14px; align-items: center; }
.camera { position: absolute; top: calc(var(--sb) / 2 - 15px); left: 50%; width: 30px; height: 30px; margin-left: -15px; border-radius: 50%;
  background: radial-gradient(circle at 40% 35%, #2C3340 0%, #050608 60%); }
.badge { position: absolute; display: flex; align-items: center; gap: 16px; padding: 22px 34px; border-radius: 999px; font-weight: 600; font-size: 34px;
  white-space: nowrap; color: #0E2A57; background: rgba(255,255,255,.93); box-shadow: 0 30px 60px rgba(2,6,20,.35); }
.zoom { position: absolute; border-radius: 52px; overflow: hidden; box-shadow: 0 50px 110px rgba(2,6,20,.55); outline: 2px solid rgba(255,255,255,.5); }
.zoom img { position: absolute; }
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
  const screenW = pw - 44;
  const scale = screenW / RAW_W;
  const sb = Math.round(RAW_STATUS * scale);
  const phoneTop = 760;
  const raw = url(join(rawDir, slide.raw));
  let extras = '';
  if (t.badge) {
    // Between the caption and the phone, so it covers nothing on the screen.
    extras += `<div class="badge" style="left:50%;transform:translateX(-50%);top:${phoneTop - 150}px">
      <svg width="34" height="38" viewBox="0 0 34 38"><path d="M17 1L3 7v11c0 9 6 16.5 14 19 8-2.5 14-10 14-19V7z" fill="#0062BD"/><path d="M10.5 19l4.5 4.5 8.5-9" stroke="#fff" stroke-width="3.6" fill="none" stroke-linecap="round" stroke-linejoin="round"/></svg>${esc(t.badge)}</div>`;
  }
  if (slide.widget) {
    // The widget card of the home screen, lifted out of the phone: enlarged around its own centre, so it covers the original.
    const crop = { x: 67, y: 422, w: 946, h: 613 };
    const zw = 1060;
    const zs = zw / crop.w;
    const centreX = W / 2 - screenW / 2 + (crop.x + crop.w / 2) * scale;
    const centreY = phoneTop + 22 + (crop.y + crop.h / 2) * scale;
    extras += `<div class="zoom" style="left:${Math.round(centreX - zw / 2)}px;top:${Math.round(centreY - crop.h * zs / 2)}px;width:${zw}px;height:${Math.round(crop.h * zs)}px">
      <img src="${raw}" style="width:${RAW_W * zs}px;left:${-crop.x * zs}px;top:${-crop.y * zs}px"></div>`;
  }
  return `<!doctype html><html><head><meta charset="utf-8"><style>${STYLE}</style></head><body>
${backdrop(index, count)}<div class="grain"></div>
<div class="copy"><div class="eyebrow">${esc(t.eyebrow)}</div><h1>${accent(t.title)}</h1><div class="sub">${esc(glue(t.sub))}</div></div>
<div class="phone" style="--pw:${pw}px;--sb:${sb}px;top:${phoneTop}px">
  <div class="screen"><img src="${raw}">
    <div class="statusbar"><span>9:41</span><span class="icons">${ICONS('#INK')}</span></div><div class="camera"></div>
  </div>
</div>
${extras}
<script>${SCRIPT}</script></body></html>`;
}

function featureHtml() {
  const t = FEATURE[lang];
  const shots = ['chat.png', 'insights.png'].map((f) => url(join(rawDir, f)));
  const phone = (src, left, top, rot) => `<div class="phone" style="--pw:300px;--sb:0px;left:${left}px;top:${top}px;transform:rotate(${rot}deg);border-radius:46px;padding:9px">
    <div class="screen" style="border-radius:38px"><img src="${src}" style="top:${-RAW_STATUS * (282 / RAW_W)}px"></div></div>`;
  return `<!doctype html><html><head><meta charset="utf-8"><style>${STYLE}
html, body { width: 1024px; height: 500px; }
.copy { left: 64px; right: auto; width: 520px; top: 128px; text-align: left; }
h1 { margin-top: 0; font-size: 70px; }
.sub { font-size: 26px; margin-top: 22px; }
.phone::after { display: none; }
</style></head><body>
<div class="backdrop" style="width:1024px">
  <div class="glow" style="left:-200px;top:-260px;width:620px;height:620px;background:#2D7FF9;opacity:.55;filter:blur(110px)"></div>
  <div class="glow" style="left:560px;top:120px;width:560px;height:560px;background:#7B6CF6;opacity:.45;filter:blur(110px)"></div>
  <div class="glow" style="left:300px;top:300px;width:420px;height:420px;background:#22B8A7;opacity:.3;filter:blur(110px)"></div>
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
