using Abathur.Model;

namespace Abathur.Modules.External.Services {
    public interface IIntelManagerService {
        IntelResponse BundleIntel(IntelRequest request);
        void RegisterForEvents();
    }
}