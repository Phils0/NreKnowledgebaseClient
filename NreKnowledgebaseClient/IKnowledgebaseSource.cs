﻿using System.Threading;
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
    
    public interface IKnowledgebaseSource
    {
        Task<XmlTextReader> GetKnowledgebaseXml(KnowedgebaseSubjects subject, CancellationToken token);
    }
}