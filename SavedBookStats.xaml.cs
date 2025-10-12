using PaperTrails_ThomasAdams_c3429938.ViewModels;
using PaperTrails_ThomasAdams_c3429938.Models;

namespace PaperTrails_ThomasAdams_c3429938.Pages;

[QueryProperty(nameof(Book), "Book")]
public partial class SavedBookStats : ContentPage
{
    public Book Book { get; set; }
    public BookStats BookStats { get; set; }

    public SavedBookStats()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        this.BookStats = BookViewModel.Current.GetBookStats(this.Book.LocalId);

        BindingContext = this;
    }
}