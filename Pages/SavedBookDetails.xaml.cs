using PaperTrails_ThomasAdams_c3429938.ViewModels;
using PaperTrails_ThomasAdams_c3429938.Models;

namespace PaperTrails_ThomasAdams_c3429938.Pages;

public partial class SavedBookDetails : ContentPage
{
	public SavedBookDetails()
	{
		InitializeComponent();
	}

    private async void DeleteButton_Clicked(object sender, EventArgs e)
    {
        if (await DisplayAlert("Confirm Delete", "Are you sure you want to delete this book?", "Yes", "No") != true)
        return;
        var model = (Book)Parent.BindingContext;
        BookViewModel.Current.DeleteBook(model);
        await Navigation.PopAsync();
    }

    private async void StartReadingSession(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///ReadingSession");
    }
}