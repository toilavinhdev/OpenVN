namespace OpenVN.Master.Application.Commands
{
    public class UpdateFavouriteCommand : IRequest
    {
        public string AppId { get; }

        public bool IsFavourite { get; }

        public UpdateFavouriteCommand(string appId, bool isFavourite)
        {
            AppId = appId;
            IsFavourite = isFavourite;
        }
    }
}
