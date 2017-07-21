#!/bin/bash

# Restore
echo "Restoring dependencies..."
dotnet restore
echo

# Create/Update leaves.db
if [ ! -f leaves.db ]; then
  echo "Creating leaves.db..."
else
  echo "Updating leaves.db..."
fi
dotnet ef database update
echo

# Publish
echo "Creating production build..."
dotnet publish -c Debug -r linux-x64
echo
echo "Done."
