namespace OpenVN.Application
{
    public class CreateSpendingCommand : BaseInsertCommand<string>
    {
        public SpendingDto SpendingDto { get; }

        public CreateSpendingCommand(SpendingDto spendingDto)
        {
            SpendingDto = spendingDto;
        }
    }
}
