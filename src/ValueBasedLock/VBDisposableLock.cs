using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VBLock
{
    /// <summary>
    ///
    /// </summary>
    public class VBDisposableLock : IDisposable
    {
        private string _lockVal;
        //
        public VBDisposableLock(string lockOnVal)
        {
            if (String.IsNullOrEmpty(lockOnVal))
                throw new ArgumentNullException(lockOnVal);

            _lockVal = lockOnVal;
            Utils.LockOnValue(_lockVal);
        }

        public void Dispose()
        {
            Utils.UnlockOnValue(_lockVal);
        }
    }
}
