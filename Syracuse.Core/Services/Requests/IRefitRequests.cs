﻿using Refit;
using Syracuse.Mobitheque.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syracuse.Mobitheque.Core.Services.Requests
{
    public interface IRefitRequests
    {
        [Post("/logon.svc/logon")]
        Task<T> Authentication<T>([Body(BodySerializationMethod.UrlEncoded)]Dictionary<string, object> data);

        [Post("/logon.svc/GetUrlWithAuthenticationTransfert")]
        Task<T> GetUrlWithAuthenticationTransfert<T>([Body]UrlWithAuthenticationTransfertOptions body);

        [Get("/Portal/UserAccountService.svc/RetrieveAccountSummary?serviceCode{code}&userUniqueIdentifier={uniqueId}&token={token}")]
        Task<T> GetSummary<T>([AliasAs("token")]string token,
                              [AliasAs("code")]string code = "",
                              [AliasAs("uniqueId")]string uniqueId = "");

        [Get("/DigitalCollectionService.svc/ListDigitalDocuments?parentDocumentId={parentDocumentId}&start={start}&limit={limit}&includeMetaDatas={includeMetaDatas}")]
        Task<T> GetListDigitalDocuments<T>( [AliasAs("parentDocumentId")] string parentDocumentId,
                                            [AliasAs("start")] int start = 0,
                                            [AliasAs("limit")] int limit = 10,
                                            [AliasAs("includeMetaDatas")] bool includeMetaDatas = false);

        [Get("/digitalCollection/DigitalCollectionAttachmentDownloadHandler.ashx?parentDocumentId={parentDocumentId}&documentId={documentId}&skipWatermark={skipWatermark}&skipCopyright={skipCopyright}")]
        Task<T> GetDownloadDocument<T>([AliasAs("parentDocumentId")] string parentDocumentId,
                                    [AliasAs("documentId")] string documentId,
                                    [AliasAs("skipWatermark")] bool skipWatermark = true,
                                    [AliasAs("skipCopyright")] bool skipCopyright = true);

        [Get("/Portal/UserAccountService.svc/ListLoans?serviceCode{code}&userUniqueIdentifier={uniqueId}&token={token}&timestamp={timestamp}")]
        Task<T> GetLoans<T>([AliasAs("timestamp")] string timestamp,
                            [AliasAs("token")] string token,
                            [AliasAs("code")]string code = "",
                            [AliasAs("uniqueId")]string uniqueId = "");

        [Get("/Portal/Services/UserAccountService.svc/ListBookings?serviceCode{code}&userUniqueIdentifier={uniqueId}&token={token}&timestamp={timestamp}")]
        Task<T> GetBookings<T>([AliasAs("timestamp")] string timestamp,
                               [AliasAs("token")] string token,
                               [AliasAs("code")]string code = "",
                               [AliasAs("uniqueId")]string uniqueId = "");

        [Get("/Portal/Services/UserAccountService.svc/ListHandings?serviceCode{code}&userUniqueIdentifier={uniqueId}&token={token}")]
        Task<T> GetHandlings<T>([AliasAs("token")] string token,
                                [AliasAs("code")]string code = "",
                                [AliasAs("uniqueId")]string uniqueId = "");

        [Get("/Portal/Services/UserAccountService.svc/ListUserDemands?serviceCode{code}&userUniqueIdentifier={uniqueId}&token={token}")]
        Task<T> GetUserDemands<T>([AliasAs("token")] string token,
                [AliasAs("code")] string code = "",
                [AliasAs("uniqueId")] string uniqueId = "");

        [Get("/Portal/Services/UserAccountService.svc/SetMessageAsValidated?messageId={messageId}&token={token}")]
        Task<T> SetMessageAsValidated<T>(
        [AliasAs("messageId")] int messageId,
        [AliasAs("token")] string token);


        [Post("/Portal/Services/UserAccountService.svc/AnswerDemand")]
        Task<T> AnswerDemand<T>([Body] DemandsOptions body);

        [Post("/Portal/Recherche/Search.svc/Search")]
        Task<T> Search<T>([Body] SearchOptions body);

        [Post("/Portal/Services/ILSClient.svc/CheckAvailability")]
        Task<T> CheckAvailability<T>([Body] CheckAvailabilityOptions body);

        [Post("/Portal/Recherche/Search.svc/SearchUserBasket")]
        Task<T> SearchUserBasket<T>([Body] BasketOptions body);

        [Post("/Portal/ILSClient.svc/GetHoldings")]
        Task<T> SearchLibrary<T>([Body]SearchLibraryOptions body);

        [Post("/Portal/ILSClient.svc/PlaceReservation")]
        Task<T> PlaceReservation<T>([Body]PlaceReservationOptions body);

        [Post("/Portal/Services/UserAccountService.svc/RenewLoans")]
        Task<T> RenewLoans<T>([Body]LoanOptions body);

        [Post("/Portal/Services/UserAccountService.svc/CancelBookings")]
        Task<T> CancelBooking<T>([Body]BookingOptions body);


    }
}
