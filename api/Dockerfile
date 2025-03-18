# Stage 1
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# SRC => Source
WORKDIR /src

ENV PATH="$PATH:/root/.dotnet/tools"

COPY *.csproj .

RUN dotnet restore 

COPY . .

RUN dotnet publish -c Release -o /app/publish


# Stage 2

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["sh", "-c", "dotnet api.dll"]