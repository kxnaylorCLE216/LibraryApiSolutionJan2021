namespace LibraryApi.Services
{
    public class FormalFormatting : IFormatNames
    {
        public string FormatName(string first, string last)
        {
            return $"{last}, {first}";
        }
    }
}