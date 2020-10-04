using System;
using System.Collections.Generic;
using System.IO;
using NreKnowledgebase;

namespace NreKnowledgebaseClient.Test
{
    public static class TestFiles
    {
        public static string TicketTypes => GetPath("fares.xml");
        public static string TicketRestrictions => GetPath("fare_restrictions.xml");
        public static string Incidents => GetPath("incidents.xml");
        public static string Promotions => GetPath("promotions.xml");
        public static string ServiceIndicators => GetPath("service_indicators.xml");
        public static string Stations => GetPath("stations.xml");
        public static string Tocs => GetPath("tocs.xml");

        private static string GetPath(string file) => Path.Combine(".", "Data", file);
        
        public static Dictionary<KnowedgebaseSubjects, string> SourceFiles =>
            new Dictionary<KnowedgebaseSubjects, string>()
            {
                { KnowedgebaseSubjects.TicketTypes, TicketTypes },
                { KnowedgebaseSubjects.TicketRestrictions, TicketRestrictions },
                { KnowedgebaseSubjects.Promotions, Promotions },
                { KnowedgebaseSubjects.Incidents, Incidents },
                { KnowedgebaseSubjects.TocServiceIndicators, ServiceIndicators },
                { KnowedgebaseSubjects.Stations, Stations },
                { KnowedgebaseSubjects.Tocs, Tocs },
            };
        
        public static Dictionary<KnowedgebaseSubjects, string> TocFileOnly =>
            new Dictionary<KnowedgebaseSubjects, string>()
            {
                { KnowedgebaseSubjects.Tocs, Tocs },
            };
    }
}