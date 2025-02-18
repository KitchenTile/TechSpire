# Build stage for .NET application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
# Copy csproj and restore as distinct layers
COPY tvscheduler/*.csproj ./tvscheduler/
RUN dotnet restore ./tvscheduler/tvscheduler.csproj
# Copy everything else and build
COPY tvscheduler/. ./tvscheduler/
WORKDIR /app/tvscheduler
RUN dotnet publish -c Release -o out

# Build stage for React application
FROM node:18 AS react-build
WORKDIR /app/tvscheduler-react-app
# Copy package.json and install dependencies
COPY tvscheduler-react-app/tvscheduler-react-app/package.json ./
COPY tvscheduler-react-app/tvscheduler-react-app/package-lock.json ./
RUN npm install
# Copy everything else and build
COPY tvscheduler-react-app/tvscheduler-react-app/. ./
RUN npm run build

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
# Expose ports
EXPOSE 80
EXPOSE 8080
EXPOSE 5173
# Copy .NET application
COPY --from=build /app/tvscheduler/out ./
# Copy React application
COPY --from=react-build /app/tvscheduler-react-app/dist ./wwwroot
ENTRYPOINT ["dotnet", "tvscheduler.dll"]