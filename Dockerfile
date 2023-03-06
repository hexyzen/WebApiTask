FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WebApiTask/WebApiTask.csproj", "WebApiTask/"]
RUN dotnet restore "WebApiTask/WebApiTask.csproj"
COPY . .
WORKDIR "/src/WebApiTask"
RUN dotnet build "WebApiTask.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApiTask.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApiTask.dll"]