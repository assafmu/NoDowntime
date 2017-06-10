No Downtime (working title)
==============

Introduction
--------------
A framework to run services, and update versions without downtime. 

Usage
--------------
To use, reference Connector in your project, and implement Connector.IRecycableService service. 
To run, either instantiate and control HostService directly, or use one of the recyclers in the Recyclers project.

For more complete examples, see Samples.

Details
--------------
NoDowntime works by dynamically loading the actual recycable service into a "slave" AppDomain, and unloading it on when recycling.
Recycling also reloads the configuration of the application, which allows for changing the configuration between versions.
NoDowntime supports reasonable defaults, which can be changed via either configuration, or parameters.

Solution Structure
--------------
Core contains the essentials of the solution - The Connector library, the NoDowntime library, and the Recyclers. In the future, these will be made into a Nuget package.
Samples contains usage examples.
Tests contains both conventional unit tests, and also integration tests. The integration tests can also serve as example for more advanced cases.