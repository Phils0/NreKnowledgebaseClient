using System;
using System.Net.Http;
using System.Threading;
using NreKnowledgebase;
using NSubstitute;
using Serilog;
using Xunit;

namespace NreKnowledgebaseClient.Test
{
    public class LiveKnowledgebaseTest
    {
        private string user = Environment.GetEnvironmentVariable("NRE_USER");
        private string password = Environment.GetEnvironmentVariable("NRE_PASSWORD");
        
        [Fact(Skip = "Uses live NRE site")]
        public async void GetTocs()
        {
            var logger = Substitute.For<ILogger>();
            using (var source = new NationalRailEnquiriesSource(new HttpClient(), logger))
            {
                await source.Initiate(user, password, CancellationToken.None);
                var knowledgebase = new Knowledgebase(source, logger);
                var tocs = await knowledgebase.GetTocs(CancellationToken.None);
                Assert.NotEmpty(tocs.TrainOperatingCompany);
            }            
        }
        
        [Theory(Skip = "Uses live NRE site")]
        [InlineData(KnowedgebaseSubjects.TicketTypes, "TicketTypeDescriptionList")]
        [InlineData(KnowedgebaseSubjects.TicketRestrictions, "TicketRestrictions")]
        [InlineData(KnowedgebaseSubjects.Promotions, "PromotionList")]
        [InlineData(KnowedgebaseSubjects.Incidents, "Incidents")]
        [InlineData(KnowedgebaseSubjects.TocServiceIndicators, "NSI")]
        [InlineData(KnowedgebaseSubjects.Stations, "StationList")]
        [InlineData(KnowedgebaseSubjects.Tocs, "TrainOperatingCompanyList")]
        public async void GetXml(KnowedgebaseSubjects subject, string expectedRoot)
        {
            using (var client = new NationalRailEnquiriesSource(new HttpClient(), Substitute.For<ILogger>()))
            {
                await client.Initiate(user, password, CancellationToken.None);

                using (var reader =
                    await client.GetKnowledgebaseXml(subject, CancellationToken.None))
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
        }
    }
}