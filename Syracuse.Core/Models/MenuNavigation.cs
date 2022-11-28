﻿using Xamarin.Forms;

namespace Syracuse.Mobitheque.Core.Models
{
    public class MenuNavigation : MenuItem
    {
        public bool IsSelected { get; set; } = false;

        public string IconFontAwesome { get; set; }

        public string Color { get; set; } = "WhiteSmoke";


    }
}
