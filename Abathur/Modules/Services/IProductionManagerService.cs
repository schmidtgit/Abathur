using NydusNetwork.API.Protocol;

namespace Abathur.Modules.Services {
    public interface IProductionManagerService {
        void Execute(ProductionRequest request);
    }
}