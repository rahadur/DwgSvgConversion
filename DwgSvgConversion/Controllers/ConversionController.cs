using Aspose.CAD;
using Aspose.CAD.CadExceptions;
using Aspose.CAD.FileFormats.Cad;
using Aspose.CAD.FileFormats.Cad.CadTables;
using Aspose.CAD.ImageOptions;
using Microsoft.AspNetCore.Mvc;
using SeekOrigin = System.IO.SeekOrigin;

// https://reference.aspose.com/tutorials/cad/net/dwg-file-manipulation/exploring-underlay-flags-of-dwg/

namespace DwgSvgConversion.Controllers;

[ApiController]
public class ConversionController : ControllerBase
{
    private readonly ILogger<ConversionController> _logger;

    public ConversionController(ILogger<ConversionController> logger)
    {
        _logger = logger;
    }

    [HttpPost("conversion")]
    public async Task<IActionResult> Conversion(IFormFile? file, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        try
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, cancellationToken: cancellationToken);
            memoryStream.Seek(0, SeekOrigin.Begin); // Reset stream position to the beginning

            using var cadImage = (CadImage)Image.Load(memoryStream);

            foreach (CadStyleTableObject style in cadImage.Styles)
            {
                style.PrimaryFontName = "Arial";
                style.BigFontName = "Arial";
            }

            var options = new CadRasterizationOptions
            {
                PageWidth = 500,
                PageHeight = 500,
                BackgroundColor = Color.FromArgb(ColorTranslator.FromHtml("#121226").ToArgb()),
                AutomaticLayoutsScaling = true,
                NoScaling = true,
                ScaleMethod = ScaleType.GrowToFit,
                Zoom = 1,
                DrawType = CadDrawTypeMode.UseDrawColor,
                DrawColor = Color.FromArgb(ColorTranslator.FromHtml("#717A9B").ToArgb()),
                VisibilityMode = VisibilityMode.AsScreen,
                Quality = new RasterizationQuality
                {
                    Text = RasterizationQualityValue.Low,
                    Hatch = RasterizationQualityValue.Low,
                    Arc = RasterizationQualityValue.Low,
                    Ole = RasterizationQualityValue.Low,
                    TextThicknessNormalization = true,
                    ObjectsPrecision = RasterizationQualityValue.Low
                }
                // Layouts = new [] { "Model" }
            };

            double sizeExtX = cadImage.MaxPoint.X - cadImage.MinPoint.X;
            double sizeExtY = cadImage.MaxPoint.Y - cadImage.MinPoint.Y;

            if (cadImage.UnitType == UnitType.Inch)
            {
                options.PageHeight = CommonHelper.INtoPixels(sizeExtY, CommonHelper.DPI);
                options.PageWidth = CommonHelper.INtoPixels(sizeExtX, CommonHelper.DPI);
                _logger.LogInformation("UnitType: {0}, Width: {1}, Height: {3}", "Inch", options.PageWidth,
                    options.PageHeight);
            }
            else if (cadImage.UnitType == UnitType.Millimeter)
            {
                options.PageHeight = CommonHelper.MMtoPixels(sizeExtY, CommonHelper.DPI);
                options.PageWidth = CommonHelper.MMtoPixels(sizeExtX, CommonHelper.DPI);
                _logger.LogInformation("UnitType: {0}, Width: {1}, Height: {3}", "Millimeter", options.PageWidth,
                    options.PageHeight);
            }
            else
            {
                options.PageWidth = (float)Math.Round(sizeExtX, 2);
                options.PageHeight = (float)Math.Round(sizeExtY, 2);
                _logger.LogInformation("UnitType: {0}, Width: {1}, Height: {3}", "Unknown", options.PageWidth,
                    options.PageHeight);
            }

            var svgOptions = new SvgOptions
            {
                VectorRasterizationOptions = options,
                CancellationToken = cancellationToken
            };

            var outputStream = new MemoryStream();
            await cadImage.SaveAsync(outputStream, svgOptions);
            outputStream.Seek(0, SeekOrigin.Begin); // Reset stream position to the beginning


            _logger.LogInformation("Conversion completed!");

            return new FileStreamResult(outputStream, "image/svg+xml")
            {
                FileDownloadName = $"{Path.GetFileNameWithoutExtension(file.FileName)}.svg"
            };
        }
        catch (ImageLoadException ex)
        {
            return StatusCode(500, $"Error loading CAD image: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}

public static class Size
{
    public static (float, float) CalculateSize(float currentWidth, float currentHeight, float expectedWidth)
    {
        /*var ratioX = (float)expectedWidth / currentWidth;
        var ratioY = (float)expectedHeight / currentHeight;
        var ratio = Math.Min(ratioX, ratioY);

        var newWidth = (float)(currentWidth * ratio);
        var newHeight = (float)(currentHeight * ratio);*/
        float aspectRatio = (float)Math.Round((currentHeight / currentWidth), 2);
        float newHeight = (float)Math.Round((expectedWidth * aspectRatio), 2);

        return (expectedWidth, newHeight);
    }
}