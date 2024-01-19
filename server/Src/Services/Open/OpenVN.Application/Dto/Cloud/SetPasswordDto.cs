namespace OpenVN.Application
{
    public class SetPasswordDto
    {
        public string DirectoryId { get; set; }

        public string Password { get; set; }
    }

    public class SetPasswordDtoValidator : AbstractValidator<SetPasswordDto>
    {
        public SetPasswordDtoValidator()
        {
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}
