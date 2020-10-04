using System.Threading;
using System.Threading.Tasks;
using NreKnowledgebase.SchemaV4;
using NreKnowledgebase.SchemaV5;

namespace NreKnowledgebase
{
    public interface IKnowledgebaseAsync
    {
        Task<TicketTypeDescriptionList> GetTicketTypes(CancellationToken token);
        Task<TicketRestrictions> GetTicketRestrictions(CancellationToken token);
        Task<PromotionList> GetPromotions(CancellationToken token);
        Task<Incidents> GetIncidents(CancellationToken token);
        Task<NationalServiceIndicatorListStructure> GetTocServiceIndicators(CancellationToken token);
        Task<StationList> GetStations(CancellationToken token);
        Task<TrainOperatingCompanyList> GetTocs(CancellationToken token);
    }
}