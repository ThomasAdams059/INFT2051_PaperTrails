using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;


namespace PaperTrails_ThomasAdams_c3429938.Models
{
    [Table("BookStats")]
    public class BookStats : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int LocalId { get; set; }

        // Set to Id from Google Books API
        public int BookId { get; set; }

        // Current Page/Amount of pages read for this book so far
        public int pagesRead { get; set; }
        // Time from starting reading session to finishing reading session
        public TimeSpan timeSpentReading { get; set; }

        public int readingSessionNum { get; set; }

        public TimeSpan avgReadingTime => readingSessionNum > 0 ? timeSpentReading / readingSessionNum : TimeSpan.Zero;

        public int avgPagesPerSession => readingSessionNum > 0 ? pagesRead / readingSessionNum : 0;
    }
}
