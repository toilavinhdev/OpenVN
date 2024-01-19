namespace OpenVN.Application
{
    public class UpdateConfigCommand : BaseUpdateCommand<UserConfigDto>
    {
        public ConfigValue ConfigValue { get; }

        public UpdateConfigCommand(ConfigValue configValue)
        {
            ConfigValue = configValue;
        }
    }
}
