namespace LibraryApi.Services
{
    public class InformalFormatting : IFormatNames
    {
        public string FormatName(string first, string last)
        {
            return $"{first}, {last}";
        }
    }
}