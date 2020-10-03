using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using NreKnowledgebase;
using NSubstitute;
using Serilog;
using Xunit;

namespace NreKnowledgebaseClient.Test
{
    public class FileSourceTest
    {
        private IKnowledgebaseSource _client = new FileSource(TestFiles.SourceFiles, Substitute.For<ILogger>());

        [Theory]
        [InlineData(KnowedgebaseSubjects.TicketType, "TicketTypeDescriptionList")]
        [InlineData(KnowedgebaseSubjects.TicketRestrictions, "TicketRestrictions")]
        [InlineData(KnowedgebaseSubjects.Promotions, "PromotionList")]
        [InlineData(KnowedgebaseSubjects.Incidents, "Incidents")]
        [InlineData(KnowedgebaseSubjects.TocServiceIndicators, "NSI")]
        [InlineData(KnowedgebaseSubjects.Stations, "StationList")]
        [InlineData(KnowedgebaseSubjects.Tocs, "TrainOperatingCompanyList")]
        public async void GetXml(KnowedgebaseSubjects subject, string expectedRoot)
        {
            using (var reader =
                await _client.GetKnowledgebaseXml(subject, CancellationToken.None))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        Assert.Equal(expectedRoot, reader.Name);
                        break;
                    }
                }
            }
        }
        
        [Fact]
        public async void FileNotConfiguredThrowsKnowledgebaseException()
        {
            var source = new FileSource(TestFiles.TocFileOnly, Substitute.For<ILogger>());

            var ex = await Assert.ThrowsAsync<KnowledgebaseException>(() => source.GetKnowledgebaseXml(KnowedgebaseSubjects.Incidents, CancellationToken.None));
            Assert.Equal("Incidents file not configured", ex.Message);
        }
        
        [Fact]
        public async void FileNotExistsThrowsKnowledgebaseException()
        {
            var source = new FileSource(new Dictionary<KnowedgebaseSubjects, string>()
            {
                { KnowedgebaseSubjects.Tocs, "Invalid.xml" },
            }, Substitute.For<ILogger>());

            var ex = await Assert.ThrowsAsync<KnowledgebaseException>(() => source.GetKnowledgebaseXml(KnowedgebaseSubjects.Tocs, CancellationToken.None));
            Assert.Equal("Tocs file does not exist: Invalid.xml", ex.Message);
        }
    }
}