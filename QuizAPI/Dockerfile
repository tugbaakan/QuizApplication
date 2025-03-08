FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["QuizAPI.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet build "QuizAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "QuizAPI.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "QuizAPI.dll"] 