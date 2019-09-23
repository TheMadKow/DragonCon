using System;
using System.Collections.Generic;
using System.Text;

namespace DragonCon.Features.Shared
{
    public interface IDisplayPagination
    {
        void SetRouteValue(string key, string value);
        void SetServerActions(string area, string controller, string action);

        string Area { get; }
        string Controller { get; }
        string Action { get;  }
        string AdditionalRouteValuesString { get; }

        int CurrentPage { get; }
        int TotalPages { get; }
        int ResultsPerPage { get; }
        int SkipCount { get; }
    }
}
