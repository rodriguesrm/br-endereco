FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base

WORKDIR /app

EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build

WORKDIR /src
COPY ["OpenBr.Endereco.Web.Api/OpenBr.Endereco.Web.Api.csproj", "OpenBr.Endereco.Web.Api/"]
COPY ["OpenBr.Endereco.Business/OpenBr.Endereco.Business.csproj", "OpenBr.Endereco.Business/"]

RUN dotnet restore "OpenBr.Endereco.Web.Api/OpenBr.Endereco.Web.Api.csproj"
COPY . .

WORKDIR "/src/OpenBr.Endereco.Web.Api"
RUN dotnet build "OpenBr.Endereco.Web.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpenBr.Endereco.Web.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

RUN mkdir -p /app/wwwroot/data
RUN chmod 777 /app/wwwroot/data

ENTRYPOINT ["dotnet", "OpenBr.Endereco.Web.Api.dll"]