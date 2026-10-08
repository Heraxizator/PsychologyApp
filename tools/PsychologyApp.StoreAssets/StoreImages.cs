using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace PsychologyApp.StoreAssets;

/// <summary>
/// Turns raw phone screenshots into store images: the system bars are cut off, the screen sits in a rounded phone frame on a calm gradient,
/// and a short honest caption stands above it. Output is 1440x2560 (9:16), inside the limits of Google Play (2:1 at most) and RuStore.
/// </summary>
public static class StoreImages
{
    private const int CanvasWidth = 1440;
    private const int CanvasHeight = 2560;
    private const int StatusBar = 76;
    private const int NavigationBar = 2205;
    private const int ScreenWidth = 1020;
    private const int ScreenTop = 560;

    public sealed record Shot(string RawFile, string OutFile, string Title, string Subtitle, Color Top, Color Bottom, int CropTop = StatusBar);

    public static readonly Shot[] Shots =
    [
        // The first eight go to Google Play (at most eight); RuStore takes all ten.
        new("04-chat-offer.png", "01-chat.png", "Собеседник, который слышит", "Подскажет практику, когда она нужна", Color.FromArgb(0x00, 0x72, 0xDB), Color.FromArgb(0x4F, 0xA3, 0xF0)),
        new("03-breathing.png", "02-breathing.png", "Дышите вместе с кругом", "Спокойный ритм и счёт вдохов", Color.FromArgb(0x1F, 0x7A, 0x6B), Color.FromArgb(0x5C, 0xB8, 0xA5), 180),
        new("02-practices.png", "03-practices.png", "Практики на каждый день", "Тело, мысли и чувства — по разделам", Color.FromArgb(0x00, 0x72, 0xDB), Color.FromArgb(0x4F, 0xA3, 0xF0)),
        new("10b-month.png", "04-mood-calendar.png", "Настроение в цвете", "Дневник по дням, неделям и месяцам", Color.FromArgb(0x2F, 0x8F, 0x6F), Color.FromArgb(0x7B, 0xCB, 0xA8)),
        new("10d-chart.png", "05-mood-chart.png", "Видно, что помогает", "Отметки практик на графике настроения", Color.FromArgb(0x00, 0x5F, 0xB8), Color.FromArgb(0x3E, 0x92, 0xE0), 650),
        new("06-body-new.png", "06-body.png", "Связь тела и эмоций", "Что могло стоять за ощущением в теле", Color.FromArgb(0x1B, 0x6F, 0xA8), Color.FromArgb(0x5F, 0xB0, 0xD8)),
        new("08-prayers.png", "07-prayers.png", "Молитвы под рукой", "Утренние, покаянные и основные", Color.FromArgb(0x3F, 0x4F, 0xA8), Color.FromArgb(0x84, 0x92, 0xD8)),
        new("09b-quotes-wisdom.png", "08-quotes.png", "Цитаты на каждый день", "Мудрость и мотивация, избранное и поиск", Color.FromArgb(0xA8, 0x62, 0x1B), Color.FromArgb(0xDC, 0x98, 0x4E)),
        new("07-profile.png", "09-profile.png", "Ваш путь — на одном экране", "Практики, серия дней, недельный обзор", Color.FromArgb(0x0B, 0x5F, 0xA8), Color.FromArgb(0x5A, 0xA6, 0xE6)),
        new("05-tests.png", "10-tests.png", "Тесты с понятным итогом", "Короткие опросники и рекомендации", Color.FromArgb(0x3B, 0x6E, 0xC4), Color.FromArgb(0x86, 0xA8, 0xE6))
    ];

    public static void Compose(string rawDir, string outDir)
    {
        Directory.CreateDirectory(outDir);
        foreach (Shot shot in Shots)
        {
            string raw = Path.Combine(rawDir, shot.RawFile);
            if (!File.Exists(raw))
            {
                Console.WriteLine($"skip {shot.RawFile}: not found");
                continue;
            }

            using Bitmap source = new(raw);
            using Bitmap image = Render(source, shot);
            string path = Path.Combine(outDir, shot.OutFile);
            image.Save(path, ImageFormat.Png);
            Console.WriteLine($"{path} ({image.Width}x{image.Height})");
        }
    }

    private static Bitmap Render(Bitmap source, Shot shot)
    {
        Bitmap canvas = new(CanvasWidth, CanvasHeight, PixelFormat.Format24bppRgb);
        using Graphics g = Graphics.FromImage(canvas);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

        using (LinearGradientBrush back = new(new Rectangle(0, 0, CanvasWidth, CanvasHeight), shot.Top, shot.Bottom, 70f))
        {
            g.FillRectangle(back, 0, 0, CanvasWidth, CanvasHeight);
        }

        using (SolidBrush glow = new(Color.FromArgb(28, Color.White)))
        {
            g.FillEllipse(glow, -300, -380, 1100, 1100);
            g.FillEllipse(glow, 780, 1500, 1000, 1000);
        }

        DrawCaption(g, shot);

        int cropTop = shot.CropTop;
        Rectangle part = new(0, cropTop, source.Width, NavigationBar - cropTop);
        int screenHeight = (int)Math.Round(ScreenWidth * (double)part.Height / part.Width);
        int screenLeft = (CanvasWidth - ScreenWidth) / 2;

        // A short screen keeps the page colour below it, so the frame still runs off the bottom edge.
        int top = ScreenTop;
        Color page = source.GetPixel(6, NavigationBar - 30);
        int fillHeight = Math.Max(screenHeight, CanvasHeight - top);

        const int Bezel = 26;
        Rectangle frame = new(screenLeft - Bezel, top - Bezel, ScreenWidth + Bezel * 2, fillHeight + Bezel * 2 + 400);
        using (GraphicsPath shadowPath = Rounded(new Rectangle(frame.X, frame.Y + 24, frame.Width, frame.Height), 120))
        using (SolidBrush shadow = new(Color.FromArgb(60, 0, 0, 0)))
        {
            g.FillPath(shadow, shadowPath);
        }

        using (GraphicsPath framePath = Rounded(frame, 120))
        using (SolidBrush body = new(Color.FromArgb(0x16, 0x1A, 0x22)))
        {
            g.FillPath(body, framePath);
        }

        Rectangle screen = new(screenLeft, top, ScreenWidth, screenHeight);
        using (GraphicsPath clip = Rounded(new Rectangle(screen.X, screen.Y, screen.Width, fillHeight + 400), 96))
        {
            g.SetClip(clip);
            using (SolidBrush fill = new(page))
            {
                g.FillRectangle(fill, screenLeft, top, ScreenWidth, fillHeight + 400);
            }

            g.DrawImage(source, screen, part, GraphicsUnit.Pixel);
            g.ResetClip();
        }

        return canvas;
    }

    private static void DrawCaption(Graphics g, Shot shot)
    {
        float size = 92;
        Font title = new("Segoe UI Semibold", size, FontStyle.Regular, GraphicsUnit.Pixel);
        while (g.MeasureString(shot.Title, title).Width > CanvasWidth - 140 && size > 50)
        {
            title.Dispose();
            size -= 4;
            title = new Font("Segoe UI Semibold", size, FontStyle.Regular, GraphicsUnit.Pixel);
        }

        using Font subtitle = new("Segoe UI", 52, FontStyle.Regular, GraphicsUnit.Pixel);
        using SolidBrush white = new(Color.White);
        using SolidBrush soft = new(Color.FromArgb(225, 255, 255, 255));
        using StringFormat centre = new() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };

        g.DrawString(shot.Title, title, white, new RectangleF(60, 150, CanvasWidth - 120, 240), centre);
        g.DrawString(shot.Subtitle, subtitle, soft, new RectangleF(60, 330, CanvasWidth - 120, 160), centre);
        title.Dispose();
    }

    private static GraphicsPath Rounded(Rectangle r, int radius)
    {
        int d = radius * 2;
        GraphicsPath path = new();
        path.AddArc(r.X, r.Y, d, d, 180, 90);
        path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }
}
