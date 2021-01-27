using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Models.Reservations
{
    public class PostReservationRequest
    {
        [Required]
        public string For { get; set; }

        [Required]
        public string Books { get; set; }
    }
}