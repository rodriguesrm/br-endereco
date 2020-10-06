FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["OpenBr.Endereco.Worker/OpenBr.Endereco.Worker.csproj", "OpenBr.Endereco.Worker/"]
COPY ["OpenBr.Endereco.Business/OpenBr.Endereco.Business.csproj", "OpenBr.Endereco.Business/"]

RUN dotnet restore "OpenBr.Endereco.Worker/OpenBr.Endereco.Worker.csproj"
COPY . .

WORKDIR "/src/OpenBr.Endereco.Worker"
RUN dotnet build "OpenBr.Endereco.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpenBr.Endereco.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "OpenBr.Endereco.Worker.dll"]