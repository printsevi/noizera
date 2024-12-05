using System.Globalization;
using System.Text.RegularExpressions;

namespace Noizera.Common.Domain.Common;

public class ValidFileName
{
    public string Value { get; }

    private ValidFileName(string value) => Value = value;

    public static ValidFileName New(string fileName)
    {
        string validName = MakeValidFileName(fileName);
        return new(validName);
    }

    private static string MakeValidFileName(string name)
    {
        string invalidChars = new(Path.GetInvalidFileNameChars());
        string escapedInvalidChars = Regex.Escape(invalidChars);
        string invalidRegex = string.Format(CultureInfo.InvariantCulture, @"([{0}]*\.+$)|([{0}]+)", escapedInvalidChars);
        string validFileName = Regex.Replace(name, invalidRegex, "_");
        string result = Path.GetFileNameWithoutExtension(validFileName);

        return result.Length > Constants.OriginalFileMaxLength ? result[..Constants.OriginalFileMaxLength] : result;
    }
}
