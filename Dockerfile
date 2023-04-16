#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#COPY ./libsteam_api.so /app/
#ENV LD_LIBRARY_PATH="/app:${LD_LIBRARY_PATH}"

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["steamAuth/steamAuth.csproj", "steamAuth/"]
RUN dotnet restore "steamAuth/steamAuth.csproj"
COPY . .
WORKDIR "/src/steamAuth"
RUN dotnet build "steamAuth.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "steamAuth.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "steamAuth.dll"]