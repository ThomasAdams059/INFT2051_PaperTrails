using PaperTrails_ThomasAdams_c3429938.ViewModels;
using PaperTrails_ThomasAdams_c3429938.Models;

namespace PaperTrails_ThomasAdams_c3429938.Pages;

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

        BindingContext = this.Book;
    }

    private async void DeleteButton_Clicked(object sender, EventArgs e)
    {
        if (await DisplayAlert("Confirm Delete", "Are you sure you want to delete this book?", "Yes", "No") != true)
        return;
        BookViewModel.Current.DeleteBook(this.Book);
        await Navigation.PopAsync();
    }

    private async void StartReadingSession(object sender, EventArgs e)
    {
        if (this.Book.status == "1")
        {
            this.Book.status = "2"; // Update status to "Reading"
            // Save the book to the database.
            BookViewModel.Current.SaveBook(this.Book);
        }
        await Shell.Current.GoToAsync("ReadingSession", new Dictionary<string, object>
        {
            ["Book"] = this.Book
        });

        await DisplayAlert("Reading Session", "New Reading Session Started", "OK");
    }
}