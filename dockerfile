FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
#restore src
WORKDIR /

#Copy 
COPY tests/UnitTests/API.Restful.UnitTests/API.Restful.UnitTests.csproj tests/UnitTests/API.Restful.UnitTests/API.Restful.UnitTests.csproj
COPY src/API.Restful/API.Restful.csproj src/API.Restful/
COPY src/API.Contracts/API.Contracts.csproj src/API.Contracts/
COPY src/Domain/Domain.csproj src/Domain/
COPY src/Application/Application.csproj src/Application/
COPY src/Common/Common.csproj src/Common/
COPY src/Persistence.SQL/Persistence.SQL.csproj src/Persistence.SQL/
RUN dotnet restore src/API.Restful/API.Restful.csproj
RUN dotnet restore tests/UnitTests/API.Restful.UnitTests/API.Restful.UnitTests.csproj

COPY . .

#restore test
RUN dotnet test tests/UnitTests/API.Restful.UnitTests/API.Restful.UnitTests.csproj

#build
RUN dotnet build src/API.Restful/API.Restful.csproj -c Release -o /app

#Publish
FROM build AS publish
RUN dotnet publish src/API.Restful/API.Restful.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "API.Restful.dll"]