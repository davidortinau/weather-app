using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Geolocator;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using WeatherApp.Models;
using WeatherApp.Services;
using WeatherApp.Views;
using Xamarin.Forms;

namespace WeatherApp.ViewModels
{
	public class WeatherViewModel : BindableObject
	{
		private bool _isBusy;
		private string _temp;
		private string _condition;
		private WeatherForecastRoot _forecast;
		private ICommand _reloadCommand;
		private ICommand _getCurrentLocationCommand;

		INavigation _nav;
		public INavigation Navigation
		{
			set {
				_nav = value;
			}
		}

		public bool IsBusy
		{
			get { return _isBusy; }
			set
			{
				_isBusy = value;
				OnPropertyChanged();
			}
		}

		public string Temp
		{
			get { return _temp; }
			set
			{
				_temp = value;
				OnPropertyChanged();
			}
		}

		public string Condition
		{
			get { return _condition; }
			set
			{
				_condition = value;
				OnPropertyChanged();
			}
		}

		public WeatherForecastRoot Forecast
		{
			get { return _forecast; }
			set
			{
				_forecast = value;
				OnPropertyChanged();
			}
		}

		public ICommand ReloadCommand =>
				_reloadCommand ??
				(_reloadCommand = new Command(async () => await GetWeatherAsync()));

		public ICommand GetCurrentLocationCommand =>
			_getCurrentLocationCommand ??
		(_getCurrentLocationCommand = new Command(async () => await GetCurrentLocationAsync()));

		public async Task GetWeatherAsync(bool usePosition = false)
		{
			if (IsBusy)
				return;

			IsBusy = true;
			try
			{
				WeatherRoot weatherRoot = null;
				var units = AppSettings.IsImperial ? Units.Imperial : Units.Metric;

				if(usePosition && AppSettings.Position != null){
					weatherRoot = await WeatherService.Instance.GetWeatherAsync(AppSettings.Position.Latitude, AppSettings.Position.Longitude, units);
				}else{
					weatherRoot = await WeatherService.Instance.GetWeatherAsync(AppSettings.Location.Trim(), units);
				}

				Forecast = await WeatherService.Instance.GetForecast(weatherRoot.CityId, units);
				var unit = AppSettings.IsImperial ? "F" : "C";
				Temp = $"{weatherRoot?.MainWeather?.Temperature ?? 0}°{unit}";
				Condition = $"{weatherRoot.Name}: {weatherRoot?.Weather?[0]?.Description ?? string.Empty}";
			}
			catch (Exception)
			{
				Temp = "Unable to get Weather";
			}
			finally
			{
				IsBusy = false;
			}
		}

		public async Task GetCurrentLocationAsync()
		{
			if(Device.RuntimePlatform != Device.macOS)
			{
				var hasPermission = await Utils.CheckPermissions(Permission.Location);
				if (!hasPermission)
					return;

				var locator = CrossGeolocator.Current;

				var isMetric = System.Globalization.RegionInfo.CurrentRegion.IsMetric;
				AppSettings.IsImperial = !isMetric;

				var position = await locator.GetLastKnownLocationAsync();
				if(position == null)
					position = await locator.GetPositionAsync();
				
				AppSettings.Position = position;

				await GetWeatherAsync(true);
			}else{
				// alert to see city entry field
				_nav?.PushModalAsync(new LocationEntryPage(), true);
			}
		}
	}

	public static class Utils
	{
		public static async Task<bool> CheckPermissions(Permission permission)
		{
			var permissionStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);
			bool request = false;
			if (permissionStatus == PermissionStatus.Denied)
			{
				if (Device.RuntimePlatform == Device.iOS)
				{

					var title = $"{permission} Permission";
					var question = $"To use this plugin the {permission} permission is required. Please go into Settings and turn on {permission} for the app.";
					var positive = "Settings";
					var negative = "Maybe Later";
					var task = Application.Current?.MainPage?.DisplayAlert(title, question, positive, negative);
					if (task == null)
						return false;

					var result = await task;
					if (result)
					{
						CrossPermissions.Current.OpenAppSettings();
					}

					return false;
				}

				request = true;

			}

			if (request || permissionStatus != PermissionStatus.Granted)
			{
				var newStatus = await CrossPermissions.Current.RequestPermissionsAsync(permission);
				if (newStatus.ContainsKey(permission) && newStatus[permission] != PermissionStatus.Granted)
				{
					var title = $"{permission} Permission";
					var question = $"To use the plugin the {permission} permission is required.";
					var positive = "Settings";
					var negative = "Maybe Later";
					var task = Application.Current?.MainPage?.DisplayAlert(title, question, positive, negative);
					if (task == null)
						return false;

					var result = await task;
					if (result)
					{
						CrossPermissions.Current.OpenAppSettings();
					}
					return false;
				}
			}

			return true;
		}
	}
}