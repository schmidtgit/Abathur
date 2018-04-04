using Abathur.Core;
using Abathur.Core.Intel;
using NydusNetwork.API.Protocol;
using System.Collections.Generic;
using System.Linq;
using Abathur.Core.Combat;
using Abathur.Model;
using Abathur.Repositories;

namespace Abathur.Modules.External.Services {
    public class IntelManagerService : IIntelManagerService {
        private IIntelManager _intel;
        private ISquadRepository _squadRepo;
        private ICollection<IntelEvent> _events;

        public IntelManagerService(IIntelManager intel, ISquadRepository squadRepository) {
            _intel = intel;
            _squadRepo = squadRepository;
            _events = new List<IntelEvent>();
        }

        public void RegisterForEvents() {
            _intel.Handler.RegisterHandler(Case.MineralDepleted,HandleMineralDepleted);
            _intel.Handler.RegisterHandler(Case.UnitDestroyed,HandleUnitDestroyed);
            _intel.Handler.RegisterHandler(Case.AddedHiddenEnemy,HandleAddedHiddenEnemy);
            _intel.Handler.RegisterHandler(Case.StructureAddedEnemy,HandleStructureAddedEnemy);
            _intel.Handler.RegisterHandler(Case.StructureAddedSelf,HandleStructureAddedSelf);
            _intel.Handler.RegisterHandler(Case.StructureDestroyed,HandleStructureDestroyed);
            _intel.Handler.RegisterHandler(Case.UnitAddedEnemy,HandleUnitAddedEnemy);
            _intel.Handler.RegisterHandler(Case.UnitAddedSelf,HandleUnitAddedSelf);
            _intel.Handler.RegisterHandler(Case.WorkerAddedEnemy,HandleWorkerAddedEnemy);
            _intel.Handler.RegisterHandler(Case.WorkerAddedSelf,HandleWorkerAddedSelf);
            _intel.Handler.RegisterHandler(Case.WorkerDestroyed,HandleWorkerDestroyed);
        }

        private void HandleWorkerDestroyed(IUnit unit) {
            _events.Add(new IntelEvent{CaseType = CaseType.WorkerDestroyed,UnitTag = unit.Tag});
        }

        private void HandleWorkerAddedSelf(IUnit unit) {
            _events.Add(new IntelEvent { CaseType = CaseType.WorkerAddedSelf,UnitTag = unit.Tag });
        }

        private void HandleWorkerAddedEnemy(IUnit unit) {
            _events.Add(new IntelEvent { CaseType = CaseType.WorkerAddedEnemy,UnitTag = unit.Tag });
        }

        private void HandleUnitAddedSelf(IUnit unit) {
            _events.Add(new IntelEvent { CaseType = CaseType.UnitAddedSelf,UnitTag = unit.Tag });
        }

        private void HandleUnitAddedEnemy(IUnit unit) {
            _events.Add(new IntelEvent { CaseType = CaseType.UnitAddedEnemy,UnitTag = unit.Tag });
        }

        private void HandleStructureDestroyed(IUnit unit) {
            _events.Add(new IntelEvent { CaseType = CaseType.StructureDestroyed,UnitTag = unit.Tag });
        }

        private void HandleStructureAddedSelf(IUnit unit) {
            _events.Add(new IntelEvent { CaseType = CaseType.StructureAddedSelf,UnitTag = unit.Tag });
        }

        private void HandleStructureAddedEnemy(IUnit unit) {
            _events.Add(new IntelEvent { CaseType = CaseType.StructureAddedEnemy,UnitTag = unit.Tag });
        }

        private void HandleAddedHiddenEnemy(IUnit unit) {
            _events.Add(new IntelEvent { CaseType = CaseType.AddedHiddenEnemy,UnitTag = unit.Tag });
        }

        private void HandleUnitDestroyed(IUnit unit) {
            _events.Add(new IntelEvent { CaseType = CaseType.UnitDestroyed,UnitTag = unit.Tag });
        }

        private void HandleMineralDepleted(IUnit unit) {
            _events.Add(new IntelEvent { CaseType = CaseType.MineralDepleted,UnitTag = unit.Tag });
        }

        public IntelResponse BundleIntel(IntelRequest request) {
            AbathurMap map = null;
            if(request.Map)
                map = new AbathurMap { };
            Score score = null;
            if(request.Score)
                score = _intel.CurrentScore;
            PlayerCommon playerCommon = null;
            if(request.Common)
                playerCommon = _intel.Common;
            ICollection<UpgradeData> upgradeSelf = null;
            if (request.UpgradesSelf)
                upgradeSelf = _intel.UpgradesSelf;
            ICollection<IUnit> alliedBuilding = null;
            if(request.BuildingsSelf)
                alliedBuilding = _intel.StructuresSelf().ToList();
            ICollection<IUnit> alliedWorkers = null;
            if(request.WorkersSelf)
                alliedWorkers = _intel.WorkersSelf().ToList();
            ICollection<IUnit> alliedUnits = null;
            if(request.WorkersSelf)
                alliedUnits = _intel.UnitsSelf().ToList();
            ICollection<IUnit> structuresEnemy = null;
            if(request.StructuresEnemy)
                structuresEnemy = _intel.WorkersEnemy().ToList();
            ICollection<IUnit> unitsEnemy = null;
            if(request.UnitsEnemy)
                unitsEnemy = _intel.WorkersEnemy().ToList();
            ICollection<IUnit> workersEnemy = null;
            if (request.WorkersEnemy)
                workersEnemy = _intel.WorkersEnemy().ToList();
            IColony primaryColony = null;
            if (request.PrimaryColony)
                primaryColony = _intel.PrimaryColony;
            IEnumerable<IColony> colonies = null;
            if (request.Colonies)
                colonies = _intel.Colonies;
            ICollection<IUnit> mineralFields = null;
            if(request.MineralFields)
                mineralFields = _intel.MineralFields.ToList();
            ICollection<IUnit> vespeneGeysers = null;
            if(request.VespeneGeysers)
                vespeneGeysers = _intel.VespeneGeysers.ToList();
            ICollection<IUnit> destructibles = null;
            if(request.Destructibles)
                destructibles = _intel.Destructibles().ToList();
            ICollection<UnitTypeData> productionQueue = null;
            if (request.ProductionQueue)
                productionQueue = _intel.ProductionQueue.ToList();
            IEnumerable<Squad> squads = null;
            if (request.Squads)
                squads = _squadRepo.Get();
            uint gameloop = 0;
            if (request.GameLoop)
                gameloop = _intel.GameLoop;

            var events = _events.ToList();
            _events.Clear();

            return new IntelResponse {
                Map = map,
                Score = score,
                Common = playerCommon,
                UpgradesSelf = { upgradeSelf },
                BuildingsSelf = { Convert(alliedBuilding) },
                WorkersSelf = { Convert(alliedWorkers) },
                UnitsSelf = { Convert(alliedUnits) },
                StructuresEnemy = { Convert(structuresEnemy) },
                WorkersEnemy = { Convert(workersEnemy) },
                UnitsEnemy = { Convert(unitsEnemy) },
                PrimaryColony = Convert(primaryColony),
                Colonies = { Convert(colonies.ToList()) },
                MineralFields = { Convert(mineralFields) },
                VespeneGeysers = { Convert(vespeneGeysers) },
                Destructibles = { Convert(destructibles) },
                ProductionQueue = { productionQueue },
                Squads = { squads.Select(Convert) },
                GameLoop = gameloop,
                Events = {events}
            };
        }

        private static IEnumerable<Unit> Convert(ICollection<IUnit> units) {
            return units.Select(u => ((IntelUnit)u).DataSource);
        }

        private static Unit Convert(IUnit unit) {
            return ((IntelUnit)unit).DataSource;
        }

        private static IEnumerable<ColonyData> Convert(ICollection<IColony> colonies) {
            return colonies.Select(c => Convert(c));
        }

        private static ColonyData Convert(IColony colony)
        {
            return new ColonyData
            {
                ColId = colony.Id,
                Point = colony.Point,
                IsStartingLocation = colony.IsStartingLocation,
                Minerals = {Convert(colony.Minerals.ToList())},
                Vespene = {Convert(colony.Vespene.ToList())},
                Structures = {Convert(colony.Structures)},
                Workers = {Convert(colony.Workers)},
                DesiredVespeneWorkers = colony.DesiredVespeneWorkers,
            };
        }

        private static SquadData Convert(Squad squad)
        {
            return new SquadData
            {
                Name = squad.Name,
                SquadId = squad.Id,
                Units = { Convert(squad.Units.ToList())}
            };
        }

    }
}
