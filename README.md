[![NuGet version](https://badge.fury.io/nu/VBLock.svg)](https://badge.fury.io/nu/VBLock) 
vblock
=========
Value based scope locking utility for .NET Standard.

Usage
-----------
##### Value based locking
```sh
string lockVal = "test";
using (var lockObj = new VBDisposableLock(lockVal))
{
    // Only one thread can enter this scope when the lockVal is "test"
    // Do some processing stuff...
}
```
##### Getting the list of locked values
```sh
List<string> lockedVals = VBLock.Utils.GetLockedValuesList();
```

Sample Scenario
-----------

Suppose, there is a central invoice processing service and it accepts invoice data from the various branches of a company.
- Each of the branches is able send multiple data package to central invoice processing service at a time.
- Invoice processing service can consume the multiple data packages of different branches at the same time however, it cannot process the multiple data packages of a same branch.
- Invoice processing operation is queued with respect to brach id by using VBLock.

```sh
    
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
```
For further details please see SampleApp.
