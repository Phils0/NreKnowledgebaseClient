# NreKnowledgebaseClient
Provides a client API to access the NRE Knowledgebase https://wiki.openraildata.com/index.php?title=KnowledgeBase

![Build](https://github.com/phils0/NreKnowledgebaseClient/actions/workflows/build.yml/badge.svg)
![Package](https://github.com/phils0/NreKnowledgebaseClient/actions/workflows/package.yml/badge.svg)
 
 ## Using the library
 
The `Knowedgebase` class has properties (`IKnowedgebase` interface) and async methods (`IKnowedgebaseAsync` interface) to easily access the knowledgebase data.
 
The `Knowedgebase` constructor requires an implementation of `IKnowledgebaseSource`.  The library provides 2 implementations of this:
* `NationalRailEnquiriesSource` : this connects to https://opendata.nationalrail.co.uk to download the knowledgebase
* `FileSource` : this uses local files to load the knowledgebase
 
### Using `NationalRailEnquiriesSource` 
`NationalRailEnquiriesSource` requires being initialised before it can get any knowledgebase data. You need to have a registered user on https://opendata.nationalrail.co.uk and to have enabled the Knowledgebase feed. It implements`IDispose` to ensure it cleans itself up.  
```
    using (var source = new NationalRailEnquiriesSource(new HttpClient(), logger))
    {
        await source.Initiate(user, password, CancellationToken.None);
        var knowledgebase = new Knowledgebase(source, logger);
        var tocs = await knowledgebase.GetTocs(CancellationToken.None);
        ...
    }
```
### Using `FileSource` 
`FileSource` requires being initialised with a dictionary with the location of the knowledgebase files.  It does not require all knowledgebase subjects to be configured.  The library throws `KnowledgebaseException` if a knowledgebase subject is not configured or the file does not exist.  

```
    var sourceFiles = new Dictionary<KnowedgebaseSubjects, string>()
        {
            { KnowedgebaseSubjects.TicketTypes, "Data/TicketTypes.xml" },
            { KnowedgebaseSubjects.TicketRestrictions, "Data/TicketRestrictions.xml" },
            { KnowedgebaseSubjects.Promotions, "Data/Promotions.xml" },
            { KnowedgebaseSubjects.Incidents, "Data/Incidents.xml" },
            { KnowedgebaseSubjects.TocServiceIndicators, "Data/ServiceIndicators.xml" },
            { KnowedgebaseSubjects.Stations, "Data/Stations.xml" },
            { KnowedgebaseSubjects.Tocs, "Data/Tocs.xml" },
        };

    var knowledgebase = new Knowledgebase(new FileSource(sourceFiles, logger), logger);

    var tocs = await knowledgebase.GetTocs(CancellationToken.None);
```
 
## Build and Tests

The library has a set of unit tests.  
`LiveKnowledgebaseTest` is an integration test class that call the NRE web site.  Need to have your user and password set as environment variables `NRE_USER` and `NRE_PASSWORD`.  To run these tests need to delete the skip parameters.

### Library Dependencies
 
The library is .NetStandard2.0 and depends upon:
* Microsoft.AspNet.WebApi.Client
* Serilog for logging 
 
### Generating the xml classes
 
Run the `generateClasses.ps1` powershell script found in the schemas folder.
 
The Fares schema "http://nationalrail.co.uk/xml/ticket" generates an incompatible `ClassEnumeration`, manually change the `TicketTypeDescriptionStructure.Class` property to be a string to stop deserialisation issues.