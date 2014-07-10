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
    public class Item
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Google Voice email of the user.
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// WNS connection string.
        /// </summary>
        public string PushConnectionString { get; set; }
        /// <summary>
        /// What kind of app does the PushConnectionString belong to?
        /// 0 - Windows
        /// 1 - Windows Phone
        /// </summary>
        public int DeviceType { get; set; }
    }
}
