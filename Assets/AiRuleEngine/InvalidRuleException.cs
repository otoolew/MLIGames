using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AiRuleEngine
{
    class InvalidRuleException : Exception
    {
        string m_Message;

        public InvalidRuleException() { m_Message = "Unknown exception"; }
        public InvalidRuleException(string message) { m_Message = message; }

        string GetMessage() { return m_Message; }
    }
}
