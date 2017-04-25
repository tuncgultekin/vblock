/*
 *  Scenario:
 *  
 *  Suppose, there is a central invoice processing service and it accepts invoice data from the various branches of a company.
 *  Each of the branches is able send multiple data package to central invoice processing service at a time.
 *  Invoice processing service can consume the multiple data packages of different branches at the same time 
 *  however, it cannot process the multiple data packages of a same branch.
 *  Invoice processing operation is queued with respect to brach id by using VBLock.
   
    
    +-------------------+     Branch-1 Packages
    |                   +------------------------------+
    |    Branch + 1     |----------------------------+ |
    |                   +--------------------------+ | |
    +-------------------+                          v v v                                         +-------------------------------------+
                                     +-------------------------------+     Branch-1 Serialized   |                                     |
                                     |                               |          Packages         |                                     |
    +-------------------+            |                               | ------------------------> |                                     |
    |                   +----------> |       Invoice Processing      |                           |                                     |
    |    Branch + 2     |----------> |                               | ------------------------> |          Invoice Consumer           |
    |                   +----------> |            Service            |                           |                                     |
    +-------------------+            |                               | ------------------------> |                                     |
                                     |                               |     Branch-3 Serialized   |                                     |
                                     +-------------------------------+          Packages         |                                     |
    +-------------------+                          ^ ^ ^                                         +-------------------------------------+
    |                   +--------------------------+ | |
    |    Branch + 3     |----------------------------+ |
    |                   +------------------------------+
    +-------------------+     Branch-3 Packages

*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SampleApp
{
    internal class Branch
    {
        private Task[] cashDeskTasks;
        private bool isTaskLoopEnabled = false;
        //
        internal string BranchId { get; private set; }
        internal int CashDeskCount { get; private set; }

        internal Branch(string branchId, int cashDeskCount)
        {
            this.BranchId = branchId;
            cashDeskTasks = new Task[cashDeskCount];
        }

        internal void StartSendingdData()
        {
            isTaskLoopEnabled = true;

            for (int i = 0; i < cashDeskTasks.Length; i++)
            {
                cashDeskTasks[i] = Task.Run(() =>
                {
                    Random r = new Random();
                    while (isTaskLoopEnabled)
                    {
                        BranchInvoiceData newInvoice = new BranchInvoiceData(BranchId, string.Concat(BranchId, " invoice at ", DateTime.UtcNow));
                        Console.WriteLine(string.Concat("Invoice: ", newInvoice.Data, " is created and going to be processed..."));
                        InvoiceProcessingService.ProcessInvoice(newInvoice);
                        Thread.Sleep(r.Next(2500) +2500);
                    }
                });
            }

        }

        internal void StopSendingdData()
        {
            isTaskLoopEnabled = false;
        }
    }
}
