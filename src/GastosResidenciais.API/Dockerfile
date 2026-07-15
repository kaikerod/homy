FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Directory.Build.props Directory.Packages.props global.json GastosResidenciais.sln ./
COPY src/GastosResidenciais.Domain/GastosResidenciais.Domain.csproj src/GastosResidenciais.Domain/
COPY src/GastosResidenciais.Application/GastosResidenciais.Application.csproj src/GastosResidenciais.Application/
COPY src/GastosResidenciais.Infrastructure/GastosResidenciais.Infrastructure.csproj src/GastosResidenciais.Infrastructure/
COPY src/GastosResidenciais.API/GastosResidenciais.API.csproj src/GastosResidenciais.API/
RUN dotnet restore src/GastosResidenciais.API/GastosResidenciais.API.csproj
COPY src/ src/
RUN dotnet publish src/GastosResidenciais.API/GastosResidenciais.API.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
RUN mkdir -p /data /app/logs && chown -R app:app /data /app/logs
COPY --from=build --chown=app:app /app/publish .
USER app
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
ENTRYPOINT ["dotnet", "GastosResidenciais.API.dll"]
