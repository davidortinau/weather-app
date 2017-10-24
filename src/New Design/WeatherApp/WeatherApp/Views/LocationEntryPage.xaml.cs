using System;
using System.Collections.Generic;
using WeatherApp.ViewModels;
using Xamarin.Forms;

namespace WeatherApp.Views
{
	public partial class LocationEntryPage : ContentPage
	{
		public LocationEntryPage()
		{
			InitializeComponent();

			BindingContext = new LocationEntryViewModel {
				Navigation = this.Navigation
			};
		}
	}
}
