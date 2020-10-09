using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Serilog;

namespace NreKnowledgebase
{
    public abstract class KnowledgebaseSourceBase : IKnowledgebaseSource
    {
        protected ILogger _logger;

        protected KnowledgebaseSourceBase(ILogger logger)
        {
            _logger = logger;
        }
        
        public async Task<XmlTextReader> GetKnowledgebaseXml(KnowedgebaseSubjects subject, CancellationToken token)
        {
            try
            {
                var stream = await GetKnowledgebaseStream(subject, token);
                return new XmlTextReader(stream);
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

        public abstract Task<Stream> GetKnowledgebaseStream(KnowedgebaseSubjects subject, CancellationToken token);
    }
}