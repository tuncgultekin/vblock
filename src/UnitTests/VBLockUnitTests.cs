using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VBLock;
using Xunit;

namespace UnitTests
{
    public class VBLockUnitTests
    {
        [Fact]
        public void NullOrEmptyLockValTest()
        {

            Assert.Throws<ArgumentNullException>(() =>
            {

                string lockVal = null;
                var lockObj = new VBDisposableLock(lockVal);

            });

            Assert.Throws<ArgumentNullException>(() =>
            {

                string lockVal = "";
                var lockObj = new VBDisposableLock(lockVal);

            });
        }

        [Fact]
        public void GetLockedValuesListTest()
        {
            Random r = new Random();
            List<string> lockedValsInUsingScope = new List<string>();
            string lockVal = "testLockVal" + r.Next(1000);
            using (var lockObj = new VBDisposableLock(lockVal))
            {
                lockedValsInUsingScope = VBLock.Utils.GetLockedValuesList();
            }

            List<string> lockedValsAfterUsing = VBLock.Utils.GetLockedValuesList();

            Assert.Contains<string>(lockVal, lockedValsInUsingScope);
            Assert.DoesNotContain<string>(lockVal, lockedValsAfterUsing);

        }


        [Fact]
        public void LockValueTest()
        {
            Random r = new Random();
            string lockVal = "testLockVal" + r.Next(1000) + 1000;
            int waitTimeMs = 9000;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var task1 = Task.Run(() => {

                using (var lockObj = new VBDisposableLock(lockVal))
                {                                        
                    Thread.Sleep(waitTimeMs);
                }

            });

            var task2 = Task.Run(() => {

                using (var lockObj = new VBDisposableLock(lockVal))
                {
                    Thread.Sleep(waitTimeMs);
                }
            });

            Task.WaitAll(new Task[] { task1, task2 });
            sw.Stop();
            long elapsedMs = sw.ElapsedMilliseconds;

            Assert.False((elapsedMs < 2 * waitTimeMs));
        }
    }

}
