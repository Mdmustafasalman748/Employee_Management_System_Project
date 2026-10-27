FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first so this layer is cached until the .csproj changes
COPY EmployeeManagement.Web/EmployeeManagement.Web.csproj EmployeeManagement.Web/
RUN dotnet restore EmployeeManagement.Web/EmployeeManagement.Web.csproj

COPY EmployeeManagement.Web/ EmployeeManagement.Web/
RUN dotnet publish EmployeeManagement.Web/EmployeeManagement.Web.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true

EXPOSE 8080
# Hosts usually provide the port in $PORT; fall back to 8080
CMD ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-8080} dotnet EmployeeManagement.Web.dll"]
