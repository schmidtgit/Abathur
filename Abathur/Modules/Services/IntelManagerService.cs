using Abathur.Constants;
using Abathur.Core;
using Abathur.Core.Intel;
using Abathur.Model;
using NydusNetwork.API.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using Abathur.Core.Combat;
using Abathur.Repositories;

namespace Abathur.Modules.Services
{
    public class IntelManagerService : IIntelManagerService {
        private IIntelManager _intel;
        private ISquadRepository _squadRepo;

        public IntelManagerService(IIntelManager intel, ISquadRepository squadRepository)
        {
            _intel = intel;
            _squadRepo = squadRepository;
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

            return new IntelResponse {
                Map = map,
                Score = score,
                Common = playerCommon,
                UpgradesSelf = { upgradeSelf },
                BuildingsSelf = { Convert(alliedBuilding) },
                WorkersSelf = { Convert(alliedWorkers) },
                UnitsSelf = { Convert(alliedUnits) },
                StructuresEnemy = { Convert(structuresEnemy)},
                WorkersEnemy = { Convert(workersEnemy)},
                UnitsEnemy = { Convert(unitsEnemy)},
                PrimaryColony = Convert(primaryColony),
                Colonies = { Convert(colonies.ToList())},
                MineralFields = { Convert(mineralFields) },
                VespeneGeysers = { Convert(vespeneGeysers) },
                Destructibles = { Convert(destructibles) },
                ProductionQueue = {productionQueue},
                Squads = { squads.Select(Convert)},
                GameLoop = gameloop
            };
        }

        private static IEnumerable<Unit> Convert(ICollection<IUnit> units) {
            return units.Select(u => ((IntelUnit)u).DataSource);
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
