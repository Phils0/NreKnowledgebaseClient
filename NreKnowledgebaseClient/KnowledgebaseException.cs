using System;

namespace NreKnowledgebase
{
    public class KnowledgebaseException : Exception
    {
        public KnowledgebaseException()
        {
        }

        public KnowledgebaseException(string message)
            : base(message)
        {
        }

        public KnowledgebaseException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}