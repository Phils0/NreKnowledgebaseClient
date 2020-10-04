using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NreKnowledgebaseClient.Test
{
    public class MockResponse
    {
        public string Content { get; }
        public HttpStatusCode Status { get; }

        public MockResponse(string content, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            Content = content;
            Status = statusCode;
        }
    }
    
    /// <summary>
    /// Supports mocking <see cref="HttpClient"/>
    /// </summary>
    /// <remarks>Based upon https://dev.to/n_develop/mocking-the-httpclient-in-net-core-with-nsubstitute-k4j</remarks>
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly MockResponse[] _responses;
        
        public List<HttpRequestMessage> Input { get; } = new List<HttpRequestMessage>();
        public int NumberOfCalls { get; private set; }

        public MockHttpMessageHandler( string response, HttpStatusCode statusCode = HttpStatusCode.OK) :
            this(new []{ new MockResponse(response, statusCode) })
        {
        }
        
        public MockHttpMessageHandler( MockResponse[] responses)
        {
            _responses = responses;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = _responses[NumberOfCalls];
            NumberOfCalls++;
            Input.Add(request);

            return Task.FromResult(new HttpResponseMessage
               {
                   StatusCode = response.Status,
                   Content = new StringContent(response.Content)
               }) ;
        }
    }
}