using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;
using Tesseract;

public class OcrService
{
    private readonly string _tessDataPath;

    public OcrService(string tessDataPath)
    {
        _tessDataPath = tessDataPath;
    }

    public string RecognizeText(Stream imageStream)
    {
        // Load và xử lý ảnh bằng ImageSharp
        using var image = Image.Load(imageStream);

        image.Mutate(x =>
        {
            x.Resize(new ResizeOptions
            {
                Size = new Size(300, 100),
                Mode = ResizeMode.Max
            });
            x.Grayscale();                 // chuyển trắng đen
            x.Contrast(1.2f);              // tăng tương phản
        });

        // Chuyển ảnh về stream để OCR
        using var ms = new MemoryStream();
        image.Save(ms, new PngEncoder());
        ms.Seek(0, SeekOrigin.Begin);

        // OCR bằng Tesseract
        using var engine = new TesseractEngine(_tessDataPath, "eng", EngineMode.Default);
        using var pix = Pix.LoadFromMemory(ms.ToArray());
        using var page = engine.Process(pix);

        return page.GetText().Trim();
    }
}
