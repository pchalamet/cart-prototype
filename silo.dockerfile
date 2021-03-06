﻿FROM microsoft/dotnet:2.2-sdk as builder
ARG configuration=Release

COPY . /build
RUN echo "Building $configuration"
RUN dotnet build -c $configuration /build/services/silo/silo.csproj
RUN dotnet test -c $configuration /build/services/cart.grain.tests/cart.grain.tests.csproj
RUN dotnet publish -c $configuration --output /artifacts/silo /build/services/silo/silo.csproj

FROM microsoft/dotnet:2.2-runtime
COPY --from=builder /artifacts/silo /app
WORKDIR /app
ENTRYPOINT ["dotnet", "silo.dll"]
