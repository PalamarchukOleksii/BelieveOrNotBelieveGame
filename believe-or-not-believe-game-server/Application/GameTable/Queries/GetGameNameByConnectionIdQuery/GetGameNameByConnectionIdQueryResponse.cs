namespace Application.GameTable.Queries.GetGameNameByConnectionIdQuery
{
    public class GetGameNameByConnectionIdQueryResponse
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public string GameName { get; set; } = string.Empty;
    }
}
