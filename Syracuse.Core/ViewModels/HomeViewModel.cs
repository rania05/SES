using MvvmCross.Commands;
using MvvmCross.Navigation;
using Newtonsoft.Json;
using Syracuse.Mobitheque.Core.Models;
using Syracuse.Mobitheque.Core.Services.Requests;
using Syracuse.Mobitheque.Core.ViewModels.Sorts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using XF.Material.Forms.Models;

namespace Syracuse.Mobitheque.Core.ViewModels
{
    public class HomeViewModel : BaseDownloadPageViewModel
    {
        private int filterIndex = 0;
        private int page = 0;
        private readonly IRequestService requestService;
        private readonly IMvxNavigationService navigationService;



        private Result[] results  = new Result[0];
        public Result[] Results
        {
            get => this.results;
            set
            {
                SetProperty(ref this.results, value);
                if (this.Results.Count() < this.NbrResults)
                {
                    this.DisplayLoadMore = true;
                }
                else {
                    this.DisplayLoadMore = false;
                }
            }
        }
        private bool notCurrentEvent = false;
        public bool NotCurrentEvent
        {
            get => this.notCurrentEvent;
            set
            {
                SetProperty(ref this.notCurrentEvent, value);
                this.NotCurrentEventReverse = !this.notCurrentEvent;
            }
        }
        private long? nbrResults;
        public long? NbrResults
        {
            get => this.nbrResults;
            set
            {
                SetProperty(ref this.nbrResults, value);
            }
        }

        private bool displayLoadMore = true;
        public bool DisplayLoadMore
        {
            get => this.displayLoadMore;
            set
            {
                SetProperty(ref this.displayLoadMore, value);
            }
        }

        private bool notCurrentEventReverse = false;
        public bool NotCurrentEventReverse
        {
            get => this.notCurrentEventReverse;
            set
            {
                SetProperty(ref this.notCurrentEventReverse, value);
            }
        }

        private bool isEvent = false;
        public bool IsEvent
        {
            get => this.isEvent;
            set
            {
                this.ReverseIsEvent = !value;
                SetProperty(ref this.isEvent, value && NotCurrentEventReverse);
            }
        }
        /// <summary>
        /// Determine si la page doit afficher un événement ou non 
        /// </summary>
        private bool reverseIsEvent = false;
        public bool ReverseIsEvent
        {
            get => this.reverseIsEvent;
            set
            {
                SetProperty(ref this.reverseIsEvent, value && NotCurrentEventReverse);
            }
        }

        /// <summary>
        /// Variable qui indique si la page est occupé 
        /// </summary>
        private bool isBusy = true;
        public bool IsBusy
        {
            get => this.isBusy;
            set
            {
                SetProperty(ref this.isBusy, value);
            }
        }

        private MvxAsyncCommand<SearchResult> openDetailsCommand;
        public MvxAsyncCommand<SearchResult> OpenDetailsCommand => this.openDetailsCommand ??
            (this.openDetailsCommand = new MvxAsyncCommand<SearchResult>((result) => this.OpenResultDetails(result)));

        private MvxAsyncCommand<string> searchCommand;
        public MvxAsyncCommand<string> SearchCommand => this.searchCommand ??
            (this.searchCommand = new MvxAsyncCommand<string>((text) => this.PerformSearch(text)));

        private MvxAsyncCommand<Result> downloadDocumentCommand;
        public MvxAsyncCommand<Result> DownloadDocumentCommand => this.downloadDocumentCommand ??
            (this.downloadDocumentCommand = new MvxAsyncCommand<Result>((item) => this.DownloadDocument(item)));

        private MvxAsyncCommand<string> loadMore;
        public MvxAsyncCommand<string> LoadMore => this.loadMore ??
            (this.loadMore = new MvxAsyncCommand<string>((text) => this.getNextPage()));


        private Command<MaterialMenuResult> filtersCommand;
        public Command<MaterialMenuResult> FiltersCommand => this.filtersCommand ??
            (this.filtersCommand = new Command<MaterialMenuResult>((filter) => this.SetFilter(filter)));

        private readonly Dictionary<string, Func<IEnumerable<SearchResult>, IEnumerable<SearchResult>>> filters =
        new Dictionary<string, Func<IEnumerable<SearchResult>, IEnumerable<SearchResult>>>()
        {
                    { "A à Z (Titre)", (x) => SortAlgorithmFactory.GetAlgorithm(SortAlgorithm.ASCENDING).Sort(x, "Title")},
                    { "Z à A (Titre)", (x) => SortAlgorithmFactory.GetAlgorithm(SortAlgorithm.DESCENDING).Sort(x, "Title")},
                    { "A à Z (Auteur)", (x) => SortAlgorithmFactory.GetAlgorithm(SortAlgorithm.ASCENDING).Sort(x, "Author")},
                    { "Z à A (Auteur)", (x) => SortAlgorithmFactory.GetAlgorithm(SortAlgorithm.DESCENDING).Sort(x, "Author")},
                    { "+ récent", (x) => SortAlgorithmFactory.GetAlgorithm(SortAlgorithm.ASCENDING).Sort(x, "Date")},
                    { "- récent", (x) => SortAlgorithmFactory.GetAlgorithm(SortAlgorithm.DESCENDING).Sort(x, "Date")}
        };

        public List<string> FiltersName()
        {
            return this.filters.Keys.ToList();
        }

        private string library;
        public string Library
        {
            get => this.library;
            set
            {
                SetProperty(ref this.library, value);
            }
        }
        private String eventsScenarioCode;
        public string EventsScenarioCode
        {
            get => this.eventsScenarioCode;
            set
            {
                SetProperty(ref this.eventsScenarioCode, value);
            }
        }

        public HomeViewModel(IMvxNavigationService navigationService, IRequestService requestService)
        {
            this.navigationService = navigationService;
            this.requestService = requestService;
        }

        public async override Task Initialize()
        {
            this.IsBusy = true;
            var user = await App.Database.GetActiveUser();
            this.Library = user.Library;
            this.EventsScenarioCode = user.EventsScenarioCode;
            this.Results = await this.loadPage(user.IsEvent);

            if (this.results.Length == 0)
            {
                this.NotCurrentEvent = true;
            }
            else
            {
                this.NotCurrentEvent = false;
            }
            this.IsEvent = user.IsEvent;
            await this.GetRedirectURL();
            await this.RaiseAllPropertiesChanged();
            this.IsBusy = false;
            await base.Initialize();
        }

        public async Task DownloadAllDocument(Result[] results)
        {
            foreach (var result in results)
            {
                if (result.CanDownload)
                {
                    this.DownloadDocument(result);
                }

            }
        }

        /// <summary>
        /// Déclenche une oppération de télecharchement de document 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task DownloadDocument(Result result)
        {
            this.isBusy = true;
            await RaiseAllPropertiesChanged();
            var statusDownload = await this.requestService.GetDownloadDocument(result.downloadOptions.parentDocumentId, result.downloadOptions.documentId, result.downloadOptions.fileName);
            if (statusDownload.Success)
            {
                SaveNewDocumentDatabaseObject(result, statusDownload.D);
                foreach (var Result in this.Results)
                {
                    if (Result == result)
                    {
                        Result.CanDownload = false;
                        Result.IsDownload = true;
                    }
                }
            }
            else
            {
                this.DisplayAlert(ApplicationResource.Error, statusDownload.Errors?[0]?.Msg != null ? String.Format(ApplicationResource.DownloadGetFileError, result.FieldList.Title[0], statusDownload.Errors?[0]?.Msg) : ApplicationResource.ErrorOccurred, ApplicationResource.ButtonValidation);
            }
            this.isBusy = false;
            this.ForceListUpdate();
            await RaiseAllPropertiesChanged();
        }

        private async Task getNextPage()
        {

            this.IsBusy = true;
            await this.RaisePropertyChanged(nameof(IsBusy));
            this.page += 1;

            Result[] res = await loadPage(this.IsEvent);
            this.Results = this.Results.Concat(res).ToArray();
            await this.GetRedirectURL();
            this.IsBusy = false;
            await this.RaisePropertyChanged(nameof(IsBusy));
        }

        private async Task<Result[]> loadPage(bool IsSortField)
        {
            SearchOptions options = new SearchOptions();
            if (IsSortField)
            {
                options.Query = new SearchOptionsDetails()
                {
                    QueryString = "NumberOfDigitalNotices_int_idx:[1 TO *]",
                    ScenarioCode = this.eventsScenarioCode,
                    Page = this.page,
                    SortField = "DateStart_sort",
                    SortOrder = 1,

                };
            }
            else
            {
                options.Query = new SearchOptionsDetails()
                {
                    QueryString = "NumberOfDigitalNotices_int_idx:[1 TO *]",
                    ScenarioCode = this.eventsScenarioCode,
                    Page = this.page,
                };
            }
            var search = await this.requestService.Search(options);
            if (!search.Success)
            {
                this.DisplayAlert(ApplicationResource.Error, search.Errors?[0]?.Msg != null ? search.Errors?[0]?.Msg : ApplicationResource.ErrorOccurred, ApplicationResource.ButtonValidation);
                return new Result[0];
            }
            else
            {
                if (search != null )
                {
                    this.NbrResults = search?.D?.SearchInfo?.NbResults;
                    if (search.D != null && this.IsEvent)
                    {
                        search.D.Results = await this.CheckAvCheckAvailability(search.D.Results);
                    }
                }

            }
            return await this.HaveDownloadOption(search?.D?.Results, this.requestService);
        }
        public async Task<Result[]> CheckAvCheckAvailability(Result[] results)
        {

            List<RecordIdArray> RecordIdArray = new List<RecordIdArray>();
            CheckAvailabilityOptions optionsTempo = new CheckAvailabilityOptions();

            foreach (var result in results)
            {
                RecordIdArray.Add(new RecordIdArray(result.Resource.RscBase, result.Resource.RscId, result.Resource.Frmt));
            }

            optionsTempo.Query = new SearchOptionsDetails()
            {
                ScenarioCode = this.eventsScenarioCode,
                Page = this.page,
            };
            optionsTempo.RecordIdArray = RecordIdArray;

            // HTTP Request
            CheckAvailabilityResult rslts = await this.requestService.CheckAvailability(optionsTempo);

            var resultTempo = results.ToList();
            if (rslts.Success && rslts.D != null )
            {
                foreach (var rslt in rslts.D)
                {
                    int v = resultTempo.FindIndex(x => x.Resource.RscId == rslt.Id.RscId);
                    results[v].Resource.HtmlViewDisponibility = rslt.HtmlView;
                }
            }

            return results;
        }


        public async Task GoToDetailView(Result item)
        {
            var parameter = new SearchResult[2];
            for (int i = 0; i < 2; i++)
            {
                parameter[i] = new SearchResult();
                parameter[i].D = new D();
            }
            Result[] tmpResults = { new Result() };
            parameter[0].D.Results = tmpResults;
            parameter[0].D.Results[0] = item;
            parameter[1].D.Results = tmpResults;
            parameter[1].D.Results = this.Results;
            SearchOptions searchOptions = new SearchOptions();
            searchOptions.Query = new SearchOptionsDetails()
            {
                ScenarioCode = this.eventsScenarioCode,
                Page = this.page,
               
            };
            var tempo = new SearchDetailsParameters()
            {
                parameter = parameter,
                searchOptions = searchOptions,
                nbrResults = this.NbrResults.ToString()
            };
            await this.navigationService.Navigate<SearchDetailsViewModel, SearchDetailsParameters>(tempo);
        }

        private async Task PerformSearch(string search)
        {
            var options = new SearchOptionsDetails()
            {
                QueryString = search
            };
            SearchOptions opt = new SearchOptions() { Query = options };
            if (App.AppState.NetworkConnection)
            {
                await this.navigationService.Navigate<SearchViewModel, SearchOptions>(opt);
            }
            else
            {
                this.DisplayAlert(ApplicationResource.Warning, ApplicationResource.NetworkDisable, ApplicationResource.ButtonValidation);
            }

        }

        private void SetFilter(MaterialMenuResult filter)
        {
            if (this.filterIndex == filter.Index)
                return;
            if (this.Results == null)
                return;
            this.filterIndex = filter.Index;

            
        }

        private async Task OpenResultDetails(SearchResult result)
        {
            var parameter = new SearchResult[2];
            parameter[0] = result;
            parameter[1] = new SearchResult();
            parameter[1].D = new D();
            parameter[1].D.Results = this.Results;
            SearchOptions searchOptions = new SearchOptions();
            searchOptions.Query = new SearchOptionsDetails()
            {
                ScenarioCode = this.eventsScenarioCode,
                Page = this.page,
            };
            var tempo = new SearchDetailsParameters()
            {
                parameter = parameter,
                searchOptions = searchOptions,
                nbrResults = this.NbrResults.ToString()
            };
            await this.navigationService.Navigate<SearchDetailsViewModel, SearchDetailsParameters>(tempo);
        }

        private async Task GetRedirectURL()
        {
            var defaultUrlTempo = this.IsEvent ? "https://graphisme-syracuse.archimed.fr/basicfilesdownload.ashx?itemGuid=05E01B10-51AE-4EDF-AEF2-64E696038A71" : "https://user-images.githubusercontent.com/24848110/33519396-7e56363c-d79d-11e7-969b-09782f5ccbab.png";
            foreach (var search in this.Results)
            {
                if (search.FieldList.ThumbMedium != null && search.FieldList.ThumbMedium[0] != null)
                    search.FieldList.ThumbMedium[0] = new Uri(await this.requestService.GetRedirectURL(search.FieldList.ThumbMedium[0].ToString(), defaultUrlTempo)) ;
                else if (search.FieldList.ThumbSmall != null && search.FieldList.ThumbSmall[0] != null)
                    search.FieldList.ThumbSmall[0] = new Uri(await this.requestService.GetRedirectURL(search.FieldList.ThumbSmall[0].ToString(), defaultUrlTempo));
                else
                {
                    Debug.Write("Invalid object. ");
                }

            }
            await this.RaiseAllPropertiesChanged();
        }

        private void ForceListUpdate()
        {
            var tempo = this.Results;
            this.Results = new Result[0];
            this.Results = tempo;
        }

    }

}
