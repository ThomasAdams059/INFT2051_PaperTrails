using PaperTrails_ThomasAdams_c3429938.Models;
using PaperTrails_ThomasAdams_c3429938.Services;
using SQLite;
using System.Collections.ObjectModel;

namespace PaperTrails_ThomasAdams_c3429938.ViewModels
{
    internal class BookViewModel : ObservableObject
    {
        public static BookViewModel Current { get; set; }

        SQLiteConnection connection;

        SQLiteConnection statsConnection;

        SQLiteConnection readingLocationsConnection;

        // --- PRIVATE FIELDS FOR AGGREGATE STATS ---
        private int _totalPagesRead;
        private int _totalBooksRead;
        private TimeSpan _totalTimeSpent;
        private TimeSpan _averageReadingTime;

       public int TotalPagesRead
        {
            get => _totalPagesRead;
            set { if (_totalPagesRead != value) { _totalPagesRead = value; OnPropertyChanged(); } }
        }

        public int TotalBooksRead
        {
            get => _totalBooksRead;

            set { if (_totalBooksRead != value) { _totalBooksRead = value; OnPropertyChanged(); } }
        }

        // Changed return type to raw TimeSpan
        public TimeSpan TotalTimeSpent
        {
            get => _totalTimeSpent;
            set { if (_totalTimeSpent != value) { _totalTimeSpent = value; OnPropertyChanged(); } }
        }

        // Changed return type to raw TimeSpan
        public TimeSpan AverageReadingTime
        {
            get => _averageReadingTime;
            set { if (_averageReadingTime != value) { _averageReadingTime = value; OnPropertyChanged(); } }
        }
        // ------------------------------------------

        public BookViewModel()
        {
            // When this viewmodel is created, it creates a static reference to itself called Current. We'll use this to reference it from other pages.
            
            connection = DatabaseService.Connection;
            statsConnection = DatabaseService.StatsConnection;
            readingLocationsConnection = DatabaseService.ReadingLocationsConnection;

        }

        public List<Book> Books
        {
            get
            {
                return connection.Table<Book>().ToList();
            }
        }

        public List<Book> WantToReadBooks => Books.Where(b => b.status == "1").ToList();
        public List<Book> ReadingBooks => Books.Where(b => b.status == "2").ToList();
        public List<Book> ReadBooks => Books.Where(b => b.status == "3").ToList();

        // --- DATA FETCHING METHODS ---
        public List<BookStats> GetAllBookStats()
        {
            return statsConnection.Table<BookStats>().ToList();
        }

        public List<ReadingLocation> GetAllReadingLocations()
        {
            return readingLocationsConnection.Table<ReadingLocation>().ToList();
        }
        // -----------------------------

        // --- NEW CALCULATION METHOD ---
        public void CalculateTotalStats()
        {
            var allStats = GetAllBookStats();
            var allLocations = GetAllReadingLocations();

            // 1. Total Pages Read & Total Books Read
            TotalPagesRead = allStats.Sum(s => s.pagesRead);
            TotalBooksRead = Books.Count(b => b.status == "3");

            // 2. Total Time Spent & Average Session Time
            // We sum TotalSeconds from BookStats, which stores the aggregate time for a book.
            double totalSeconds = allStats.Sum(s => s.timeSpentReading.TotalSeconds);
            int totalSessions = allLocations.Count;

            _totalTimeSpent = TimeSpan.FromSeconds(totalSeconds);

            double avgSeconds = 0;
            if (totalSessions > 0)
            {
                avgSeconds = totalSeconds / totalSessions;
            }
            _averageReadingTime = TimeSpan.FromSeconds(avgSeconds);


            // Notify UI for all calculated properties.
            // Note: Pages and Times are handled by property setters, but explicitly calling 
            // the filtered collections ensures the whole stats page is refreshed.
            OnPropertyChanged(nameof(TotalBooksRead));
            OnPropertyChanged(nameof(TotalPagesRead));
            OnPropertyChanged(nameof(TotalTimeSpent));
            OnPropertyChanged(nameof(AverageReadingTime));

            // Explicitly notify for the filtered lists to update the bottom section
            OnPropertyChanged(nameof(ReadBooks));
            OnPropertyChanged(nameof(ReadingBooks));
        }
        // -----------------------------------------

        public void SaveBook(Book model)
        {
            //If it has an Id, then it already exists and we can update it
            if (model.LocalId > 0)
            {
                connection.Update(model);
            }
            //If not, it's new and we need to add it
            else
            {
                connection.Insert(model);
            }

           /* OnPropertyChanged(nameof(ReadingBooks));
            OnPropertyChanged(nameof(WantToReadBooks));
            OnPropertyChanged(nameof(ReadBooks)); */
        }
        public void DeleteBook(Book model)
        {
            //If it has an Id, then we can delete it
            if (model.LocalId > 0)
            {
                DeleteAllReadingLocations(model.LocalId);
                DeleteBookStats(model.LocalId);
                connection.Delete(model);

                OnPropertyChanged(nameof(ReadBooks));
                OnPropertyChanged(nameof(ReadingBooks));
                OnPropertyChanged(nameof(WantToReadBooks));
                OnPropertyChanged(nameof(TotalBooksRead));

            }
        }

        public void SaveBookStats(BookStats model)
        {
            if (model.LocalId > 0)
            {
                statsConnection.Update(model);
            }
            else
            {
                statsConnection.Insert(model);
            }
        }

        public void DeleteBookStats(int bookId)
        {
            var stats = GetBookStats(bookId);
            if (stats != null)
            {
                statsConnection.Delete(stats);
            }
        }

        public BookStats GetBookStats(int bookId)
        {
            return statsConnection.Table<BookStats>().FirstOrDefault(bs => bs.BookId == bookId);
        }

        public void SaveReadingLocation(ReadingLocation location)
        {
            if (location.Id != 0)
            {
                // Update existing location (usually not needed for session logs)
                readingLocationsConnection.Update(location);
            }
            else
            {
                // Insert a new reading location record
                readingLocationsConnection.Insert(location);
            }
        }

        public List<ReadingLocation> GetReadingLocations(int bookLocalId)
        {
            // Retrieve all ReadingLocation records that match the specific Book's LocalId
            return readingLocationsConnection.Table<ReadingLocation>().Where(l => l.BookLocalId == bookLocalId).OrderBy(l => l.TimeStamp).ToList();
        }

        public void DeleteAllReadingLocations(int bookLocalId)
        {
            // Retrieve all ReadingLocation records that match the specific Book's LocalId
            var locationsToDelete = readingLocationsConnection.Table<ReadingLocation>()
                                              .Where(l => l.BookLocalId == bookLocalId)
                                              .ToList();

            // Iterate through the list and delete each one
            foreach (var location in locationsToDelete)
            {
                readingLocationsConnection.Delete(location);
            }
        }


        // New property to hold the search results
        public ObservableCollection<Book> SearchResults { get; set; }

        // Add a method to populate the search results
        public void LoadSearchResults(List<Book> books)
        {
            SearchResults = new ObservableCollection<Book>(books);
            OnPropertyChanged(nameof(SearchResults));
        }
    }
}
