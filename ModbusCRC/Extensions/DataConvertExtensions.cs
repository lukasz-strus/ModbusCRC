namespace ModbusCRC.Extensions;

internal static class DataConvertExtensions
{
    // ReSharper disable once InconsistentNaming
    private static readonly char[] HEX_ARRAY = "0123456789ABCDEF".ToCharArray();

    internal static byte[] ToByteArray(this string s)
    {
        if (s.Length % 2 != 0) s = "0" + s;

        var len = s.Length;
        var data = new byte[len / 2];
        for (var i = 0; i < len; i += 2)
        {
            data[i / 2] = Convert.ToByte(s.Substring(i, 2), 16);
        }
        return data;
    }

    internal static string ToHex(this byte[] bytes, bool bigEndian = true)
    {
        var hexChars = new char[bytes.Length * 2];
        for (var j = 0; j < bytes.Length; j++)
        {
            int v = bigEndian ? bytes[j] : bytes[bytes.Length - j - 1];

            hexChars[j * 2] = HEX_ARRAY[v >> 4];
            hexChars[j * 2 + 1] = HEX_ARRAY[v & 0x0F];
        }
        return new string(hexChars);
    }
}