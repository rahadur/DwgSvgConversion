using Aspose.CAD;

namespace DwgSvgConversion;

public class AsposeLicense
{
    /// <summary>
    /// Loads the Aspose.CAD license file from the specified directory.
    /// </summary>
    /// <remarks>
    /// The license file must be named "Aspose.Cad.lic".
    /// </remarks>
    /// <param name="filePath">The directory path where the license file is located.</param>
    public static void Load(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            Console.WriteLine("License path is invalid.");
            return;
        }

        var licenseFile = Path.Combine(filePath, "Aspose.Cad.lic");

        if (!File.Exists(licenseFile))
        {
            Console.WriteLine("License file not found.");
            return;
        }

        new License().SetLicense(licenseFile);
        Console.WriteLine("License loaded successfully.");
    }
}
