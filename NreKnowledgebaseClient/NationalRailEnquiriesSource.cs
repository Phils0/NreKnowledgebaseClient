using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Serilog;

namespace NreKnowledgebase
{
    /// <summary>
    /// Client to get the knowledgebase directly from https://opendata.nationalrail.co.uk
    /// </summary>
    public class NationalRailEnquiriesSource : IKnowledgebaseSource, IDisposable
    {
        public static readonly Uri Authenticate = new Uri(@"https://opendata.nationalrail.co.uk/authenticate");

        public static readonly IReadOnlyDictionary<KnowedgebaseSubjects, Uri> SourceUrls =
            new Dictionary<KnowedgebaseSubjects, Uri>()
            {
                {
                    KnowedgebaseSubjects.TicketTypes,
                    new Uri(@"https://opendata.nationalrail.co.uk/api/staticfeeds/4.0/ticket-types")
                },
                {
                    KnowedgebaseSubjects.TicketRestrictions,
                    new Uri(@"https://opendata.nationalrail.co.uk/api/staticfeeds/4.0/ticket-restrictions")
                },
                {
                    KnowedgebaseSubjects.Promotions,
                    new Uri(@"https://opendata.nationalrail.co.uk/api/staticfeeds/4.0/promotions-publics")
                },
                {
                    KnowedgebaseSubjects.Incidents,
                    new Uri(@"https://opendata.nationalrail.co.uk/api/staticfeeds/5.0/incidents")
                },
                {
                    KnowedgebaseSubjects.TocServiceIndicators,
                    new Uri(@"https://opendata.nationalrail.co.uk/api/staticfeeds/4.0/serviceIndicators")
                },
                {
                    KnowedgebaseSubjects.Stations,
                    new Uri(@"https://opendata.nationalrail.co.uk/api/staticfeeds/4.0/stations")
                },
                {KnowedgebaseSubjects.Tocs, new Uri(@"https://opendata.nationalrail.co.uk/api/staticfeeds/4.0/tocs")},
            };

        private readonly HttpClient _client;
        private readonly ILogger _logger;

        public string Token { get; private set; }

        public bool IsInititated => !string.IsNullOrEmpty(Token);

        public NationalRailEnquiriesSource(HttpClient client, ILogger logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task Initiate(string user, string password, CancellationToken token)
        {
            try
            {
                using (var request = CreateAuthenticateRequest(user, password))
                {
                    var response = await _client.PostAsync(Authenticate, request, token);
                    ;
                    var responseData = await response.Content.ReadAsStringAsync();
                    ExtractToken(responseData);
                    _client.DefaultRequestHeaders.Add("X-Auth-Token", Token);
                }
            }
            catch (KnowledgebaseException ke)
            {
                _logger.Error(ke, ke.Message);
                throw;
            }
            catch (WebException we)
            {
                _logger.Error(we, we.Message);
                throw;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error initialise knowledgebase source");
                throw new KnowledgebaseException("Error initialise knowledgebase source", e);
            }
        }

        private FormUrlEncodedContent CreateAuthenticateRequest(string user, string password)
        {
            var formParameters = new Dictionary<string, string>()
            {
                {"username", user},
                {"password", password}
            };

            var request = new FormUrlEncodedContent(formParameters);
            request.Headers.Clear();
            request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            return request;
        }

        private readonly Regex tokenRegex = new Regex(@"""token"":\s*""(.*)""");

        private void ExtractToken(string response)
        {
            var match = tokenRegex.Match(response);
            if (match.Success)
            {
                Token = match.Groups[1].Value;
                _logger.Debug("Knowledgebase Token: {Token}", Token);
            }
            else
            {
                throw new KnowledgebaseException($"Authentication error. Response: {response}");
            }
        }

        public void Dispose()
        {
            Token = null;
            _client.DefaultRequestHeaders.Remove("X-Auth-Token");
        }

        public async Task<XmlTextReader> GetKnowledgebaseXml(KnowedgebaseSubjects subject, CancellationToken token)
        {
            if (!IsInititated)
                throw new KnowledgebaseException("Source not initialised.");

            try
            {
                var response = await _client.GetAsync(SourceUrls[subject], token);
                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    return new XmlTextReader(stream);
                }

                var message =
                    $"Http Error {response.StatusCode} getting {Enum.GetName(typeof(KnowedgebaseSubjects), subject)}";
                _logger.Error(message);
                throw new KnowledgebaseException(message);
            }
            catch (KnowledgebaseException)
            {
                throw;
            }
            catch (Exception e)
            {
                var message = $"Error getting {Enum.GetName(typeof(KnowedgebaseSubjects), subject)}";
                _logger.Error(e, message);
                throw new KnowledgebaseException(message, e);
            }
        }
    }
}