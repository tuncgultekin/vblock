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

namespace SampleApp
{
    internal class InvoiceConsumer
    {        
        internal static void Consume(BranchInvoiceData invoice)
        {
            // Do some processing stuff...                
            Console.WriteLine(string.Concat("Invoice: ", invoice.Data," is processed."));
            Thread.Sleep(2000);
        }
    }
}
