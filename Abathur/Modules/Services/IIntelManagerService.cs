using NydusNetwork.API.Protocol;

namespace Abathur.Modules.Services {
    public interface IIntelManagerService {
        IntelResponse BundleIntel(IntelRequest request);
    }
}