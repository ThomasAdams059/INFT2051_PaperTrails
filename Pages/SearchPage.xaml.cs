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

    
}

