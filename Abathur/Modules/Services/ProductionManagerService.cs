using Abathur.Core;
using Abathur.Model;

namespace Abathur.Modules.Services
{
    public class ProductionManagerService : IProductionManagerService {
        private IProductionManager _manager;

        public ProductionManagerService(IProductionManager manager)
        {
            _manager = manager;
        }
        public void Execute (ProductionRequest request) {
            switch(request.CallCase) {
                case ProductionRequest.CallOneofCase.None:
                    break;
                case ProductionRequest.CallOneofCase.ClearBuildOrder:
                    _manager.ClearBuildOrder();
                    break;
                case ProductionRequest.CallOneofCase.QueueUnit:
                    _manager.QueueUnit(request.QueueUnit.UnitId,request.QueueUnit.Pos,request.QueueUnit.Spacing,request.QueueUnit.Skippable);
                    break;
                case ProductionRequest.CallOneofCase.QueueTech:
                    _manager.QueueTech(request.QueueTech.UpgradeId, request.QueueTech.Skippable);
                    break;
            }
        }
    }
}
