using Abathur.Core;
using Abathur.Core.Raw;
using NydusNetwork.API.Protocol;

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
