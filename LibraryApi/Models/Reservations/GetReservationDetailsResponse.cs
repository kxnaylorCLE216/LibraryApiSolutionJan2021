using LibraryApi.Domain;

namespace LibraryApi.Models.Reservations
{
    public class GetReservationDetailsResponse
    {
        public int Id { get; set; }
        public string For { get; set; }
        public string Books { get; set; }
        public ReservationStatus Status { get; set; }
    }
}