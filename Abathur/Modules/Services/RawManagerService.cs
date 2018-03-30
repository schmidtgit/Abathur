using Abathur.Core;
using Abathur.Model;

namespace Abathur.Modules.Services
{
    public class RawManagerService : IRawManagerService {
        private IRawManager _manager;

        public RawManagerService(IRawManager manager)
        {
            _manager = manager;
        }
        public void Execute (RawRequest request) {
                _manager.SendRawRequest(request.Request);
        }
    }
}
