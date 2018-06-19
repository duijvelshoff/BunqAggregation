FROM microsoft/aspnetcore-build:1.1.8-1.1.9 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/aspnetcore:1.1.8
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "bunqAggregation.dll"] 
