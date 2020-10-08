using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace NreKnowledgebase
{
    public enum KnowedgebaseSubjects
    {
        TicketTypes,
        TicketRestrictions,
        Promotions,
        Incidents,
        TocServiceIndicators,
        Stations,
        Tocs
    }
    
    /// <summary>
    /// Interface to get the knowledgebase data
    /// </summary>
    public interface IKnowledgebaseSource
    {
        Task<XmlTextReader> GetKnowledgebaseXml(KnowedgebaseSubjects subject, CancellationToken token);
    }
}