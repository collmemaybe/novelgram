FROM microsoft/aspnetcore-build:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY Src.csproj .
RUN dotnet restore Src.csproj
COPY . .
WORKDIR /src
RUN dotnet build Src.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Src.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Src.dll"]
