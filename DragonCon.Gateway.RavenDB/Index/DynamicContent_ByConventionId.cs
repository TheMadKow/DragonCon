using System.Linq;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.UserDisplay;
using Raven.Client.Documents.Indexes;

namespace DragonCon.RavenDB.Index
{
    public class DynamicContent_ByConventionId : AbstractMultiMapIndexCreationTask<DynamicContent_ByConventionId.Result>
    {
        public class Result
        {
            public string ConventionId { get; set; } = string.Empty;
            public string[] Updates { get; set; } = new string[0];
            public string[] Sponsors { get; set; } = new string[0];
            public string[] Slides { get; set; } = new string[0];
            public string[] Day { get; set; } = new string[0];
            public string[] Location { get; set; } = new string[0];
            public string[] English { get; set; } = new string[0];
        }

        public DynamicContent_ByConventionId()
        {
            AddMap<DynamicUpdateItem>(updates =>
                            from u in updates
                            select new
                            {
                                ConventionId = u.ConventionId,
                                English = new string[0],
                                Location = new string[0],
                                Day = new string[0],
                                Sponsors = new string[0],
                                Slides = new string[0],
                                Updates = new[] { u.Id },
                            });
            AddMap<DynamicSponsorItem>(sponsors =>
                from s in sponsors
                select new
                {
                    ConventionId = s.ConventionId,
                    Sponsors = new[] { s.Id },
                    English = new string[0],
                    Location = new string[0],
                    Day = new string[0],
                    Slides = new string[0],
                    Updates = new string[0],
                });

            AddMap<DynamicSlideItem>(slides =>
                from s in slides
                select new
                {
                    ConventionId = s.ConventionId,
                    Sponsors = new string[0],
                    Location = new string[0],
                    Day = new string[0],
                    Slides = new string[] {s.Id},
                    English = new string[0],
                    Updates = new string[0],
                });


            AddMap<DynamicEnglish>(slides =>
                from eng in slides
                select new
                {
                    ConventionId = eng.ConventionId,
                    Sponsors = new string[0],
                    Slides = new string[0],
                    Location = new string[0],
                    Day = new string[0],
                    English = new[] { eng.Id },
                    Updates = new string[0],
                });

            AddMap<DynamicDays>(slides =>
                from day in slides
                select new
                {
                    ConventionId = day.ConventionId,
                    Sponsors = new string[0],
                    Slides = new string[0],
                    Location = new string[0],
                    English = new string[0],
                    Day = new[] { day.Id },
                    Updates = new string[0],
                });


            AddMap<DynamicLocation>(slides =>
                from loc in slides
                select new
                {
                    ConventionId = loc.ConventionId,
                    Sponsors = new string[0],
                    Slides = new string[0],
                    Day = new string[0],
                    English = new string[0],
                    Location = new[] { loc.Id },
                    Updates = new string[0],
                });




            Reduce = results => from result in results
                                group result by result.ConventionId
                                into g
                                let first = g.FirstOrDefault()
                                select new
                                {
                                    ConventionId = first.ConventionId,
                                    Updates = g.SelectMany(x => x.Updates),
                                    Slides = g.SelectMany(x => x.Slides),
                                    Sponsors = g.SelectMany(x => x.Sponsors),
                                    English = g.SelectMany(x => x.English),
                                    Location = g.SelectMany(x => x.Location),
                                    Day = g.SelectMany(x => x.Day),
                                };

        }
    }
}
