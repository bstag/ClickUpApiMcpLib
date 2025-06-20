#!/bin/bash
# Modify TaskRelationshipsService.cs to fix AddTaskLinkAsync return

FILE_PATH="src/ClickUp.Api.Client/Services/TaskRelationshipsService.cs"
METHOD_SIGNATURE="public async Task<CuTask?> AddTaskLinkAsync("
TARGET_RETURN_LINE_PATTERN="return response;"
REPLACEMENT_RETURN_LINE="return response?.Task;"

# Check if the method signature exists in the file
if grep -q "$METHOD_SIGNATURE" "$FILE_PATH"; then
    # Use sed to replace the return statement within the AddTaskLinkAsync method
    # This pattern looks for the method signature, then within the block defined by { ... }
    # it replaces "return response;"
    sed -i "/${METHOD_SIGNATURE}/,/^ *}/s/${TARGET_RETURN_LINE_PATTERN}/${REPLACEMENT_RETURN_LINE}/" "$FILE_PATH"

    echo "Attempted to modify return statement in AddTaskLinkAsync in $FILE_PATH."

    # Verify the change for AddTaskLinkAsync
    # We need to check if the specific line is now present within the method context
    # A simple grep might find it elsewhere if the pattern is too generic.
    # For now, a direct grep for the replaced line is a good first check.
    if grep -q "$REPLACEMENT_RETURN_LINE" "$FILE_PATH"; then
        echo "Successfully modified AddTaskLinkAsync in $FILE_PATH."
        exit 0
    else
        echo "Failed to verify modification in AddTaskLinkAsync in $FILE_PATH. The line '$REPLACEMENT_RETURN_LINE' was not found."
        # Optionally, print the method block to see what happened
        echo "--- Method Block for AddTaskLinkAsync after sed ---"
        sed -n "/${METHOD_SIGNATURE}/,/^ *}/p" "$FILE_PATH"
        echo "--- End of Method Block ---"
        exit 1
    fi
else
    echo "Could not find method signature '$METHOD_SIGNATURE' in $FILE_PATH."
    exit 1
fi
