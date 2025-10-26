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
            // SecretsReader service gets API Key from Secrets.json file
            var apiKey = SecretsReader.GetApiKey();

            // URL for Google Books API search
            var searchUrl = $"https://www.googleapis.com/books/v1/volumes?q={query}&key={apiKey}";

            using var client = new HttpClient();
            var response = await client.GetStringAsync(searchUrl);

            // Deserialize the API response into the new model class
            var searchResult = JsonSerializer.Deserialize<GoogleBooksApiResponse>(response);

            if (searchResult?.items?.Length > 0)
            {
                // Map the API data to local Book class
                var books = searchResult.items.Select(item => new Book
                {
                    // Assign properties with null checks and defaults
                    title = item.volumeInfo?.title ?? "N/A",
                    description = item.volumeInfo?.description ?? "N/A",
                    thumbnail = item.volumeInfo.imageLinks?.smallThumbnail ?? "book_placeholder.png",
                    publisher = item.volumeInfo?.publisher ?? "N/A",
                    publishedDate = item.volumeInfo?.publishedDate ?? "N/A",
                    pageCount = item.volumeInfo?.pageCount ?? 0,
                    // Convert arrays to comma-separated strings
                    categories = ListToString(item.volumeInfo.categories),
                    authors = ListToString(item.volumeInfo.authors),
                    // Default status for new search results
                    status = "0",
                    Id = item.id, 
                }).ToList();

                BookViewModel.Current.LoadSearchResults(books);

                // Navigate to search results page
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




