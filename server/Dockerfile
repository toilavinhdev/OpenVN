#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Src/Services/Integration/OpenVN.Integrations/OpenVN.Integrations.csproj", "Src/Services/Integration/OpenVN.Integrations/"]
COPY ["Src/SharedKernel/SharedKernel.csproj", "Src/SharedKernel/"]
RUN dotnet restore "Src/Services/Integration/OpenVN.Integrations/OpenVN.Integrations.csproj"
COPY . .
WORKDIR "/src/Src/Services/Integration/OpenVN.Integrations"
RUN dotnet build "OpenVN.Integrations.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpenVN.Integrations.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenVN.Integrations.dll"]
