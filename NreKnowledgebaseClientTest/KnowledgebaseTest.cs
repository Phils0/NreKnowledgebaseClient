using System.Collections.Generic;
using NreKnowledgebase;
using NSubstitute;
using Serilog;
using Xunit;

namespace NreKnowledgebaseClient.Test
{
    public class KnowledgebaseTest
    {
        private IKnowledgebase _knowledgebase = new NreKnowledgebase.Knowledgebase(
            new FileSource(TestFiles.SourceFiles, Substitute.For<ILogger>()),
            Substitute.For<ILogger>());

        [Fact]
        public void TicketTypes()
        {
            Assert.NotNull(_knowledgebase.TicketTypes);
        }
        
        [Fact]
        public void TicketRestrictions()
        {
            Assert.NotNull(_knowledgebase.TicketRestrictions);
        }
        
        [Fact]
        public void Promotions()
        {
            Assert.NotNull(_knowledgebase.Promotions);
        }
        
        [Fact]
        public void Incidents()
        {
            Assert.NotNull(_knowledgebase.Incidents);
        }
        
        [Fact]
        public void ServiceIndicators()
        {
            Assert.NotNull(_knowledgebase.TocServiceIndicators);
        }
        
        [Fact]
        public void Stations()
        {
            Assert.NotNull(_knowledgebase.Stations);
        }
        
        [Fact]
        public void Tocs()
        {
            Assert.NotNull(_knowledgebase.Tocs);
        }

        [Fact]
        public void SourceFailsThrowsKnowledgebaseException()
        {
            var knowledgebase = new NreKnowledgebase.Knowledgebase(
                new FileSource(TestFiles.TocFileOnly, Substitute.For<ILogger>()),
                Substitute.For<ILogger>());

            var ex = Assert.Throws<KnowledgebaseException>(() => knowledgebase.Incidents);
            Assert.Equal("Incidents file not configured", ex.Message);
        }
        
        [Fact]
        public void BadXmlThrowsKnowledgebaseException()
        {
            var knowledgebase = new NreKnowledgebase.Knowledgebase(
                new FileSource(new Dictionary<KnowedgebaseSubjects, string>()
                {
                    { KnowedgebaseSubjects.Tocs, TestFiles.TicketTypes },
                }, Substitute.For<ILogger>()),
                Substitute.For<ILogger>());

            var ex = Assert.Throws<KnowledgebaseException>(() => knowledgebase.Tocs);
            Assert.Equal("Error when deserialising xml for Tocs", ex.Message);           
        }
    }
}