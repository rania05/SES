﻿using MvvmCross.IoC;
using MvvmCross.ViewModels;
using Syracuse.Mobitheque.Core.Models;
using Syracuse.Mobitheque.Core.Services.Database;
using Syracuse.Mobitheque.Core.ViewModels;
using Syracuse.Mobitheque.Core.ViewModels.Sorts;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Syracuse.Mobitheque.Core
{
    public class App : MvxApplication
    {
        private string deviceOS { get; set; }

        public string DeviceOS
        {
            get { return deviceOS; }
            set { deviceOS = value; }
        }

        static CookiesDatabase database;

        public static CookiesDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new CookiesDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CookiesDatabase.db3"));
                }
                return database;
            }
        }

        static DocumentsDatabase docDatabase;

        public static DocumentsDatabase DocDatabase
        {
            get
            {
                if (docDatabase == null)
                {
                    docDatabase = new DocumentsDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DocumentsDatabase.db3"));
                }
                return docDatabase;
            }
        }

        static ApplicationState appState;

        public static ApplicationState AppState
        {
            get
            {
                if (appState == null)
                {
                    appState = new ApplicationState(false);
                }
                return appState;
            }
        }
        public override async void Initialize()
        {
            Log.Warning("Mobidoc", "Start App");
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            SortAlgorithmFactory.RegisterAlgorithms();

             CookiesSave user = await Database.GetActiveUser();
            if (user != null)
            {
                if (user.RememberMe)
                {
                    RegisterAppStart<MasterDetailViewModel>();
                }
                else
                {
                    RegisterAppStart<LoginViewModel>();
                }
            }
            else { 
                RegisterAppStart<SelectLibraryViewModel>();
            }
            Log.Warning("Mobidoc", "End App");
        }
    }
}
