No Downtime (working title)
==============

Introduction
--------------
A framework to run services, and update versions without downtime. 

Usage
--------------
Currently, the entire repository is a Visual Studio 2015 solution. NoDowntime and Connector are the core of the solution: The other projects are examples, and will be moved to a different solution.

To use, simply reference Connector in your project, and implement Connector.IRecycableService in your service.
To run, either instantiate and control HostService directly, or use one of the recyclers in the Recyclers project.

Currently NoDowntime is still in a "proof of concept" stage. 

Details
--------------
NoDowntime works by dynamically loading the actual recycable service into a "slave" AppDomain, and unloading it on when recycling.
Recycling also reloads the configuration of the application, which allows for changing the configuration between versions.
NoDowntime supports reasonable defaults, which can be changed via either configuration, or parameters.
