using System.Collections.Generic;
namespace ClickUp.Api.Client.Services
{
    internal class ClickUpV3DataResponse<T>
    {
        public T? Data { get; set; }
    }

    internal class ClickUpV3DataListResponse<T>
    {
        public List<T>? Data { get; set; }
    }
}
