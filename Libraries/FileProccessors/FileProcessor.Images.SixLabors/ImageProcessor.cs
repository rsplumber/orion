using System.Diagnostics;
using FileProcessor.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Pbm;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace FileProcessor.Images.SixLabors;

internal sealed class ImageProcessor : IFileProcessor
{
    private static readonly JpegEncoder JpegEncoder = new();
    private static readonly PngEncoder PngEncoder = new();
    private static readonly TiffEncoder TiffEncoder = new();
    private static readonly WebpEncoder WebpEncoder = new();
    private static readonly BmpEncoder BmpEncoder = new();
    private static readonly TgaEncoder TgaEncoder = new();
    private static readonly PbmEncoder PbmEncoder = new();
    private const string TempFilesPath = "./ProcessorTempFiles";

    private static readonly List<string> SupportedImages = new()
    {
        "jpg", "jpeg", "jpe", "jif", "jfif", "jfi",
        "png", "tiff", "tif", "webp", "bmp", "dib",
        "tga", "icb", "vda", "vst", "pbm", "pgm", "ppm", "pnm"
    };

    public string Type => "images";

    public string Name => "six_labors";

    public IEnumerable<string> SupportedTypes => SupportedImages;

    public async Task<ProcessedResponse> ProcessAsync(Stream file, Dictionary<string, string> configs = default!, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        if (configs is null) throw new ArgumentNullException(nameof(configs), "Enter valid configs");
        var img = await Image.LoadAsync(file, cancellationToken);
        var finalExtension = SanitizeExtension();
        var name = $"{Guid.NewGuid()}.{finalExtension}";
        if (!Directory.Exists(TempFilesPath))
        {
            Directory.CreateDirectory(TempFilesPath);
        }

        var tempFilePath = $"{TempFilesPath}/{name}";

        img.Mutate(x =>
        {
            if (TryExtractWidthHeight(configs, out var resizeConfig))
            {
                x.Resize(resizeConfig.Width, resizeConfig.Height, KnownResamplers.Lanczos3);
            }

            if (TryExtractImageConfig(configs, out var imageConfig))
            {
                if (imageConfig.Contrast is not null) x.Contrast(imageConfig.Contrast.Value);
                if (imageConfig.Brightness is not null) x.Brightness(imageConfig.Brightness.Value);
                if (imageConfig.Hue is not null) x.Hue(imageConfig.Hue.Value);
            }
        });
        await img.SaveAsync(tempFilePath, EncoderResolver(finalExtension), cancellationToken);
        var fileStream = File.OpenRead(tempFilePath);
        File.Delete(tempFilePath);
        stopwatch.Stop();
        return new ProcessedResponse
        {
            Content = fileStream,
            Name = name,
            ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
        };

        string SanitizeExtension()
        {
            if (!configs.TryGetValue("final_extension", out var requestedExtension))
            {
                throw new InvalidProcessConfig($"Invalid final_extensions {requestedExtension}");
            }

            return requestedExtension.StartsWith(".") ? string.Join("", requestedExtension[1..]) : requestedExtension;
        }
    }

    private static bool TryExtractWidthHeight(IReadOnlyDictionary<string, string> configs, out ResizeConfig resizeConfig)
    {
        if (!configs.TryGetValue("resize_width", out var resizeWidth) ||
            !configs.TryGetValue("resize_height", out var resizeHeight))
        {
            resizeConfig = ResizeConfig.Empty;
            return false;
        }

        var width = int.Parse(resizeWidth ?? throw new InvalidProcessConfig($"invalid resize_width: {resizeWidth}"));
        var height = int.Parse(resizeHeight ?? throw new InvalidProcessConfig($"invalid resize_height: {resizeHeight}"));
        resizeConfig = new ResizeConfig(width, height);
        return true;
    }

    private static bool TryExtractImageConfig(IReadOnlyDictionary<string, string> configs, out ImageConfig imageConfig)
    {
        configs.TryGetValue("hue", out var hueValue);
        configs.TryGetValue("brightness", out var brightnessValue);
        configs.TryGetValue("contrast", out var contrastValue);
        if (hueValue is null && brightnessValue is null && contrastValue is null)
        {
            imageConfig = ImageConfig.Empty;
            return false;
        }

        imageConfig = new ImageConfig();

        if (float.TryParse(hueValue, out var hueFloat))
        {
            imageConfig.Hue = hueFloat;
        }

        if (float.TryParse(brightnessValue, out var brightnessFloat))
        {
            imageConfig.Brightness = brightnessFloat;
        }

        if (float.TryParse(contrastValue, out var contrastFloat))
        {
            imageConfig.Contrast = contrastFloat;
        }

        return true;
    }

    private static ImageEncoder EncoderResolver(string finalFormat) => finalFormat switch
    {
        "jpg" or "jpeg" or "jpe" or "jif" or "jfif" or "jfi" => JpegEncoder,
        "png" => PngEncoder,
        "tiff" or "tif" => TiffEncoder,
        "webp" => WebpEncoder,
        "bmp" or "dib" => BmpEncoder,
        "tga" or "icb" or "vda" or "vst" => TgaEncoder,
        "pbm" or "pgm" or "ppm" or "pnm" => PbmEncoder,
        _ => throw new ArgumentOutOfRangeException(nameof(finalFormat), finalFormat, $"{finalFormat} not supported!")
    };


    private sealed record ResizeConfig(int Width, int Height)
    {
        public static readonly ResizeConfig Empty = new(0, 0);
    }

    private sealed record ImageConfig
    {
        public static readonly ImageConfig Empty = new();

        public float? Brightness { get; set; }

        public float? Contrast { get; set; }

        public float? Hue { get; set; }
    }
}