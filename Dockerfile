FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["IsHttpBug.csproj", "./"]
RUN dotnet restore "./IsHttpBug.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "IsHttpBug.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "IsHttpBug.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "IsHttpBug.dll"]
