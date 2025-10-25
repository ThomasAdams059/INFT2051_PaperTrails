using PaperTrails_ThomasAdams_c3429938.ViewModels;
using PaperTrails_ThomasAdams_c3429938.Models;

namespace PaperTrails_ThomasAdams_c3429938.Pages;

// Query property to receive the Book object
[QueryProperty(nameof(Book), "Book")]
public partial class SavedBookDetails : ContentPage
{
    public Book Book { get; set; }

    public SavedBookDetails()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Set the BindingContext to the Book object for data binding
        BindingContext = this.Book;
    }

    private async void DeleteButton_Clicked(object sender, EventArgs e)
    {
        // Confirm with user before deleting
        if (await DisplayAlert("Confirm Delete", "Are you sure you want to delete this book?", "Yes", "No") != true)
        return;

        // Delete the book and its stats
        BookViewModel.Current.DeleteBook(this.Book);
        BookViewModel.Current.DeleteBookStats(this.Book.LocalId);
        await Navigation.PopAsync();
    }

    private async void StartReadingSession(object sender, EventArgs e)
    {
        // If the book is marked as "Read", confirm if user wants to re-read (resetting stats)
        if (this.Book.status == "3")
        {
            bool rereadConfirmed = await DisplayAlert("Re-read Book?", "This book is marked as 'Read'. Do you want to start a new reading session and re-read it? This will reset your stats with this book.", "Yes", "No");

            if (rereadConfirmed)
            {
                // Reset stats and status
                BookViewModel.Current.DeleteBookStats(this.Book.LocalId);

                // Update status to "Want To Read" (Will be set to "Reading" upon finishing session, this is in case user exits before completing reading session)
                this.Book.status = "3"; 
                BookViewModel.Current.SaveBook(this.Book);
            }
            else
            {
                // User chose not to re-read, exit the method
                return;
            }
        }

        // Navigate to the Reading Session page, passing the Book object and alert user of new reading session started
        await Shell.Current.GoToAsync("ReadingSession", new Dictionary<string, object>
            {
                ["Book"] = this.Book
            });

        await DisplayAlert("Reading Session", "New Reading Session Started", "OK");
    }
}