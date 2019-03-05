FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
COPY RanIoTCA.crt /usr/local/share/ca-certificates/RanIoTCA.crt
RUN update-ca-certificates

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["Client/Client.csproj", "Client/"]
RUN dotnet restore "Client/Client.csproj"
COPY . .
WORKDIR "/src/Client"
RUN dotnet build "Client.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Client.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Client.dll"]