using NydusNetwork.API.Protocol;

namespace Abathur.Modules.Services {
    public interface IRawManagerService {
        void Execute(RawRequest request);
    }
}