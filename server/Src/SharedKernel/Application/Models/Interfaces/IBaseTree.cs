using System.Reflection.Metadata.Ecma335;

namespace SharedKernel.Application
{
    public interface IBaseTree
    {
        public string Code { get; set; }

        public string RefCode { get; set; }

        public List<IBaseTree> Subs { get; set; }
    }
}
