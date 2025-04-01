#!/bin/sh

echo "Running mnemosyne statrup script"

until pg_isready -h db -p 5434; do
  echo "Waiting for db..."
  sleep 1
done

dotnet ef database update

dotnet Mnemosyne.dll