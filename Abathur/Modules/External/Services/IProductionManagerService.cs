using Abathur.Model;
using NydusNetwork.API.Protocol;

namespace Abathur.Modules.External.Services {
    public interface IProductionManagerService {
        void Execute(ProductionRequest request);
    }
}