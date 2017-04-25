using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VBLock
{
    /// <summary>
    /// Keeps the lock status of a value
    /// </summary>
    internal class LockItem
    {
        internal object LockObj { get; private set; }
        internal DateTime LastAccessDate { get; set; }
        internal bool IsLocked { get; set; }

        internal LockItem()
        {
            LockObj = new object();
            LastAccessDate = DateTime.UtcNow;
            IsLocked = false;
        }
    }
}
