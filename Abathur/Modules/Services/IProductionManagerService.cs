using Abathur.Model;

namespace Abathur.Modules.Services {
    public interface IProductionManagerService {
        void Execute(ProductionRequest request);
    }
}