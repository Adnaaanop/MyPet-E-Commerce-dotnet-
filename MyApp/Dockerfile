# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Run stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install sqlcmd and dependencies so wait-for-sql.sh will work
RUN apt-get update && apt-get install -y curl apt-transport-https gnupg \
 && curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - \
 && curl https://packages.microsoft.com/config/ubuntu/22.04/prod.list > /etc/apt/sources.list.d/mssql-release.list \
 && apt-get update \
 && ACCEPT_EULA=Y apt-get install -y msodbcsql18 mssql-tools unixodbc-dev

# Copy published app
COPY --from=build /app/publish .

# Copy the wait-for-sql script, fix line endings, and make executable
COPY wait-for-sql.sh .
RUN sed -i 's/\r$//' wait-for-sql.sh && chmod +x wait-for-sql.sh

EXPOSE 7200

# Use wait-for-sql.sh to wait for SQL Server before starting app
ENTRYPOINT ["./wait-for-sql.sh", "sqlserver", "dotnet", "MyApp.dll"]
