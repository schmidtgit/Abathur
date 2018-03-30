using Abathur.Model;

namespace Abathur.Modules.Services {
    public interface IIntelManagerService {
        IntelResponse BundleIntel(IntelRequest request);
    }
}