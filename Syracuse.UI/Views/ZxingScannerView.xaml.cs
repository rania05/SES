﻿using MvvmCross.Forms.Presenters.Attributes;
using Rg.Plugins.Popup.Services;
using Syracuse.Mobitheque.Core.Services.Database;
using System.Threading.Tasks;
using ZXing;
using ZXing.Net.Mobile.Forms;

namespace Syracuse.Mobitheque.UI.Views
{
    [MvxContentPagePresentation(NoHistory = true)]
    public partial class ZxingScannerView : ZXingScannerPage
    {
        private TutorialPopup _tutorialPopup;
        public ZxingScannerView()
        {
            InitializeComponent();
            DisplayPopUp();
        }

        /// <summary>
        /// Cette methoode permet de determiner, si l'utilisateur souhaite que l'on affiche le tutoriel ou non 
        /// </summary>
        /// <returns></returns>
        private async Task DisplayPopUp()
        {
            var database = Syracuse.Mobitheque.Core.App.Database;
            var user = await database.GetActiveUser();
            if (user != null)
            {
                if (user.IsTutorial)
                {
                    _tutorialPopup = new TutorialPopup(database , user);
                    await PopupNavigation.Instance.PushAsync(_tutorialPopup);
                }
            }
            else
            {
                _tutorialPopup = new TutorialPopup();
                await PopupNavigation.Instance.PushAsync(_tutorialPopup);
            }
        }

        public void Handle_OnScanResult(Result result)
        {
            //Implement in the parent.
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            IsScanning = true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            IsScanning = false;
        }

        
#pragma warning disable S125 // Sections of code should not be commented out
/*

        ZXingScannerView zxing;
        ZXingDefaultOverlay defaultOverlay = null;

        public TestBarCode(/*MobileBarcodeScanningOptions options = null, View customOverlay = null* /) : base()
        {
           // MobileBarcodeScanningOptions options = null;
            var options = new MobileBarcodeScanningOptions();
            options.TryHarder = true;
            options.InitialDelayBeforeAnalyzingFrames = 300;
            options.DelayBetweenContinuousScans = 100;
            options.DelayBetweenAnalyzingFrames = 200;
            options.AutoRotate = false;
            View customOverlay = null;
            zxing = new ZXingScannerView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Options = options,



                AutomationId = "zxingScannerView"
            };

            zxing.IsScanning = true;

            zxing.SetBinding(ZXingScannerView.IsTorchOnProperty, new Binding(nameof(IsTorchOn)));
            zxing.SetBinding(ZXingScannerView.IsAnalyzingProperty, new Binding(nameof(IsAnalyzing)));
            zxing.SetBinding(ZXingScannerView.IsScanningProperty, new Binding(nameof(IsScanning)));
            zxing.SetBinding(ZXingScannerView.HasTorchProperty, new Binding(nameof(HasTorch)));
            zxing.SetBinding(ZXingScannerView.ResultProperty, new Binding(nameof(Result)));

            zxing.OnScanResult += (result) => {
                this.OnScanResult?.Invoke(result);
                Device.BeginInvokeOnMainThread (() => Console.WriteLine(result));
            };

            if (customOverlay == null)
            {
                defaultOverlay = new ZXingDefaultOverlay() { AutomationId = "zxingDefaultOverlay" };

                defaultOverlay.SetBinding(ZXingDefaultOverlay.TopTextProperty, new Binding(nameof(DefaultOverlayTopText)));
                defaultOverlay.SetBinding(ZXingDefaultOverlay.BottomTextProperty, new Binding(nameof(DefaultOverlayBottomText)));
                defaultOverlay.SetBinding(ZXingDefaultOverlay.ShowFlashButtonProperty, new Binding(nameof(DefaultOverlayShowFlashButton)));

                DefaultOverlayTopText = "Hold your phone up to the barcode";
                DefaultOverlayBottomText = "Scanning will happen automatically";
                DefaultOverlayShowFlashButton = HasTorch;

                defaultOverlay.FlashButtonClicked += (sender, e) => {
                    zxing.IsTorchOn = !zxing.IsTorchOn;
                };

                Overlay = defaultOverlay;
            }
            else
            {
                Overlay = customOverlay;
            }

            var grid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            grid.Children.Add(zxing);
            grid.Children.Add(Overlay);

            // The root page of your application
            Content = grid;
        }

        #region Default Overlay Properties

        public static readonly BindableProperty DefaultOverlayTopTextProperty =
            BindableProperty.Create(nameof(DefaultOverlayTopText), typeof(string), typeof(ZXingScannerPage), string.Empty);
        public string DefaultOverlayTopText
        {
            get { return (string)GetValue(DefaultOverlayTopTextProperty); }
            set { SetValue(DefaultOverlayTopTextProperty, value); }
        }

        public static readonly BindableProperty DefaultOverlayBottomTextProperty =
            BindableProperty.Create(nameof(DefaultOverlayBottomText), typeof(string), typeof(ZXingScannerPage), string.Empty);
        public string DefaultOverlayBottomText
        {
            get { return (string)GetValue(DefaultOverlayBottomTextProperty); }
            set { SetValue(DefaultOverlayBottomTextProperty, value); }
        }

        public static readonly BindableProperty DefaultOverlayShowFlashButtonProperty =
            BindableProperty.Create(nameof(DefaultOverlayShowFlashButton), typeof(bool), typeof(ZXingScannerPage), false);
        public bool DefaultOverlayShowFlashButton
        {
            get { return (bool)GetValue(DefaultOverlayShowFlashButtonProperty); }
            set { SetValue(DefaultOverlayShowFlashButtonProperty, value); }
        }

        #endregion

        public delegate void ScanResultDelegate(ZXing.Result result);
        public event ScanResultDelegate OnScanResult;

        public View Overlay
        {
            get;
            private set;
        }

        #region Functions

        public void ToggleTorch()
        {
            if (zxing != null)
                zxing.ToggleTorch();
        }

        /*protected override void Initialize()
        {
            base.OnAppearing();

            zxing.IsScanning = true;
        }

        protected override void OnDisappearing()
        {
            zxing.IsScanning = false;

            base.OnDisappearing();
        }* /

        public void PauseAnalysis()
        {
            if (zxing != null)
                zxing.IsAnalyzing = false;
        }

        public void ResumeAnalysis()
        {
            if (zxing != null)
                zxing.IsAnalyzing = true;
        }

        public void AutoFocus()
        {
            if (zxing != null)
                zxing.AutoFocus();
        }

        public void AutoFocus(int x, int y)
        {
            if (zxing != null)
                zxing.AutoFocus(x, y);
        }

        #endregion

        public static readonly BindableProperty IsTorchOnProperty =
            BindableProperty.Create(nameof(IsTorchOn), typeof(bool), typeof(ZXingScannerPage), false);
        public bool IsTorchOn
        {
            get { return (bool)GetValue(IsTorchOnProperty); }
            set { SetValue(IsTorchOnProperty, value); }
        }

        public static readonly BindableProperty IsAnalyzingProperty =
            BindableProperty.Create(nameof(IsAnalyzing), typeof(bool), typeof(ZXingScannerPage), false);
        public bool IsAnalyzing
        {
            get { return (bool)GetValue(IsAnalyzingProperty); }
            set { SetValue(IsAnalyzingProperty, value); }
        }

        public static readonly BindableProperty IsScanningProperty =
            BindableProperty.Create(nameof(IsScanning), typeof(bool), typeof(ZXingScannerPage), false);
        public bool IsScanning
        {
            get { return (bool)GetValue(IsScanningProperty); }
            set { SetValue(IsScanningProperty, value); }
        }

        public static readonly BindableProperty HasTorchProperty =
            BindableProperty.Create(nameof(HasTorch), typeof(bool), typeof(ZXingScannerPage), false);
        public bool HasTorch
        {
            get { return (bool)GetValue(HasTorchProperty); }
            set { SetValue(HasTorchProperty, value); }
        }

        public static readonly BindableProperty ResultProperty =
            BindableProperty.Create(nameof(Result), typeof(Result), typeof(ZXingScannerPage), default(Result));
        public Result Result
        {
            get { return (Result)GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value); }
        }*/
    }
#pragma warning restore S125 // Sections of code should not be commented out
}