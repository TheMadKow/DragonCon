using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DragonCon.Features.Shared
{
    public class DisplayPagination : IDisplayPagination
    {
        private static int DefaultPageSize { get; set; } = 30;

        public static IDisplayPagination BuildForGateway(int page, int perPage)
        {
            return new DisplayPagination(page, perPage);
        }

        public static IDisplayPagination BuildForView(int totalItems, int skippedResult, int resPerPage)
        {
            return new DisplayPagination(totalItems, skippedResult, resPerPage);
        }

        private DisplayPagination(int page, int perPage)
        {
            ResultsPerPage = perPage <= 0
                ? ResultsPerPage = DefaultPageSize
                : perPage;
            CurrentPage = page <= 0 ? 0 : page;
        }
        
        private DisplayPagination(int totalItems, int skippedResult, int resPerPage)
        {
            CurrentPage = (skippedResult / resPerPage);
            ResultsPerPage = resPerPage;

            TotalPages = totalItems / resPerPage;
            if (totalItems % resPerPage > 0)
                TotalPages++;

            CurrentPage = Math.Min(CurrentPage, TotalPages - 1);
        }

        [JsonProperty("page")]
        public int CurrentPage { get; set; }
        [JsonProperty("totalPages")]
        public int TotalPages { get; set; }
        [JsonProperty("resPerPage")]
        public int ResultsPerPage { get; set; }
        [JsonProperty("skipCount")]
        public int SkipCount => ResultsPerPage * CurrentPage;

        #region Server Actions
        public void SetServerActions(string area, string controller, string action)
        {
            Area = area;
            Controller = controller;
            Action = action;
        }

        public string Area { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }

        public void SetRouteValue(string key, string value)
        {
            AdditionalRoutes[key] = value;
        }

        public Dictionary<string, string> AdditionalRoutes { get; set; } = new Dictionary<string, string>();
        public string AdditionalRouteValuesString
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var route in AdditionalRoutes)
                {
                    sb.Append($"&{route.Key}={route.Value}");
                }

                return sb.ToString();
            }
        }
        #endregion
    }
}