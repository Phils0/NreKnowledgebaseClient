function getSchemas {
    param (
        $filter
    )
    $files = Get-ChildItem -Path C:\Users\phils\source\NreKnowledgebaseClient\schemas -Filter *.xsd;

    $schemas = ""
    foreach ($f in $files){
        if($f.Name -match $filter)
        {
            $schemas = $schemas + " " +  $f.Name;
        }
    }
    return $schemas;
}

$commonSchemas = " ";
$common = Get-ChildItem -Path C:\Users\phils\source\NreKnowledgebaseClient\schemas\apd -Filter *.xsd;
foreach ($f in $common){
    $commonSchemas = $commonSchemas + " .\apd\" +  $f.Name;
}

# Add dummy schema so output class file called schemaV4.cs
# See https://stackoverflow.com/a/33906829/3805124
$versionSchemas = getSchemas 'nre-.+-v4-0.xsd';
$schemas = "xsd " +  $versionSchemas + $commonSchemas + " .\dummy\schemaV4.xsd /c /n:NreKnowledgebase.SchemaV4";

Write-Output $schemas;
Invoke-Expression $schemas;

$versionSchemas = getSchemas 'nre-.+-v5-0.xsd';
$schemas = "xsd " +  $versionSchemas + $commonSchemas + " .\dummy\schemaV5.xsd /c /n:NreKnowledgebase.SchemaV5";

Write-Output $schemas;
Invoke-Expression $schemas;