FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/RobotCleaner/RobotCleaner.csproj", "RobotCleaner/"]
RUN dotnet restore "RobotCleaner/RobotCleaner.csproj"
COPY ["src/RobotCleaner/.", "RobotCleaner/"]
WORKDIR "/src/RobotCleaner"
RUN dotnet build "RobotCleaner.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RobotCleaner.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "RobotCleaner.dll"]