namespace OpenVN.Application.Dto.Cpanel
{
    public class RoleDto
    {
        public string Id { get; set; }

        public string RoleCode { get; set; }

        public string RoleName { get; set; }

        public List<ActionDto> Actions { get; set; }
    }
}
