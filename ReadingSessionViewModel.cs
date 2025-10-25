using PaperTrails_ThomasAdams_c3429938.Models;
using PaperTrails_ThomasAdams_c3429938.Services;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Maui.Devices.Sensors;

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
            // Initialize with the current book and start time
            CurrentBook = book;
            _sessionStartTime = DateTime.Now;
            FinishSessionCommand = new Command(FinishSession);
        }

        private async void FinishSession()
        {
            // Prevents users from entering more pages than the book contains
            if (PagesRead > CurrentBook.pageCount)
            {
                await Application.Current.MainPage.DisplayAlert("Invalid Page Number", $"The number of pages read cannot exceed the total page count of {CurrentBook.pageCount}.", "OK");
                return;
            }

            try
            {
                // Get current location (using a medium accuracy request with a timeout)
                var location = await Geolocation.GetLocationAsync(
                    new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10)));

                if (location != null)
                {
                    // Create and populate the new ReadingLocation model
                    var readingLocation = new ReadingLocation
                    {
                        BookLocalId = CurrentBook.LocalId,
                        Latitude = location.Latitude,
                        Longitude = location.Longitude,
                        TimeStamp = DateTime.Now,
                        Description = $"Session {BookViewModel.Current.GetBookStats(CurrentBook.LocalId)?.readingSessionNum + 1 ?? 1} End"
                    };

                    // Save the location using the ViewModel
                    BookViewModel.Current.SaveReadingLocation(readingLocation);
                }
            }
            catch (Exception ex)
            {
                // Catch all other exceptions (timeouts, etc.)
                System.Diagnostics.Debug.WriteLine($"Error getting location: {ex.Message}");
            }

            // Calculate time spent reading using the session start time (App is intended for non-moving sessions with connectivity, so timezones are a non-factor as of right now)
            TimeSpan timeSpent = DateTime.Now - _sessionStartTime;

            BookStats existingStats = BookViewModel.Current.GetBookStats(CurrentBook.LocalId);

            if (existingStats == null)
            {
                // Ensure status is set to "Reading" if not already
                CurrentBook.status = "2"; 

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
                // Mark book as Read
                CurrentBook.status = "3"; 
            }

            BookViewModel.Current.SaveBook(CurrentBook);

            await Application.Current.MainPage.DisplayAlert("Session Saved", "Your reading session has been saved.", "OK");
            // Navigate back
            await Shell.Current.GoToAsync(".."); 
        }
    }
}