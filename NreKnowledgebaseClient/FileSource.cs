using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Serilog;

namespace NreKnowledgebase
{
    public class FileSource : IKnowledgebaseSource
    {
        private readonly Dictionary<KnowedgebaseSubjects, string> _files;
        private readonly ILogger _logger;
        
        public FileSource(Dictionary<KnowedgebaseSubjects, string> files, ILogger logger)
        {
            _files = files;
            _logger = logger;
        }
        
        public async Task<XmlTextReader> GetKnowledgebaseXml(KnowedgebaseSubjects subject, CancellationToken token)
        {
            if (!_files.TryGetValue(subject, out var file))
            {
                var message = $"{Enum.GetName(typeof(KnowedgebaseSubjects), subject)} file not configured";
                _logger.Error(message);
                throw new KnowledgebaseException(message);
            }
            

            try
            {
                var stream = File.OpenRead(file);
                return await Task.FromResult(new XmlTextReader(stream));
            }
            catch (FileNotFoundException fe)
            {
                var message = $"{Enum.GetName(typeof(KnowedgebaseSubjects), subject)} file does not exist: {file}";
                _logger.Error(message);
                throw new KnowledgebaseException(message);                
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