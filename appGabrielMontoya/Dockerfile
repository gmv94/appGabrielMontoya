# Etapa 1: Construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar el archivo .csproj y restaurar las dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar el resto de los archivos y compilar la aplicación
COPY . ./
RUN dotnet publish -c Release -o /out

# Etapa 2: Ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

# Exponer el puerto en el contenedor
EXPOSE 80
EXPOSE 443

# Comando para ejecutar la aplicación
ENTRYPOINT ["dotnet", "appGabrielMontoya.dll"]
