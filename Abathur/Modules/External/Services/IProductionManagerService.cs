using Abathur.Model;

namespace Abathur.Modules.External.Services {
    public interface IProductionManagerService {
        void Execute(ProductionRequest request);
    }
}