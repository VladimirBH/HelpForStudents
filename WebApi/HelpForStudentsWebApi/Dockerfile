FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY WebApi/HelpForStudentsWebApi/HelpForStudentsWebApi.csproj WebApi/HelpForStudentsWebApi/HelpForStudentsWebApi.csproj
RUN dotnet restore WebApi/HelpForStudentsWebApi/HelpForStudentsWebApi.csproj
COPY . .
WORKDIR /src/WebApi/HelpForStudentsWebApi
RUN dotnet build HelpForStudentsWebApi.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish HelpForStudentsWebApi.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "HelpForStudentsWebApi.dll"]
