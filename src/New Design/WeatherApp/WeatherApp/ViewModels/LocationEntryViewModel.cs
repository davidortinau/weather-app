using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace WeatherApp.ViewModels
{
	public class LocationEntryViewModel : BindableObject
	{
		INavigation _nav;
		public INavigation Navigation
		{
			set {
				_nav = value;
			}
		}

		public string _city;
		public string City
		{
			get { return _city; }
			set
			{
				_city = value;
				OnPropertyChanged();
			}
		}

		private ICommand _submitCommand;

		public ICommand SubmitCommand =>
		_submitCommand ??
		(_submitCommand = new Command(() => Submit()));

		void Submit()
		{
			AppSettings.Location = _city;
			_nav?.PopModalAsync(true);
		}

		public LocationEntryViewModel()
		{
		}
	}
}
