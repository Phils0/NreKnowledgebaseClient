using System;
using System.Threading;
using System.Xml.Serialization;
using NreKnowledgebase.SchemaV4;
using NreKnowledgebase.SchemaV5;
using Serilog;

namespace NreKnowledgebase
{
    public class Knowledgebase : IKnowledgebase
    {
        private readonly IKnowledgebaseSource _source;
        private readonly ILogger _logger;

        public Knowledgebase(IKnowledgebaseSource source, ILogger logger)
        {
            _source = source;
            _logger = logger;
        }

        public TicketTypeDescriptionList TicketTypes =>
            GetSubject<TicketTypeDescriptionList>(KnowedgebaseSubjects.TicketType);

        public TicketRestrictions TicketRestrictions =>
            GetSubject<TicketRestrictions>(KnowedgebaseSubjects.TicketRestrictions);

        public PromotionList Promotions =>
            GetSubject<PromotionList>(KnowedgebaseSubjects.Promotions);

        public Incidents Incidents =>
            GetSubject<Incidents>(KnowedgebaseSubjects.Incidents);

        public NationalServiceIndicatorListStructure TocServiceIndicators =>
            GetSubject<NationalServiceIndicatorListStructure>(KnowedgebaseSubjects.TocServiceIndicators);

        public StationList Stations =>
            GetSubject<StationList>(KnowedgebaseSubjects.Stations);

        public TrainOperatingCompanyList Tocs =>
            GetSubject<TrainOperatingCompanyList>(KnowedgebaseSubjects.Tocs);

        private T GetSubject<T>(KnowedgebaseSubjects subject) where T : class
        {
            try
            {
                using (var reader = _source.GetKnowledgebaseXml(subject, CancellationToken.None).Result)
                {
                    var serialiser = new XmlSerializer(typeof(T));
                    return serialiser.Deserialize(reader) as T;
                }
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