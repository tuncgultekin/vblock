using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VBLock
{
    /// <summary>
    /// Provides main locking functions and garbage collection facilities
    /// </summary>
    public class Utils
    {
        private const int GARBAGE_COLLECTION_PERIOD_MS = 10000;
        private const int CACHE_EXISTANCE_DURATION_MINS = 60;
        private static readonly Dictionary<String, LockItem> _lockDict = new Dictionary<string, LockItem>();
        private static ReaderWriterLockSlim _lockDictLock = new ReaderWriterLockSlim();
        private static Timer _garbageCollectorTimer = new Timer(GarbageCollectorTimerElapsed, null, 1000, GARBAGE_COLLECTION_PERIOD_MS);

        /// <summary>
        /// Creates a lock for specified value
        /// </summary>
        /// <param name="value">Lock value</param>
        public static void LockOnValue(string value)
        {
            if (value == null)
                throw new ArgumentNullException(value);

            var lockItem = GetLockObjectForValue(value);
            
            Monitor.Enter(lockItem);
            lockItem.IsLocked = true;
        }

        /// <summary>
        /// Releases the lock which is held for specified value
        /// </summary>
        /// <param name="value">Lock value</param>
        public static void UnlockOnValue(string value)
        {
            if (value == null)
                throw new ArgumentNullException(value);

            var lockItem = GetLockObjectForValue(value);

            if ((lockItem == null) || (lockItem.IsLocked == false))
                throw new Exception(string.Format("Lock has not been acquired for value {0}", value));

            lockItem.IsLocked = false;
            Monitor.Exit(lockItem);
        }

        /// <summary>
        /// Provides the list of locked values
        /// </summary>
        /// <returns>List of locked values</returns>
        public static List<string> GetLockedValuesList()
        {
            try
            {
                _lockDictLock.EnterReadLock();
                return _lockDict.Where(t => (t.Value.IsLocked == true)).Select(t => t.Key).ToList<string>();
            }
            finally
            {
                if (_lockDictLock.IsReadLockHeld)
                    _lockDictLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Callback method for garbage collector timer
        /// Removes the value cache entries which are not used for CACHE_EXISTANCE_DURATION_MINS minutes
        /// </summary>
        /// <param name="stateInfo">Timer state info</param>
        private static void GarbageCollectorTimerElapsed(Object stateInfo)
        {
            try
            {
                _lockDictLock.EnterUpgradeableReadLock();
                var dateThreshold = DateTime.UtcNow.AddMinutes(-1 * CACHE_EXISTANCE_DURATION_MINS);
                var candidateList = _lockDict.Where(t => (t.Value.LastAccessDate < dateThreshold && t.Value.IsLocked == false));

                if (candidateList.Count() > 0)
                {
                    _lockDictLock.EnterWriteLock();
                    candidateList = _lockDict.Where(t => (t.Value.LastAccessDate < dateThreshold && t.Value.IsLocked == false));
                    if (candidateList.Count() > 0)
                    {
                        foreach (var item in candidateList)
                        {
                            _lockDict.Remove(item.Key);
                        }
                    }
                }
            }
            finally
            {
                if (_lockDictLock.IsWriteLockHeld)
                    _lockDictLock.ExitWriteLock();

                if (_lockDictLock.IsUpgradeableReadLockHeld)
                    _lockDictLock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Provides an LockItem instance for specified string value to hold a lock
        /// </summary>
        /// <param name="value">String value</param>
        /// <returns>LockItem instance</returns>
        private static LockItem GetLockObjectForValue(string value)
        {
            LockItem lockItem = null;
            try
            {
                _lockDictLock.EnterUpgradeableReadLock();
                lockItem = _lockDict.ContainsKey(value) ? _lockDict[value] : null;

                if (lockItem == null)
                {
                    _lockDictLock.EnterWriteLock();

                    lockItem = _lockDict.ContainsKey(value) ? _lockDict[value] : null;
                    if (lockItem == null)
                    {
                        lockItem = new LockItem();
                        _lockDict.Add(value, lockItem);
                    }
                };

                lockItem.LastAccessDate = DateTime.UtcNow;
                return lockItem;
            }
            finally
            {
                if (_lockDictLock.IsWriteLockHeld)
                    _lockDictLock.ExitWriteLock();

                if (_lockDictLock.IsUpgradeableReadLockHeld)
                    _lockDictLock.ExitUpgradeableReadLock();
            }
        }
    }
}
