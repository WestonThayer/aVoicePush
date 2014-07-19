using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IEmailHeader
    {
        string From { get; }
        string DeliveredTo { get; }
        string References { get; }
        string MessageId { get; }
        string Subject { get; }
    }
}
