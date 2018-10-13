FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY ShopApi/*.csproj ./ShopApi/
RUN dotnet restore

# copy everything else and build app
COPY ShopApi/. ./ShopApi/
WORKDIR /app/ShopApi
RUN dotnet publish -c Release -o out


FROM microsoft/dotnet:2.1-aspnetcore-runtime AS runtime
WORKDIR /app
COPY --from=build /app/ShopApi/out ./
ENTRYPOINT ["dotnet", "ShopApi.dll"]