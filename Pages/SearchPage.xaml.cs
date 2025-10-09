using System.Net.Http;
using System.Text.Json;
using PaperTrails_ThomasAdams_c3429938.Models;
using PaperTrails_ThomasAdams_c3429938.Services;
using PaperTrails_ThomasAdams_c3429938.ViewModels;
namespace PaperTrails_ThomasAdams_c3429938.Pages;

public partial class SearchPage : ContentPage
{
	public SearchPage()
	{
		InitializeComponent();
	}

    private CancellationTokenSource _cancelTokenSource;
    private bool _isCheckingLocation;

    private async void GeoButton_Clicked(object sender, EventArgs e)
    {
        try
        {

            _isCheckingLocation = true;

            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

            _cancelTokenSource = new CancellationTokenSource();

            Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

            if (location != null)
                await DisplayAlert("Got Geolocation", $"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}", "OK");

        }

        catch (Exception ex)
        {
            await DisplayAlert("Geolocation Failure", "Unable to retrieve Geolocation.", "OK");
        }
        finally
        {
            _isCheckingLocation = false;
        }
    }

    public void CancelRequest()
    {
        if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
            _cancelTokenSource.Cancel();
    }


    private async void OnSearchButtonPressed(object sender, EventArgs e)
    {
        // The sender is the SearchBar that triggered the event.
        var searchBar = sender as SearchBar;
        var query = searchBar.Text;

        if (!string.IsNullOrWhiteSpace(query))
        {
            // Search function is called with the query
            await PerformBookSearch(query);
        }
    }

    private async Task PerformBookSearch(string query)
    {
        try
        {
            var apiKey = SecretsReader.GetApiKey();
            var searchUrl = $"https://www.googleapis.com/books/v1/volumes?q={query}&key={apiKey}";

            using var client = new HttpClient();
            var response = await client.GetStringAsync(searchUrl);

            // Deserialize the API response into the new model class
            var searchResult = JsonSerializer.Deserialize<GoogleBooksApiResponse>(response);

            if (searchResult?.items?.Length > 0)
            {
                // Map the API data to your local Book class
                var books = searchResult.items.Select(item => new Book
                {
                    title = item.volumeInfo.title,
                    description = item.volumeInfo.description,
                    
                    publisher = item.volumeInfo.publisher,
                    publishedDate = item.volumeInfo.publishedDate,
                    pageCount = item.volumeInfo.pageCount,
                    categories = ListToString(item.volumeInfo.categories),
                    authors = ListToString(item.volumeInfo.authors),
                    Id = item.id, // Store the API ID as a string
                    // You can add more properties here as needed
                }).ToList();

                BookViewModel.Current.LoadSearchResults(books);

                // Now you can navigate to a search results page
                await Shell.Current.GoToAsync("SearchResultsPage");

            }
            else
            {
                await DisplayAlert("No Results", "No books found for your search term.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private string ListToString(string[] list)
    {
        if (list == null || list.Length == 0)
            return "Unknown";
        return string.Join(", ", list);
    }
}


    


