#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
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
ENTRYPOINT ["dotnet", "OpenBr.Endereco.Web.Api.dll"]