using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IItem
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        long Id { get; set; }
        /// <summary>
        /// Google Voice email of the user.
        /// </summary>
        string Email { get; set; }
        /// <summary>
        /// WNS connection string.
        /// </summary>
        string PushConnectionString { get; set; }
        /// <summary>
        /// What kind of app does the PushConnectionString belong to?
        /// 0 - Windows
        /// 1 - Windows Phone
        /// </summary>
        int DeviceType { get; set; }

        /// <summary>
        /// Find all rows that have the given email address.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        IEnumerable<IItem> Query(string userEmail);
    }
}
