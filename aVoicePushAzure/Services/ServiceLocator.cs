using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ServiceLocator
    {
        #region Singleton

        private static ServiceLocator current;

        public static ServiceLocator Current
        {
            get
            {
                if (current == null)
                {
                    current = new ServiceLocator();
                }

                return current;
            }
        }

        #endregion

        public ILog Log { get; set; }
    }
}
