using SQLite;
using Microsoft.Maui.Devices.Sensors;

namespace PaperTrails_ThomasAdams_c3429938.Models
{
    public class ReadingLocation
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Foreign Key to the Book.LocalId
        public int BookLocalId { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public DateTime TimeStamp { get; set; }

        // Optional: A brief note or address
        public string Description { get; set; }
    }
}