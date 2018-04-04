using Abathur.Constants;
using Abathur.Core;
using NydusNetwork.API.Protocol;
using System.Linq;

namespace Abathur.Modules
{
    class AutoSupply : IModule {
        private IIntelManager intelManager;
        private IProductionManager productionManager;
        public AutoSupply(IIntelManager intelManager, IProductionManager productionManager) {
            this.intelManager = intelManager;
            this.productionManager = productionManager;
        }
        void IModule.OnStep() {
            if(intelManager.Common.FoodCap >= 200)
                return;
            if(intelManager.Common.FoodUsed < intelManager.Common.FoodCap)
                return;
            if(intelManager.ProductionQueue.Where(u => u.UnitId == Supply()).FirstOrDefault() != null)
                return;
            productionManager.QueueUnitImportant(Supply());
        }

        private uint Supply() {
            switch(intelManager.ParticipantRace) {
                case Race.Terran:
                    return BlizzardConstants.Unit.SupplyDepot;
                case Race.Zerg:
                    return BlizzardConstants.Unit.Overlord;
                case Race.Protoss:
                    return BlizzardConstants.Unit.Pylon;
                case Race.NoRace:
                case Race.Random:
                default:
                    throw new System.ArgumentException("Race is unknown!");
            }
        }

        void IModule.Initialize() { }
        void IModule.OnStart() { } 
        void IModule.OnGameEnded() { }
        void IModule.OnRestart() { }
    }
}
