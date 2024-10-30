namespace Application.GameTable.Commands.CreateGameCommand
{
    public class CreateGameCommandResponse
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public Domain.Models.GameModels.GameTable GameTable { get; set; } = new Domain.Models.GameModels.GameTable();
    }
}
