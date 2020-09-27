using System.IO;

namespace NreKnowledgebaseClient.Test
{
    
    public static class TestFiles
    {
        public static string Fares {
            get => GetPath("fares.xml");
        }
        public static string FareRestrictions {
            get => GetPath("fare_restrictions.xml");
        }
        public static string Incidents {
            get => GetPath("incidents.xml");
        }
        public static string Promotions {
            get => GetPath("promotions.xml");
        }
        public static string ServiceIndicators {
            get => GetPath("service_indicators.xml");
        }
        public static string Stations {
            get => GetPath("stations.xml");
        }
        public static string Tocs
        {
            get => GetPath("tocs.xml");
        }

        private static string GetPath(string file) => Path.Combine(".", "Data", file);
    }
}