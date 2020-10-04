using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using NreKnowledgebase;
using NSubstitute;
using Serilog;
using Xunit;

namespace NreKnowledgebaseClient.Test
{
    public class NationalRailEnquiriesSourceTest
    {
        private string TestUser => "testuser";
        private string TestPassword = "testpassword";
        
        private string SuccessResponse => @"{
    ""username"": ""testuser"",
    ""roles"": {
        ""ROLE_DARWIN"": true,
        ""ROLE_STANDARD"": true,
        ""ROLE_KB_API"": true,
       ""ROLE_DTD"": true
    },
    ""token"": ""testuser:1601637685000:xAN06yHWAL2P9Tzud2S3piJN2701IDqz1gUpbmMM3CQ=""
    }";
        
        private string InvalidResponse => @"{
    ""error"": ""Invalid username/password""
    }";
        
        [Fact]
        public async void UserPasswordInInitiateRequest()
        {
            var handler = new MockHttpMessageHandler(SuccessResponse, HttpStatusCode.OK);
            var client = new NationalRailEnquiriesSource(new HttpClient(handler), Substitute.For<ILogger>());
            await client.Initiate(TestUser, TestPassword, CancellationToken.None);
            
            Assert.True(client.IsInititated);
        }
        
        [Fact]
        public async void BadUserOrPassword()
        {
            var handler = new MockHttpMessageHandler(InvalidResponse, HttpStatusCode.Unauthorized);
            var client = new NationalRailEnquiriesSource(new HttpClient(handler), Substitute.For<ILogger>());
            
            var ex = await Assert.ThrowsAsync<KnowledgebaseException>(() => client.Initiate(TestUser, TestPassword, CancellationToken.None));
            Assert.Contains("Invalid username/password", ex.Message);;
        }
        
        [Fact]
        public async void NreErrorWhenInitialising()
        {
            var handler = new MockHttpMessageHandler("Server error", HttpStatusCode.InternalServerError);
            var client = new NationalRailEnquiriesSource(new HttpClient(handler), Substitute.For<ILogger>());
            
            var ex = await Assert.ThrowsAsync<KnowledgebaseException>(() => client.Initiate(TestUser, TestPassword, CancellationToken.None));
            Assert.Equal("Authentication error. Response: Server error", ex.Message);;
        }
        
        [Fact]
        public async void TokenCorrectlySet()
        {
            var handler = new MockHttpMessageHandler(SuccessResponse, HttpStatusCode.OK);
            var client = new NationalRailEnquiriesSource(new HttpClient(handler), Substitute.For<ILogger>());
            await client.Initiate(TestUser, TestPassword, CancellationToken.None);
            
            Assert.Equal("testuser:1601637685000:xAN06yHWAL2P9Tzud2S3piJN2701IDqz1gUpbmMM3CQ=", client.Token);
        }
        
        [Fact]
        public async void DisposeRemovesToken()
        {
            NationalRailEnquiriesSource client;
            var handler = new MockHttpMessageHandler(SuccessResponse, HttpStatusCode.OK);
            var httpClient = new HttpClient(handler);
            using(client = new NationalRailEnquiriesSource(httpClient, Substitute.For<ILogger>()))
            {
                await client.Initiate(TestUser, TestPassword, CancellationToken.None);
                Assert.NotNull(client.Token);
            }
            Assert.Null(client.Token);
            Assert.DoesNotContain(httpClient.DefaultRequestHeaders, h => h.Key == "X-Auth-Token");
        }
        
        [Fact]
        public async void GetXml()
        {
            var responses = new[]
            {
                new MockResponse(SuccessResponse, HttpStatusCode.OK),
                new MockResponse(File.ReadAllText(TestFiles.Tocs), HttpStatusCode.OK),
            };
            
            var handler = new MockHttpMessageHandler(responses);
            var source = new NationalRailEnquiriesSource(new HttpClient(handler), Substitute.For<ILogger>());
            await source.Initiate(TestUser, TestPassword, CancellationToken.None);

            using (var reader =
                await source.GetKnowledgebaseXml(KnowedgebaseSubjects.Tocs, CancellationToken.None))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        Assert.Equal("TrainOperatingCompanyList", reader.Name);
                        break;
                    }
                }
            }
        }
        
        [Fact]
        public async void GetXmlThrowsExceptionIfNotInitiated()
        {
            var responses = new[]
            {
                new MockResponse(SuccessResponse, HttpStatusCode.OK),
                new MockResponse(File.ReadAllText(TestFiles.Tocs), HttpStatusCode.OK),
            };
            
            var handler = new MockHttpMessageHandler(responses);
            var source = new NationalRailEnquiriesSource(new HttpClient(handler), Substitute.For<ILogger>());

            var ex = await Assert.ThrowsAsync<KnowledgebaseException>(() =>
                source.GetKnowledgebaseXml(KnowedgebaseSubjects.Tocs, CancellationToken.None));
            Assert.Equal("Source not initialised.", ex.Message);
        }
        
        [Fact]
        public async void GetXmlThrowsExceptionIfCallToNreFails()
        {
            var responses = new[]
            {
                new MockResponse(SuccessResponse, HttpStatusCode.OK),
                new MockResponse("Server error", HttpStatusCode.InternalServerError),
            };
            
            var handler = new MockHttpMessageHandler(responses);
            var source = new NationalRailEnquiriesSource(new HttpClient(handler), Substitute.For<ILogger>());
            await source.Initiate(TestUser, TestPassword, CancellationToken.None);
            
            var ex = await Assert.ThrowsAsync<KnowledgebaseException>(() =>
                source.GetKnowledgebaseXml(KnowedgebaseSubjects.Tocs, CancellationToken.None));
            Assert.Equal("Http Error InternalServerError getting Tocs", ex.Message);
        }
    }
}