using System.Runtime.Diagnostics;
using System.Diagnostics;

Task("Publish").Does(() => {
    DotNetCorePublish("src/AlfrescoGroupImport/AlfrescoGroupImport.fsproj", new DotNetCorePublishSettings {
        OutputDirectory = "./publish/alfresco-group-import",
        Configuration = "Release"
    });
});

Task("Zip")
    .IsDependentOn("Publish")
    .Does(() => {
        Zip("publish/alfresco-group-import", "publish/alfresco-group-import.0.1.0.zip");
    });


var target = Argument("target", "default");
RunTarget(target);
