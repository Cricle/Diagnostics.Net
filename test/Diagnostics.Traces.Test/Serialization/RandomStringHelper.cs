namespace Diagnostics.Traces.Test.Serialization
{
    internal static class RandomStringHelper
    {
        private static readonly char[] chars = Enumerable.Range(0, 24).Select(x => (char)('a' + x)).ToArray();

        public static string CreateRandomString(int length)
        {
            var chars = new char[1024 * 2];
            var rand = new Random();
            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = chars[rand.Next(0, chars.Length)];
            }

            return new string(chars);
        }

    }
}
