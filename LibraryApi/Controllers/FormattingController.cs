using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers
{
    public class FormattingController : ControllerBase
    {
        private readonly IFormatNames _nameFormatter;

        public FormattingController(IFormatNames nameFormatter)
        {
            _nameFormatter = nameFormatter;
        }

        [HttpGet("formates/name/{first}/{last}")]
        public ActionResult FormateName(string last, string first)
        {
            //Step One - think about what you want
            // WTCYWYH - Write the Code you wish you had
            string response = _nameFormatter.FormatName(first, last);
            //var response = $"{last}, { first}";
            return Ok(new { fullName = response });
        }
    }
}