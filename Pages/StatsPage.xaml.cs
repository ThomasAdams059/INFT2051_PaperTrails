using System.Windows.Input;
using PaperTrails_ThomasAdams_c3429938.Models;
using PaperTrails_ThomasAdams_c3429938.Pages;

namespace PaperTrails_ThomasAdams_c3429938.Pages
{
    public partial class StatsPage : ContentPage
    {
        public ICommand NavigateToBookStatsCommand { get; }

        public StatsPage()
        {
            InitializeComponent();

            NavigateToBookStatsCommand = new Command<Book>(async (book) => await NavigateToBookStats(book));
            this.BindingContext = this;
        }

        private async Task NavigateToBookStats(Book tappedBook)
        {
            await Shell.Current.GoToAsync("SavedBookStatsPage", new Dictionary<string, object>
            {
                ["Book"] = tappedBook
            });
        }
    }
}