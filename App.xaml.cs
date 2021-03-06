﻿using Cards.Core;
using DLToolkit.Forms.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Cards
{
	public partial class App : Application
	{
		public App ()
		{
			InitializeComponent();

            FlowListView.Init();

            //MainPage = new NavigationPage(new MainPage());
 
            MainPage = new NavigationPage(new Cards.NewCardPage());
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
