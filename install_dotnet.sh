#!/bin/bash
# Install .NET SDK
chmod +x utilities/dotnet-install.sh
echo "Attempting to install .NET SDK 9.0..."
./utilities/dotnet-install.sh --channel 9.0
INSTALL_STATUS=$?
if [ $INSTALL_STATUS -eq 0 ]; then
  echo ".NET SDK installation script completed successfully."
  # Attempt to add dotnet to PATH for the current session
  # The script typically suggests a path like /root/.dotnet or /home/user/.dotnet
  # We'll try to find it and add it.
  DOTNET_ROOT=$(find ~ -maxdepth 1 -type d -name ".dotnet" -print -quit)
  if [ -n "$DOTNET_ROOT" ] && [ -d "$DOTNET_ROOT" ]; then
    export PATH="$DOTNET_ROOT:$DOTNET_ROOT/tools:$PATH"
    echo "Attempted to add $DOTNET_ROOT to PATH."
    dotnet --version
    PROBE_STATUS=$?
    if [ $PROBE_STATUS -eq 0 ]; then
      echo "dotnet command found and working."
    else
      echo "dotnet command still not found after attempting to set PATH. Further PATH adjustments might be needed or the script might have installed it elsewhere."
    fi
  else
    echo ".dotnet directory not found in home. dotnet might be installed elsewhere or not at all."
  fi
else
  echo ".NET SDK installation script failed with status $INSTALL_STATUS."
fi
exit $INSTALL_STATUS
