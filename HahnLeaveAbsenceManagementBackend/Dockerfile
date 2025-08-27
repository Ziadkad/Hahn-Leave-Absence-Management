FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Development


FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["HahnLeaveAbsenceManagement.Api/HahnLeaveAbsenceManagement.Api.csproj", "HahnLeaveAbsenceManagement.Api/"]
RUN dotnet restore "HahnLeaveAbsenceManagement.Api/HahnLeaveAbsenceManagement.Api.csproj"
COPY . .


WORKDIR "/src/HahnLeaveAbsenceManagement.Api"
RUN dotnet build "HahnLeaveAbsenceManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "HahnLeaveAbsenceManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HahnLeaveAbsenceManagement.Api.dll"]