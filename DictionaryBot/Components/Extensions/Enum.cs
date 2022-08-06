namespace DictionaryBot.Components.Extensions
{
    public static class EnumExtension
    {
        public static int GetAlignCount(this Enum E)
        {
            var enumType = E.GetType();
            var nameArray = Enum.GetNames(enumType).OrderByDescending(e => e.Length).ToArray();

            var longestLen = nameArray[0].Length;
            var currentLen = Enum.GetName(enumType, E).Length;

            return longestLen - currentLen;
        }
    }
}
