EdmGen2M
========

EdmGen2 Modified : All Entity related processing now in separate class file

The functionality is broadly the same as other versions of EdmGen2 available.

My version here is based on the verion by Jiri Cincura, with many thanks for his EF version 5 updates.
http://blog.cincura.net/233311-edmgen2-for-net-4-5-and-entity-framework-5/
https://bitbucket.org/cincura_net/edmgen2

In my version I have split the program into two class files - EdmGen2.cs and EdmGen2Library.cs
This is because I wanted to use the Entity Frameworks processing as a dropin source library.

Additional Functionality:
I have added an option Designer arguement to /ToEdmx
	/ToEdmx <csdl file> <ssdl file> <msl file> [<des file>]"
I need this for work I am doing, where the Edmx processing must keep the same information present in the Edmx for when it is reloaded into the EF Designer.
	
Jsrsoft
June 2013
