namespace PaperTrails_ThomasAdams_c3429938.Pages;

// SearchResultsPage.xaml.cs
using System.Collections.ObjectModel;
using PaperTrails_ThomasAdams_c3429938.Models;
using PaperTrails_ThomasAdams_c3429938.ViewModels;

public partial class SearchResultsPage : ContentPage
{
    public SearchResultsPage()
    {
        InitializeComponent();
        BindingContext = BookViewModel.Current;
    }
}