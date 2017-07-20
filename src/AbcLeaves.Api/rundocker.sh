echo "Building leaves-api docker image..."
docker build -t leaves-api --build-arg source=bin/Debug/netcoreapp2.0/linux-x64/publish .
echo

echo "Run docker container..."
docker run --rm -it -p 8080:80 leaves-api
