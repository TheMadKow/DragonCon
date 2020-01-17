using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DragonCon.Features.Shared
{
    public class Select2Option
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("text")] public string Text { get; set; }
        [JsonProperty("brackets")] public string Brackets { get; set; }
    }

    public class Select2Pagination
    {
        [JsonProperty("more")] public bool More { get; set; }
    }

    public class Select2ViewModel
    {
        [JsonProperty("results")] public List<Select2Option> Results = new List<Select2Option>();

        [JsonProperty("pagination")]
        public Select2Pagination Pagination { get; set; } = new Select2Pagination();
    }
}
