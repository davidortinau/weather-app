﻿using System;
using WeatherApp.Views;
using Xamarin.Forms;

namespace WeatherApp
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			MainPage = new CustomNavigationPage(new WeatherView())
			{
				BarBackgroundColor = Color.Black,
				BarTextColor = Color.White
			};
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
