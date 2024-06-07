namespace DwgSvgConversion;

public static class CommonHelper
{
    public const double DPI = 96.0;
    public static int INtoPixels(double inches, double dpi)
    {
        return (int)(inches * dpi);
    }
    
    public static int MMtoPixels(double millimeters, double dpi)
    {
        double inches = millimeters / 25.4;
        return (int)(inches * dpi);
    }
}