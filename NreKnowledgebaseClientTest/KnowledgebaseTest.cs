using System.Collections.Generic;
using System.Threading;
using NreKnowledgebase;
using NreKnowledgebase.SchemaV5;
using NSubstitute;
using Serilog;
using Xunit;

namespace NreKnowledgebaseClient.Test
{
    public class KnowledgebaseTest
    {
        private Knowledgebase _knowledgebase = new Knowledgebase(
            new FileSource(TestFiles.SourceFiles, Substitute.For<ILogger>()),
            Substitute.For<ILogger>());

        private IKnowledgebase Knowledgebase => _knowledgebase;
        private IKnowledgebaseAsync KnowledgebaseAsync => _knowledgebase;

        [Fact]
        public void TicketTypes()
        {
            Assert.NotNull(Knowledgebase.TicketTypes);
        }
        
        [Fact]
        public void TicketRestrictions()
        {
            Assert.NotNull(Knowledgebase.TicketRestrictions);
        }
        
        [Fact]
        public void Promotions()
        {
            Assert.NotNull(Knowledgebase.Promotions);
        }
        
        [Fact]
        public void Incidents()
        {
            Assert.NotNull(Knowledgebase.Incidents);
        }
        
        [Fact]
        public void ServiceIndicators()
        {
            Assert.NotNull(Knowledgebase.TocServiceIndicators);
        }
        
        [Fact]
        public void Stations()
        {
            Assert.NotNull(Knowledgebase.Stations);
        }
        
        [Fact]
        public void Tocs()
        {
            Assert.NotNull(Knowledgebase.Tocs);
        }

        [Fact]
        public async void GetTicketTypes()
        {
            Assert.NotNull(await KnowledgebaseAsync.GetTicketTypes(CancellationToken.None));
        }
        
        [Fact]
        public async void GetTicketRestrictions()
        {
            Assert.NotNull(await KnowledgebaseAsync.GetTicketRestrictions(CancellationToken.None));
        }
        
        [Fact]
        public async void GetPromotions()
        {
            Assert.NotNull(await KnowledgebaseAsync.GetPromotions(CancellationToken.None));
        }
        
        [Fact]
        public async void GetIncidents()
        {
            Assert.NotNull(await KnowledgebaseAsync.GetIncidents(CancellationToken.None));
        }
        
        [Fact]
        public async void GetServiceIndicators()
        {
            Assert.NotNull(await KnowledgebaseAsync.GetTocServiceIndicators(CancellationToken.None));
        }
        
        [Fact]
        public async void GetStations()
        {
            Assert.NotNull(await KnowledgebaseAsync.GetStations(CancellationToken.None));
        }
        
        [Fact]
        public async void GetTocs()
        {
            Assert.NotNull(await KnowledgebaseAsync.GetTocs(CancellationToken.None));
        }
        
        [Fact]
        public async void GetTocsFullCode()
        {
            var logger = Substitute.For<ILogger>();
            var sourceFiles = new Dictionary<KnowedgebaseSubjects, string>()
                {
                    { KnowedgebaseSubjects.TicketTypes, "Data/TicketTypes.xml" },
                    { KnowedgebaseSubjects.TicketRestrictions, "Data/TicketRestrictions.xml" },
                    { KnowedgebaseSubjects.Promotions, "Data/Promotions.xml" },
                    { KnowedgebaseSubjects.Incidents, "Data/Incidents.xml" },
                    { KnowedgebaseSubjects.TocServiceIndicators, "Data/ServiceIndicators.xml" },
                    { KnowedgebaseSubjects.Stations, "Data/Stations.xml" },
                    { KnowedgebaseSubjects.Tocs, "Data/Tocs.xml" },
                };

            var knowledgebase = new Knowledgebase(new FileSource(sourceFiles, logger), logger);

            var tocs = await knowledgebase.GetTocs(CancellationToken.None);
            Assert.NotNull(tocs);
        }
        
        [Fact]
        public async void SourceFailsThrowsKnowledgebaseException()
        {
            var knowledgebase = new Knowledgebase(
                new FileSource(TestFiles.TocFileOnly, Substitute.For<ILogger>()),
                Substitute.For<ILogger>());

            var ex = await Assert.ThrowsAsync<KnowledgebaseException>(
                () => knowledgebase.GetSubjectAsync<Incidents>(KnowedgebaseSubjects.Incidents, CancellationToken.None));
            Assert.Equal("Incidents file not configured", ex.Message);
        }
        
        [Fact]
        public async void BadXmlThrowsKnowledgebaseException()
        {
            var ex = await Assert.ThrowsAsync<KnowledgebaseException>(
                () => _knowledgebase.GetSubjectAsync<Incidents>(KnowedgebaseSubjects.Tocs, CancellationToken.None));
            Assert.Equal("Error when deserialising xml for Tocs", ex.Message);           
        }
    }
}