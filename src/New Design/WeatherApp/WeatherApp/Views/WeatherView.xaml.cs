using WeatherApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WeatherApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class WeatherView : ContentPage
	{
		WeatherViewModel _vm;

		public WeatherView()
		{
			InitializeComponent();

			BindingContext = _vm = new WeatherViewModel
			{
				Navigation = Navigation
			};
		}

		protected override async void OnAppearing()
		{
			if (BindingContext is WeatherViewModel)
			{
				await ((WeatherViewModel)BindingContext).GetWeatherAsync();
			}

			InitMenu();

			base.OnAppearing();
		}

		void InitMenu()
		{
			var mainMenu = new Menu();

			var fileMenu = new Menu { Text = "File" };
			var exitItem = new MenuItem
			{
				Text = "Exit",
				Command = new Command((obj) =>
				{
					Application.Current.Quit();
				})
			};
			MenuItem.SetAccelerator(exitItem, Accelerator.FromString("cmd+q"));
			fileMenu.Items.Add(exitItem);

			var locationMenu = new Menu { Text = "Location" };
			var changeItem = new MenuItem
			{
				Text = "Change",
				Command = new Command((obj) =>
				{
					Navigation.PushModalAsync(new LocationEntryPage());
				})
			};
			locationMenu.Items.Add(changeItem);

			var refreshItem = new MenuItem
			{
				Text = "Refresh",
				Command = _vm.ReloadCommand
			};
			MenuItem.SetAccelerator(refreshItem, Accelerator.FromString("cmd+r"));
			locationMenu.Items.Add(refreshItem);

			mainMenu.Add(fileMenu);
			mainMenu.Add(locationMenu);
			SetMenu(Application.Current, mainMenu); // uncomment when we figure out why it blanks out the app
		}
	}
}