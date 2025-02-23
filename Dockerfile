FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copia os arquivos
COPY . ./

# Restaura
RUN dotnet restore

# Publica
RUN dotnet publish -c Release -o out

# Runtime final
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app/api
COPY --from=build /app/out .
ENTRYPOINT ["./Zapchat.Api"]
