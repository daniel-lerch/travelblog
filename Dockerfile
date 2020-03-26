FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY src/*/*.csproj ./
RUN set -x \
    && dotnet restore -r debian.10-x64 \
    && dotnet tool install -g Microsoft.Web.LibraryManager.Cli \
    && export PATH="$PATH:/root/.dotnet/tools" \
    && libman restore --root src/TravelBlog

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -r debian.10-x64 -o /app/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/runtime-deps:3.1-buster-slim
WORKDIR /app
RUN set -x \
    && apt-get update \
    && apt-get install -y libfontconfig1
COPY --from=build-env /app/out .
ENTRYPOINT ["/app/TravelBlog"]
