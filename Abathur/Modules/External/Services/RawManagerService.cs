using Abathur.Core;
using Abathur.Model;
using NydusNetwork.API.Protocol;

namespace Abathur.Modules.External.Services {
    public class RawManagerService : IRawManagerService {
        private IRawManager _manager;

        public RawManagerService(IRawManager manager) {
            _manager = manager;
        }
        public Response Execute(RawRequest request) {
            if(request.GetResponse) {
                if(_manager.TryWaitRawRequest(request.Request,out var response)) //TODO if observation request is sent, risks stealing gamesteps observation and freezing the game
                {
                    return response;
                }

            }
            _manager.SendRawRequest(request.Request);
            return null;
        }
    }
}
