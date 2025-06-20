#!/bin/bash
# Modify MembersService.cs to use fully qualified Member type

FILE_PATH="src/ClickUp.Api.Client/Services/MembersService.cs"

# Ensure the Models.Entities using is present for Enumerable.Empty<Member>() if Member is from there,
# but the target type is from Models.ResponseModels.Members.Member
# The current code uses Enumerable.Empty<Member>() which will be replaced.

# Replace the return type and Enumerable.Empty for GetTaskMembersAsync
sed -i 's/public async Task<IEnumerable<Member>> GetTaskMembersAsync/public async Task<IEnumerable<ClickUp.Api.Client.Models.ResponseModels.Members.Member>> GetTaskMembersAsync/' "$FILE_PATH"
sed -i 's/return response?.Members ?? Enumerable.Empty<Member>();/return response?.Members ?? Enumerable.Empty<ClickUp.Api.Client.Models.ResponseModels.Members.Member>();/' "$FILE_PATH"

# Replace the return type and Enumerable.Empty for GetListMembersAsync
sed -i 's/public async Task<IEnumerable<Member>> GetListMembersAsync/public async Task<IEnumerable<ClickUp.Api.Client.Models.ResponseModels.Members.Member>> GetListMembersAsync/' "$FILE_PATH"
# The previous sed for Enumerable.Empty<Member>() might have already fixed the second one if it was identical,
# but we make sure by targeting the specific context if needed.
# However, the line is identical, so the previous sed should cover it.
# If GetListMembersAsync had a slightly different return line, a more specific sed would be needed.
# For now, assuming the line "return response?.Members ?? Enumerable.Empty<Member>();" is the same for both.

# Verify the changes (optional, but good for debugging)
echo "--- Modified Content of $FILE_PATH ---"
cat "$FILE_PATH"
echo "--- End of Content ---"

# Check if sed command was successful (basic check)
if grep -q "IEnumerable<ClickUp.Api.Client.Models.ResponseModels.Members.Member>" "$FILE_PATH"; then
  echo "Successfully modified $FILE_PATH."
  exit 0
else
  echo "Failed to modify $FILE_PATH as expected."
  exit 1
fi
