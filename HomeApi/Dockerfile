FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base

WORKDIR /app
RUN apt-get update && apt-get install -y libfontconfig1
ENV LANG="de_AT.UTF-8"
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_VERSION
# If BUILD_VERSION is set/non-empty, use it, otherwise use a default value
ARG VERSION=${BUILD_VERSION:-1.0.0}
RUN echo "Version will be $VERSION or ${VERSION}"
RUN apt-get update && apt-get install -y libfontconfig1
WORKDIR /HomeApi
COPY ["HomeApi.csproj", "/"]
COPY . .
RUN dotnet build "HomeApi.csproj" -c Release -o /app/build -p:Version=$VERSION -p:AssemblyVersion=$VERSION

FROM build AS publish
RUN dotnet publish "HomeApi.csproj" -c Release -o /app/publish /p:UseAppHost=false -p:Version=$VERSION -p:AssemblyVersion=$VERSION

FROM base AS final
ARG BUILD_VERSION
# If BUILD_VERSION is set/non-empty, use it, otherwise use a default value
ARG VERSION=${BUILD_VERSION:-1.0.0}
RUN echo "Version will be $VERSION or ${VERSION}"
WORKDIR /app
COPY --from=publish /app/publish .
RUN echo $VERSION >> version.txt 
ENTRYPOINT ["dotnet", "HomeApi.dll"]
