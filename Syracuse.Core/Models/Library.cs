﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace Syracuse.Mobitheque.Core.Models
{
    public class Press
    {
        [JsonProperty("query")]
        public string Query { get; set; }
        [JsonProperty("pressUri")]
        public string PressUri { get; set; }
        [JsonProperty("press_scenario_code")]
        public string PressScenarioCode { get; set; }
        [JsonProperty("name")]
        public string PressName { get; set; }
    }

    public class ConfigLibrary
    {
        [JsonProperty("baseUri")]
        public string BaseUri { get; set; }
        [JsonProperty("domainUri")]
        public string DomainUri { get; set; }
        [JsonProperty("forgetMdpUri")]
        public string ForgetMdpUri { get; set; }
        [JsonProperty("search_scenario_code")]
        public string SearchScenarioCode { get; set; }
        [JsonProperty("events_scenario_code")]
        public string EventsScenarioCode { get; set; }
        [JsonProperty("is_event")]
        public bool IsEvent { get; set; }
        [JsonProperty("remember_me")]
        public bool RememberMe { get; set; }
        [JsonProperty("is_km")]
        public bool IsKm { get; set; }
        [JsonProperty("standards_views")]
        public List<StandardsViews> StandardsViews { get; set; }
        [JsonProperty("sso")]
        public List<SSO> ListSSO { get; set; }
        [JsonProperty("can_download")]
        public bool CanDownload { get; set; }
        [JsonProperty("library_informations")]
        public List<LibraryInformations> BuildingInformations { get; set; }
    }

    public class StandardsViews
    {
        [JsonProperty("view_name")]
        public string ViewName { get; set; }
        [JsonProperty("view_icone")]
        public string ViewIcone { get; set; }
        [JsonProperty("view_query")]
        public string ViewQuery { get; set; }
        [JsonProperty("view_scenario_code")]
        public string ViewScenarioCode { get; set; }
    }
    public class SSO
    {
        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }
    public class Library
    {
        [JsonProperty("department_code")]
        public string DepartmentCode { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("config")]
        public ConfigLibrary Config { get; set; }
    }
    public class LibraryInformations
    {
        [JsonProperty("building_name")]
        public string BuildingName { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        public bool DisplayNavigationError { get; set; } = false;
    }
}
