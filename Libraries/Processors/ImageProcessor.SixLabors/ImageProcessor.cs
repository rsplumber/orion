using System.Diagnostics;
using Core;
using Core.Files;
using Core.Files.Exceptions;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Webp;
using File = System.IO.File;

namespace ImageProcessor.SixLabors;

internal sealed class ImageProcessor : IFileProcessor
{
    private static readonly JpegEncoder JpegEncoder = new();
    private static readonly PngEncoder PngEncoder = new();
    private static readonly TiffEncoder TiffEncoder = new();
    private static readonly WebpEncoder WebpEncoder = new();

    public async Task<ProcessedResponse> ProcessAsync(Stream file, Dictionary<string, string> configs = default!, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        if (configs is null) throw new ArgumentNullException(nameof(configs), "Enter valid configs");
        var img = await Image.LoadAsync(file, cancellationToken);
        var finalExtension = SanitizeExtension();
        var name = $"{Guid.NewGuid()}.{finalExtension}";
        var path = $"TempFiles/{name}";

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
        await img.SaveAsync(path, EncoderResolver(finalExtension), cancellationToken);
        var fileStream = File.OpenRead(path);
        File.Delete(path);
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
        "jpg" or "jpeg" => JpegEncoder,
        "png" => PngEncoder,
        "tiff" or "tif" => TiffEncoder,
        "webp" or "web" => WebpEncoder,
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