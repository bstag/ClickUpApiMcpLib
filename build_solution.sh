#!/bin/bash
# Perform initial build

# Add .dotnet tools to PATH
export PATH="/home/jules/.dotnet:/home/jules/.dotnet/tools:$PATH"

echo "Attempting to build the solution..."
# Using a variable to store the path to the solution file for clarity
SOLUTION_FILE="src/ClickUp.Api.sln"

if [ -f "$SOLUTION_FILE" ]; then
  dotnet build "$SOLUTION_FILE" --nologo
  BUILD_STATUS=$?
  if [ $BUILD_STATUS -eq 0 ]; then
    echo "Build completed successfully."
  else
    echo "Build failed with status $BUILD_STATUS. Errors are listed above."
  fi
  exit $BUILD_STATUS
else
  echo "Error: Solution file '$SOLUTION_FILE' not found."
  exit 1
fi
