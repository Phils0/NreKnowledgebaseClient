using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NreKnowledgebase.SchemaV4;
using NreKnowledgebase.SchemaV5;
using Serilog;

namespace NreKnowledgebase
{
    /// <summary>
    /// Facade class, acts as the entry point to the library
    /// implementing <see cref="IKnowledgebase"/> and <see cref="IKnowledgebaseAsync"/>
    /// </summary>
    public class Knowledgebase : IKnowledgebase, IKnowledgebaseAsync
    {
        private readonly IKnowledgebaseSource _source;
        private readonly ILogger _logger;

        public Knowledgebase(IKnowledgebaseSource source, ILogger logger)
        {
            _source = source;
            _logger = logger;
        }

        #region IKnowledgebase

        public TicketTypeDescriptionList TicketTypes => GetTicketTypes(CancellationToken.None).Result;

        public TicketRestrictions TicketRestrictions => GetTicketRestrictions(CancellationToken.None).Result;
        public PromotionList Promotions => GetPromotions(CancellationToken.None).Result;

        public Incidents Incidents => GetIncidents(CancellationToken.None).Result;

        public NationalServiceIndicatorListStructure TocServiceIndicators => GetTocServiceIndicators(CancellationToken.None).Result;

        public StationList Stations => GetStations(CancellationToken.None).Result;

        public TrainOperatingCompanyList Tocs => GetTocs(CancellationToken.None).Result;
        
        #endregion

        #region IKnowledgebaseAsync

        public Task<TicketTypeDescriptionList> GetTicketTypes(CancellationToken token)
        {
            return GetSubjectAsync<TicketTypeDescriptionList>(KnowedgebaseSubjects.TicketTypes, token);
        }

        public Task<TicketRestrictions> GetTicketRestrictions(CancellationToken token)
        {
            return GetSubjectAsync<TicketRestrictions>(KnowedgebaseSubjects.TicketRestrictions, token);
        }

        public Task<PromotionList> GetPromotions(CancellationToken token)
        {
            return GetSubjectAsync<PromotionList>(KnowedgebaseSubjects.Promotions, token);
        }

        public Task<Incidents> GetIncidents(CancellationToken token)
        {
            return GetSubjectAsync<Incidents>(KnowedgebaseSubjects.Incidents, token);
        }

        public Task<NationalServiceIndicatorListStructure> GetTocServiceIndicators(CancellationToken token)
        {
            return GetSubjectAsync<NationalServiceIndicatorListStructure>(KnowedgebaseSubjects.TocServiceIndicators, token);
        }

        public Task<StationList> GetStations(CancellationToken token)
        {
            return GetSubjectAsync<StationList>(KnowedgebaseSubjects.Stations, token);
        }

        public Task<TrainOperatingCompanyList> GetTocs(CancellationToken token)
        {
            return GetSubjectAsync<TrainOperatingCompanyList>(KnowedgebaseSubjects.Tocs, token);
        }
        
        #endregion
        
        public async Task<T> GetSubjectAsync<T>(KnowedgebaseSubjects subject, CancellationToken token) where T : class
        {
            try
            {
                using (var reader = await _source.GetKnowledgebaseXml(subject, token))
                {
                    var serialiser = new XmlSerializer(typeof(T));
                    return serialiser.Deserialize(reader) as T;
                }
            }
            catch (KnowledgebaseException)
            {
                throw;
            }
            catch (AggregateException ae)
            {
                if (ae.InnerExceptions.Count == 1)
                    throw ae.InnerExceptions[0];

                throw;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error when deserialising xml for {subject}", subject);
                throw new KnowledgebaseException($"Error when deserialising xml for {subject}");
            }
        }
    }
}