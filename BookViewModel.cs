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

        public BookViewModel()
        {
            // When this viewmodel is created, it creates a static reference to itself called Current. We'll use this to reference it from other pages.
            
            connection = DatabaseService.Connection;
            statsConnection = DatabaseService.StatsConnection;
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
        }
        public void DeleteBook(Book model)
        {
            //If it has an Id, then we can delete it
            if (model.LocalId > 0)
            {
                connection.Delete(model);
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

        public BookStats GetBookStats(int bookId)
        {
            return statsConnection.Table<BookStats>().FirstOrDefault(bs => bs.BookId == bookId);
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
