using Domain.Common.Options;

namespace Presentation.Dtos
{
    public class CreateGameDto
    {
        public string CreatorName { get; set; } = string.Empty;
        public GameTableOptions GameTableOptions { get; set; } = new GameTableOptions();
    }
}
