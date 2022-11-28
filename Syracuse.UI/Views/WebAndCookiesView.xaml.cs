﻿using MvvmCross.Forms.Views;
using Syracuse.Mobitheque.Core.ViewModels;
using Xamarin.Forms.Xaml;

namespace Syracuse.Mobitheque.UI.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WebAndCookiesView : MvxContentPage<WebAndCookiesViewModel>
    {
        bool CanRefresh { get; set; } = true;
        public WebAndCookiesView()
        {
            InitializeComponent();
        }

    }

}