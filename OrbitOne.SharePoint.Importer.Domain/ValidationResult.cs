using System.Collections.Generic;
using System.Linq;

namespace OrbitOne.SharePoint.Importer.Domain
{
    public class ValidationResult
    {
        private IList<ValidationMessage> m_messages;
        

        public ValidationResult()
        {
            m_messages = new List<ValidationMessage>();
        }

        public string Source { get; set; }
        public IList<string> Errors
        {
            get { return m_messages.Where(m => m.MessageType == MessageType.Error).Select(m => m.Message).ToList(); }
        }

        public IList<string> Warnings
        {
            get
            {
                return m_messages.Where(m => m.MessageType == MessageType.Warning).Select(m => m.Message).ToList(); }
        }

        public bool IsValid
        {
            get
            {
                return m_messages.Count(m => m.MessageType == MessageType.Error) == 0;
            }
        }

        public void AddError(string message)
        {
            AddMessage(message,MessageType.Error);
        }
        
        public void AddWarning(string message)
        {
            AddMessage(message, MessageType.Warning);
        }

        private void AddMessage(string message, MessageType messageType)
        {
            m_messages.Add(new ValidationMessage { Message = message, MessageType = messageType });
        }
    }

    class ValidationMessage
    {
        public string Message { get; set; }
        public MessageType MessageType { get; set; }
    }

    enum MessageType
    {
        Error,
        Warning
    }
}