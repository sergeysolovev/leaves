FROM microsoft/dotnet:2-runtime-deps
LABEL name "leaves-api"

WORKDIR /app
ARG source=.
COPY $source .

ENV ASPNETCORE_URLS http://*:80
EXPOSE 80

ENTRYPOINT ["./leaves-api", "--admins=gunzeal@gmail.com"]
