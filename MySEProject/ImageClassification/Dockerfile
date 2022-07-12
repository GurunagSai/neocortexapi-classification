#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ImageClassification/ImageClassification.csproj", "ImageClassification/"]
COPY ["ArgsConfig/ArgsConfig.csproj", "ArgsConfig/"]
RUN dotnet restore "ImageClassification/ImageClassification.csproj"
COPY . .
WORKDIR "/src/ImageClassification"
RUN dotnet build "ImageClassification.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ImageClassification.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ImageClassification.dll"]