#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["bbt.loan.sms.applicaion/bbt.loan.sms.applicaion.csproj", "bbt.loan.sms.applicaion/"]
RUN dotnet restore "bbt.loan.sms.applicaion/bbt.loan.sms.applicaion.csproj"
COPY . .
WORKDIR "/src/bbt.loan.sms.applicaion"
RUN dotnet build "bbt.loan.sms.applicaion.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "bbt.loan.sms.applicaion.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "bbt.loan.sms.applicaion.dll"]