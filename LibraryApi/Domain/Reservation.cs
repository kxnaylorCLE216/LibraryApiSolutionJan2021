using System;

namespace LibraryApi.Domain
{
    public class Reservation
    {
        public int Id { get; set; }

        public string For { get; set; }
        public string Books { get; set; }
        public ReservationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}