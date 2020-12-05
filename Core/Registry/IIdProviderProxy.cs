using System.Threading.Tasks;

namespace Hopper.Core
{
    public interface IIdProviderProxy
    {
        Task<int> GetCurrentId();
    }

    public class IdProviderProxy : IIdProviderProxy
    {
        private int m_currentId = 0;

        public Task<int> GetCurrentId()
        {
            var task = new Task<int>(() => m_currentId);
            return task;
        }
    }
}