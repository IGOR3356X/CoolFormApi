# Указываем образ для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Устанавливаем рабочую директорию внутри контейнера
WORKDIR /app

# Копируем файл решения и всех необходимых проектов
COPY CoolFormApi.sln ./
COPY CoolFormApi.csproj ./

RUN curl -I https://api.nuget.org/v3/index.json

# Восстанавливаем зависимости
RUN dotnet restore CoolFormApi.csproj

# Копируем остальные файлы проекта
COPY . .

# Сборка проекта в режиме Release
RUN dotnet publish CoolFormApi.csproj -c Release -o out

# Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

RUN apt-get update && \
    apt-get install -y awscli

RUN aws --version

COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "CoolFormApi.dll"]
EXPOSE 8080