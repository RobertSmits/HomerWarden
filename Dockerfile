FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["HomerWarden.csproj", "."]
RUN dotnet restore "HomerWarden.csproj"
COPY . .
RUN dotnet build "HomerWarden.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HomerWarden.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HomerWarden.dll"]
