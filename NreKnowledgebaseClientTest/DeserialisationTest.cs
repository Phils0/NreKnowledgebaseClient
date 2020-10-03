using System;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using NreKnowledgebase.SchemaV4;
using NreKnowledgebase.SchemaV5;
using Xunit;

namespace NreKnowledgebaseClient.Test
{
    public abstract class DeserialisationTest<T> where T : class
    {
        public abstract string File { get; }

        protected T List { get; }
        
        protected DeserialisationTest()
        {
            using var reader = new XmlTextReader(File);
            var serialiser = new XmlSerializer(typeof(T));
            List = serialiser.Deserialize(reader) as T;           
        }

        [Fact]
        public void Deserialise()
        {

            Assert.NotNull(List);
        }
    }
    
    public class TicketTypesTest : DeserialisationTest<TicketTypeDescriptionList>
    {
        public override string File => TestFiles.TicketTypes;

        [Fact]
        public void DeserialisedClass()
        {
            var classes = List.TicketTypeDescription.Select(t => t.Class).Distinct().ToArray();
            
            Assert.Equal(3, classes.Length);
            Assert.Contains("First", classes);
            Assert.Contains("Standard", classes);
            Assert.Contains("N/A", classes);
        }
    }
    
    public class TicketRestrictionsTest : DeserialisationTest<TicketRestrictions>
    {
        public override string File => TestFiles.TicketRestrictions;
    }
    
    public class PromotionsTest : DeserialisationTest<PromotionList>
    {
        public override string File => TestFiles.Promotions;
    }
    
    public class IncidentsTest : DeserialisationTest<Incidents>
    {
        public override string File => TestFiles.Incidents;
    }
    
    public class SerivceIndicatorsTest : DeserialisationTest<NationalServiceIndicatorListStructure>
    {
        public override string File => TestFiles.ServiceIndicators;
    }
    
    public class StationsTest : DeserialisationTest<StationList>
    {
        public override string File => TestFiles.Stations;
    }
    
    public class TocsTest : DeserialisationTest<TrainOperatingCompanyList>
    {
        public override string File => TestFiles.Tocs;
    }
}