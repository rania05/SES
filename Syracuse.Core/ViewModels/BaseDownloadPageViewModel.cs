﻿using MvvmCross.Commands;
using Newtonsoft.Json;
using Syracuse.Mobitheque.Core.Models;
using Syracuse.Mobitheque.Core.Services.Requests;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Syracuse.Mobitheque.Core.ViewModels
{
    public abstract class BaseDownloadPageViewModel : BaseViewModel
    {
        public CookiesSave user = null ;
        public bool IsDownloadAllDisplay { get; set; } = false;
        protected BaseDownloadPageViewModel()
        { }

        internal async Task<Result[]> HaveDownloadOption(Result[] results, IRequestService requestService)
        {
            if (this.user == null)
            {
                this.user = await App.Database.GetActiveUser();
            }
            if (this.user.CanDownload)
            {
                foreach (var result in results)
                {
                    if (result.HasViewerDr)
                    {   
                        var status = await requestService.GetListDigitalDocuments(result.Resource.RscId);
                        if (status.Success)
                        {
                            if (status.D.Documents.Count >= 1)
                            {
                                if (status.D.Documents[0].canDownload)
                                {
                                    result.CanDownload = true;
                                    result.downloadOptions.parentDocumentId = status.D.Documents[0].parentDocumentId;
                                    result.downloadOptions.documentId = status.D.Documents[0].documentId;
                                    result.downloadOptions.fileName = status.D.Documents[0].fileName;
                                    if (await App.DocDatabase.GetDocumentsByDocumentID(result.Resource.RscId) != null)
                                    {
                                        result.CanDownload = false;
                                        result.IsDownload = true;
                                    }
                                    if (result.CanDownload)
                                    {
                                        this.IsDownloadAllDisplay = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return results;
        }

        internal async Task SaveNewDocumentDatabaseObject(Result result, string documentPath)
        {
            CookiesSave user = await App.Database.GetActiveUser();
            var json = JsonConvert.SerializeObject(result);
            DocumentSave b = await App.DocDatabase.GetDocumentsByDocumentID(result.Resource.RscId);
            if (b == null)
            {
                b = new DocumentSave();
            }
            b.UserID = user.ID;
            b.JsonValue = json;
            b.DocumentID = result.Resource.RscId;
            b.ImagePath = result.FieldList.Image;
            b.Title = result.FieldList.Title[0];
            b.ShortDesc = result.FieldList.shortDesc;
            b.DocumentPath = documentPath;
            await App.DocDatabase.SaveItemAsync(b);

        }

    }
    public abstract class BaseDownloadPageViewModel<TParameter, TResult> : BaseViewModel<TParameter, TResult>
    where TParameter : class
    where TResult : class
    {
        public delegate void DisplayAlertDelegate(string title, string message, string button);
        public delegate Task<bool> DisplayAlertDelegateMult(string title, string message, string buttonYes, string buttonNo);
        public event DisplayAlertDelegate OnDisplayAlert;
        public event DisplayAlertDelegateMult OnDisplayAlertMult;

        protected BaseDownloadPageViewModel()
        { }

        public CookiesSave user = null;
        public bool IsDownloadAllDisplay { get; set; } = false;

        internal async Task<Result[]> HaveDownloadOption(Result[] results, IRequestService requestService)
        {
            if (this.user == null)
            {
                this.user = await App.Database.GetActiveUser();
            }
            if (this.user.CanDownload)
            {
                foreach (var result in results)
                {
                    if (result.HasViewerDr)
                    {
                        var status = await requestService.GetListDigitalDocuments(result.Resource.RscId);
                        if (status.Success)
                        {
                            if (status.D.Documents.Count >= 1)
                            {
                                if (status.D.Documents[0].canDownload)
                                {
                                    result.CanDownload = true;
                                    result.downloadOptions.parentDocumentId = status.D.Documents[0].parentDocumentId;
                                    result.downloadOptions.documentId = status.D.Documents[0].documentId;
                                    result.downloadOptions.fileName = status.D.Documents[0].fileName;
                                    if (await App.DocDatabase.GetDocumentsByDocumentID(result.Resource.RscId) != null)
                                    {
                                        result.CanDownload = false;
                                        result.IsDownload = true;
                                    }
                                    if (result.CanDownload)
                                    {
                                        this.IsDownloadAllDisplay = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return results;
        }

        internal async Task SaveNewDocumentDatabaseObject(Result result, string documentPath)
        {
            CookiesSave user = await App.Database.GetActiveUser();
            var json = JsonConvert.SerializeObject(result);
            DocumentSave b = await App.DocDatabase.GetDocumentsByDocumentID(result.Resource.RscId);
            if (b == null)
            {
                b = new DocumentSave();
            }
            b.UserID = user.ID;
            b.JsonValue = json;
            b.DocumentID = result.Resource.RscId;
            b.ImagePath = result.FieldList.Image;
            b.Title = result.FieldList.Title[0];
            b.ShortDesc = result.FieldList.shortDesc;
            b.DocumentPath = documentPath;
            await App.DocDatabase.SaveItemAsync(b);

        }
    }
}
