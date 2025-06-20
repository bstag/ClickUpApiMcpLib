#!/bin/bash
# Perform the build and capture detailed error output

# Add .dotnet tools to PATH
export PATH="/home/jules/.dotnet:/home/jules/.dotnet/tools:$PATH"

echo "Attempting to build the solution and capture error details..."
SOLUTION_FILE="src/ClickUp.Api.sln"

if [ -f "$SOLUTION_FILE" ]; then
  # Capture the full output of the build command
  BUILD_OUTPUT=$(dotnet build "$SOLUTION_FILE" --nologo)
  BUILD_STATUS=$? # Capture the exit status of the build command

  if [ $BUILD_STATUS -eq 0 ]; then
    echo "Build completed successfully."
    echo "$BUILD_OUTPUT" # Print the output anyway, though it might be minimal for success
  else
    echo "Build failed with status $BUILD_STATUS."
    echo "---------------- BUILD OUTPUT START ----------------"
    echo "$BUILD_OUTPUT"
    echo "---------------- BUILD OUTPUT END ------------------"
  fi
  # It's important that this script exits with the build status
  # so the calling agent knows if the build succeeded or failed.
  exit $BUILD_STATUS
else
  echo "Error: Solution file '$SOLUTION_FILE' not found."
  exit 1
fi
