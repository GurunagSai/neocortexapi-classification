# Exercise I

### Azure Account Details:

Please find my Azure Account Profile:

````
[
  {
    "cloudName": "AzureCloud",
    "homeTenantId": "66c5e13f-8c43-4359-b2e8-51775c6d298d",
    "id": "6593c0d9-711c-46fd-bd4b-25fab5c2ddf0",
    "isDefault": true,
    "managedByTenants": [],
    "name": "Azure for Students",
    "state": "Disabled",
    "tenantId": "66c5e13f-8c43-4359-b2e8-51775c6d298d",
    "user": {
      "name": "john.emenike@stud.fra-uas.de",
      "type": "user"
    }
  }
]
````

You can find my Docker Hub repository [Here][1]

Docker Hub Username:   ``ucheemenike``

Docker Hub Email Profile:  ``john.emenike@stud.fra-uas.de``

I verified the docker installation by running the ``docker version`` command as below

> Client: Docker Engine - Community
>  Version:           19.03.8
>  API version:       1.40
>  Go version:        go1.12.17
>  Git commit:        afacb8b
>  Built:             Wed Mar 11 01:23:10 2020
>  OS/Arch:           windows/amd64
>  Experimental:      false
> 
> Server: Docker Engine - Community
>  Engine:
>   Version:          19.03.8
>   API version:      1.40 (minimum version 1.12)
>   Go version:       go1.12.17
>   Git commit:       afacb8b
>   Built:            Wed Mar 11 01:29:16 2020
>   OS/Arch:          linux/amd64
>   Experimental:     false
>  containerd:
>   Version:          v1.2.13
>   GitCommit:        7ad184331fa3e55e52b890ea95e65ba581ae3429
>  runc:
>   Version:          1.0.0-rc10
>   GitCommit:        dc9208a3303feef5b3839f4323d9beb36df0a9dd
>  docker-init:
>   Version:          0.18.0
>   GitCommit:        fec3683
> 

    
### Exercise II

Kindly run the below command to donwload or pull the image published on my `docker hub` repository.

````
docker pull uchemenike/john_emenike:v1
````
Also find the `URL` of the publised image [Here][2]


### Exercise III

Please click [Here][3] to visit the published website.

The website was developed using ``Visual Studio`` with docker support. The website was publised via the Azure web service with an image built with the docker file described below:



> FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
> WORKDIR /app
> EXPOSE 80
> EXPOSE 443
> 
> FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
> WORKDIR /src
> COPY ["Uche_Emenike_Web_Page.csproj", "./"]
> RUN dotnet restore "Uche_Emenike_Web_Page.csproj"
> COPY . .
> WORKDIR "/src/"
> RUN dotnet build "Uche_Emenike_Web_Page.csproj" -c Release -o /app/build
> 
> FROM build AS publish
> RUN dotnet publish "Uche_Emenike_Web_Page.csproj" -c Release -o /app/publish
> 
> FROM base AS final
> WORKDIR /app
> COPY --from=publish /app/publish .
> ENTRYPOINT ["dotnet", "Uche_Emenike_Web_Page.dll"]
> 
 
The image was also publised on the Docker Hub repository and could be pulled with the below command:

````
docker pull uchemenike/john_emenike:exercise3
````

Please visit [my github repository][4] for the source code of the published web application.


### Exercise V

Please click [Here][5] for the source code of the Blob Storage application.

The Blob storage `ucheblobstorage` created on the Azure portal was specifically used for this application.

### Exercise VI

Please click [Here][6] for the source code of the Cosmos Table exercise.

A snapshot of the Cosmos Table on the Azure Portal could be found [here][7]

I created the cosmos DB account; `uche-cosmos-db` for this purpose.

### Exercise VII

Please visit [Here][7] for the source code for the Queue Storage exercise.

Kindly check out the snapshots of the console and the Azure Portal activities during the exercise.
1. [Snapshot #1 - Console activity][9]
2. [Snapshot #2 - Azure Queue Storage][10]






[1]: <https://hub.docker.com/repository/docker/uchemenike/john_emenike> 
[2]: <https://hub.docker.com/layers/uchemenike/john_emenike/v1/images/sha256-200509934cc8f6481fa76c53e0e09f905348497207d426d96cf8437fcc02e9ee?context=repo>
[3]: <https://ucheweb-exercise-3.azurewebsites.net>
[4]: <https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2019-2020/tree/Uche/MyWork/Cloud%20Computing/Exercise%203>
[5]: <https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2019-2020/tree/Uche/MyWork/Cloud%20Computing/Blob_Storage>
[6]: <https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2019-2020/tree/Uche/MyWork/Cloud%20Computing/Exercise%206>
[7]: <https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2019-2020/blob/Uche/MyWork/Cloud%20Computing/Exercise%206/cosmos_table_snapshot.PNG>
[8]: <https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2019-2020/tree/Uche/MyWork/Cloud%20Computing/Exercise%207>
[9]: <https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2019-2020/blob/Uche/MyWork/Cloud%20Computing/Exercise%207/Queue_console%20snapshot.PNG>
[10]: <https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2019-2020/blob/Uche/MyWork/Cloud%20Computing/Exercise%207/myqueue_snapshot.PNG>


