# NreKnowledgebaseClient
NRE Knowledgebase Client - classes to represent the knowledgebase formats plus a downloader

[![Build Status](https://dev.azure.com/phils0oss/NreKnowledgebaseClient/_apis/build/status/Phils0.NreKnowledgebaseClient?branchName=master)](https://dev.azure.com/phils0oss/NreKnowledgebaseClient/_build/latest?definitionId=1&branchName=master)
 
 ## Generating the xml classes
 
 Run the `generateClasses.ps1` powershell script found in the schemas folder.
 
 The Fares schema "http://nationalrail.co.uk/xml/ticket" generates an incompatible `ClassEnumeration`, manually change the `TicketTypeDescriptionStructure.Class` property to be a string to stop deserialisation issues.