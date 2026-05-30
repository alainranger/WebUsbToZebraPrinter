FROM rust:1-bookworm AS build
ARG LABELIZE_REF=main
WORKDIR /src
RUN cargo install --git https://github.com/GOODBOY008/labelize.git --branch ${LABELIZE_REF} --locked labelize

FROM debian:bookworm-slim
RUN apt-get update \
    && apt-get install -y --no-install-recommends ca-certificates \
    && rm -rf /var/lib/apt/lists/*
COPY --from=build /usr/local/cargo/bin/labelize /usr/local/bin/labelize
EXPOSE 8080
ENTRYPOINT ["labelize", "serve", "--host", "0.0.0.0", "--port", "8080"]
