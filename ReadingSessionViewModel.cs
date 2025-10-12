using PaperTrails_ThomasAdams_c3429938.Models;
using PaperTrails_ThomasAdams_c3429938.Services;
using System.ComponentModel;
using System.Windows.Input;

namespace PaperTrails_ThomasAdams_c3429938.ViewModels
{
    public class ReadingSessionViewModel : ObservableObject
    {
        private DateTime _sessionStartTime;
        private int _pagesRead;

        public Book CurrentBook { get; set; }

        public int PagesRead
        {
            get => _pagesRead;
            set
            {
                if (_pagesRead != value)
                {
                    _pagesRead = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand FinishSessionCommand { get; }

        public ReadingSessionViewModel(Book book)
        {
            CurrentBook = book;
            _sessionStartTime = DateTime.Now;
            FinishSessionCommand = new Command(FinishSession);
        }

        private async void FinishSession()
        {
            if (PagesRead > CurrentBook.pageCount)
            {
                await Application.Current.MainPage.DisplayAlert("Invalid Page Number", $"The number of pages read cannot exceed the total page count of {CurrentBook.pageCount}.", "OK");
                return; // Exit the method and do not save the session
            }

            TimeSpan timeSpent = DateTime.Now - _sessionStartTime;

            BookStats existingStats = BookViewModel.Current.GetBookStats(CurrentBook.LocalId);

            if (existingStats == null)
            {
                var newStats = new BookStats
                {
                    BookId = CurrentBook.LocalId,
                    pagesRead = PagesRead,
                    timeSpentReading = timeSpent,
                    readingSessionNum = 1
                };

                BookViewModel.Current.SaveBookStats(newStats);
            }
            else
            {
                // If stats exist, update the record
                existingStats.pagesRead = PagesRead;
                existingStats.timeSpentReading += timeSpent;
                existingStats.readingSessionNum += 1;
                BookViewModel.Current.SaveBookStats(existingStats);
            }

            // Save the updated book

            if (PagesRead == CurrentBook.pageCount)
            {
                CurrentBook.status = "3"; // Mark as Read
            }

            BookViewModel.Current.SaveBook(CurrentBook);

            await Application.Current.MainPage.DisplayAlert("Session Saved", "Your reading session has been saved.", "OK");
            await Shell.Current.GoToAsync(".."); // Navigate back
        }
    }
}