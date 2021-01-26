using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers
{
    public static class ControllerExtensions
    {
        public static ActionResult Maybe<T>(this ControllerBase c, T obj)
        {
            if (obj == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new OkObjectResult(obj);
            }
        }

        public static ActionResult isSuccessful<T>(this ControllerBase c, bool val)
        {

            if (val)
            {
                return new NoContentResult();
            }
            else
            {
                return new NotFoundResult();
            }
        }

    }
}