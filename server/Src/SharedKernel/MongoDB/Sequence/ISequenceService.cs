using SharedKernel.Domain;
using System.Threading.Tasks;

namespace SharedKernel.MongoDB
{
    public interface ISequenceService<T> where T : BaseEntity
    {
        /// <summary>
        /// Lấy ra giá trị inc tiếp theo
        /// </summary>
        Task<long> Next();  
    }
}
