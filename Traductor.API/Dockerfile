FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["../Traductor.API/Traductor.API.csproj", "../Traductor.API/"]
RUN dotnet restore "../Traductor.API/Traductor.API.csproj"
COPY . .
WORKDIR "/src/../Traductor.API"
RUN dotnet build "Traductor.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Traductor.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Traductor.API.dll"]