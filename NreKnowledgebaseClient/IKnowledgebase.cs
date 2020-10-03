using NreKnowledgebase.SchemaV4;
using NreKnowledgebase.SchemaV5;

namespace NreKnowledgebase
{
    public interface IKnowledgebase
    {
        TicketTypeDescriptionList TicketTypes { get; }
        TicketRestrictions TicketRestrictions { get; }
        PromotionList Promotions { get; }
        Incidents Incidents { get; }
        NationalServiceIndicatorListStructure TocServiceIndicators { get; }
        StationList Stations { get; }
        TrainOperatingCompanyList Tocs { get; }
    }
}