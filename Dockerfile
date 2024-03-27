FROM node:18 AS frontend
WORKDIR /app

# Copy package definition and restore node modules as distinct layers
COPY frontend/package.json frontend/package-lock.json ./
RUN npm install

# Copy everything else and build
COPY frontend ./
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY src/TravelBlog/TravelBlog.csproj src/TravelBlog/libman.json ./src/TravelBlog/
RUN set -x \
    && dotnet restore src/TravelBlog/TravelBlog.csproj \
    && dotnet tool install -g Microsoft.Web.LibraryManager.Cli \
    && export PATH="$PATH:/root/.dotnet/tools" \
    && libman restore --root src/TravelBlog

# Copy everything else and build
COPY . ./
RUN dotnet publish --no-restore -c Release -o /app/out src/TravelBlog/TravelBlog.csproj

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=backend /app/out .
COPY --from=frontend /app/dist wwwroot/
ENTRYPOINT ["dotnet", "TravelBlog.dll"]
