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

        // Aggregate Statistics (used in TotalStats page)
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

        public TimeSpan TotalTimeSpent
        {
            get => _totalTimeSpent;
            set { if (_totalTimeSpent != value) { _totalTimeSpent = value; OnPropertyChanged(); } }
        }

        public TimeSpan AverageReadingTime
        {
            get => _averageReadingTime;
            set { if (_averageReadingTime != value) { _averageReadingTime = value; OnPropertyChanged(); } }
        }

        public BookViewModel()
        {
            // Create a singleton instance for easy access throughout the app
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

        // Filtered Book Lists
        public List<Book> WantToReadBooks => Books.Where(b => b.status == "1").ToList();
        public List<Book> ReadingBooks => Books.Where(b => b.status == "2").ToList();
        public List<Book> ReadBooks => Books.Where(b => b.status == "3").ToList();

        // Data Fetching Methods
        public List<BookStats> GetAllBookStats()
        {
            return statsConnection.Table<BookStats>().ToList();
        }

        public List<ReadingLocation> GetAllReadingLocations()
        {
            return readingLocationsConnection.Table<ReadingLocation>().ToList();
        }

        // Method to calculate aggregate statistics
        public void CalculateTotalStats()
        {
            var allStats = GetAllBookStats();
            var allLocations = GetAllReadingLocations();

            TotalPagesRead = allStats.Sum(s => s.pagesRead);
            TotalBooksRead = Books.Count(b => b.status == "3");

            // Sum TotalSeconds from BookStats, which stores the aggregate time for a book.
            double totalSeconds = allStats.Sum(s => s.timeSpentReading.TotalSeconds);
            // Each reading session logs a location, so total sessions = total locations
            int totalSessions = allLocations.Count;

            _totalTimeSpent = TimeSpan.FromSeconds(totalSeconds);

            double avgSeconds = 0;
            if (totalSessions > 0)
            {
                avgSeconds = totalSeconds / totalSessions;
            }
            _averageReadingTime = TimeSpan.FromSeconds(avgSeconds);


            // Notify UI for all calculated properties
            OnPropertyChanged(nameof(TotalBooksRead));
            OnPropertyChanged(nameof(TotalPagesRead));
            OnPropertyChanged(nameof(TotalTimeSpent));
            OnPropertyChanged(nameof(AverageReadingTime));

            // Explicitly notify for the filtered lists to update the bottom section
            OnPropertyChanged(nameof(ReadBooks));
            OnPropertyChanged(nameof(ReadingBooks));
        }

        public void SaveBook(Book model)
        {
            //If it has an Id, then it already exists and can be updated
            if (model.LocalId > 0)
            {
                connection.Update(model);
            }
            //If not, it's new and needs to be inserted
            else
            {
                connection.Insert(model);
            }
        }
        public void DeleteBook(Book model)
        {
            //If it has an Id, then we can delete it
            if (model.LocalId > 0)
            {
                // Delete associated reading locations and stats first
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
            // Fetch all locations associated with the specified Book LocalId
            var locationsToDelete = GetReadingLocations(bookLocalId);

            // Iterate through the list and delete each one
            foreach (var location in locationsToDelete)
            {
                readingLocationsConnection.Delete(location);
            }
        }


        // New property to hold the search results
        public ObservableCollection<Book> SearchResults { get; set; }

        // Populates the SearchResults collection
        public void LoadSearchResults(List<Book> books)
        {
            SearchResults = new ObservableCollection<Book>(books);
            OnPropertyChanged(nameof(SearchResults));
        }
    }
}
