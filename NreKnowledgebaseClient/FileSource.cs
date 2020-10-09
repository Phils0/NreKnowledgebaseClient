using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Serilog;

namespace NreKnowledgebase
{
    /// <summary>
    /// Source that loads the knowledgebase from the file system
    /// </summary>
    public class FileSource : KnowledgebaseSourceBase
    {
        private readonly Dictionary<KnowedgebaseSubjects, string> _files;
        
        public FileSource(Dictionary<KnowedgebaseSubjects, string> files, ILogger logger): base(logger)
        {
            _files = files;
        }
        
        public override Task<Stream> GetKnowledgebaseStream(KnowedgebaseSubjects subject, CancellationToken token)
        {
            if (!_files.TryGetValue(subject, out var file))
            {
                var message = $"{Enum.GetName(typeof(KnowedgebaseSubjects), subject)} file not configured";
                _logger.Error(message);
                throw new KnowledgebaseException(message);
            }
            
            try
            {
                return Task.FromResult((Stream) File.OpenRead(file));
            }
            catch (FileNotFoundException fe)
            {
                var message = $"{Enum.GetName(typeof(KnowedgebaseSubjects), subject)} file does not exist: {file}";
                _logger.Error(fe, message);
                throw new KnowledgebaseException(message);                
            }
        }
    }
}