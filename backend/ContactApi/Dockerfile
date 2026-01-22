# FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
# WORKDIR /app
# EXPOSE 443
# EXPOSE 80
# # copy project csproj file and restore it in docker directory
# COPY ./*.csproj ./
# RUN dotnet restore
# # Copy everything into the docker directory and build
# COPY . .
# RUN dotnet publish -c Release -o out
# # Build runtime final image
# FROM mcr.microsoft.com/dotnet/aspnet:6.0
# WORKDIR /app
# COPY --from=build /app/out .
# ENTRYPOINT ["dotnet", "HTTPSCertDemo.dll"]


# ================================
# BUILD STAGE
# ================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy remaining source code
COPY . ./

# Build and publish optimized release
RUN dotnet publish -c Release -o /app/publish \
    /p:UseAppHost=false

# ================================
# RUNTIME STAGE
# ================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime

# Security: create non-root user
RUN addgroup -S appgroup && adduser -S appuser -G appgroup
USER appuser

WORKDIR /app

# Copy compiled output
COPY --from=build /app/publish .

# Expose API port
EXPOSE 8080

# Environment settings
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Healthcheck (Kubernetes friendly)
HEALTHCHECK --interval=30s --timeout=3s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

# Run app
ENTRYPOINT ["dotnet", "ContactApi.dll"]

