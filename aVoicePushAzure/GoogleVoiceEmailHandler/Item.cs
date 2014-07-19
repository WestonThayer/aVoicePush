using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleVoiceEmailHandler
{
    /// <summary>
    /// An ORM for the [avoicedb].[Item] table.
    /// </summary>
    public class Item : IItem
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string PushConnectionString { get; set; }
        public int DeviceType { get; set; }

        public IEnumerable<IItem> Query(string userEmail)
        {
            throw new NotImplementedException();
        }
    }
}
