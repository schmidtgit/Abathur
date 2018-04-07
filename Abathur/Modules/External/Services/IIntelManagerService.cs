using Abathur.Model;
using NydusNetwork.API.Protocol;

namespace Abathur.Modules.External.Services {
    public interface IIntelManagerService {
        IntelResponse BundleIntel(IntelRequest request);
        void RegisterForEvents();
    }
}