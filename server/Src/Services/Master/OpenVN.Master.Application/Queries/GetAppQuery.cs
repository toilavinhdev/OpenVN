using MediatR;
using OpenVN.Master.Application;

namespace OpenVN.Master.Application
{
    public class GetAppQuery : IRequest<List<AppDto>>
    {
    }
}
