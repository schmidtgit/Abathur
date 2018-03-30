using System.Linq;
using Abathur.Core;
using Abathur.Model;
using Abathur.Repositories;

namespace Abathur.Modules.Services
{
    public class CombatManagerService : ICombatManagerService {
        private ISquadRepository _repository;
        private ICombatManager _manager;
        private IIntelManager _intel;

        public CombatManagerService(IIntelManager intel, ICombatManager manager, ISquadRepository repository)
        {
            _intel = intel;
            _manager = manager;
            _repository = repository;
        }
        public void Execute (CombatRequest request) {
            switch(request.CommandCase) {
                case CombatRequest.CommandOneofCase.MoveUnit:
                    _manager.Move(request.MoveUnit.UnitTag,request.MoveUnit.Point,request.MoveUnit.Queue);
                    break;
                case CombatRequest.CommandOneofCase.MoveSquad:
                    _manager.Move(_repository.Get().First(s => s.Id.Equals(request.MoveSquad.Squad)),
                        request.MoveSquad.Point, request.MoveSquad.Queue);
                    break;
                case CombatRequest.CommandOneofCase.AttackMoveUnit:
                    _manager.AttackMove(request.AttackMoveUnit.UnitTag, request.AttackMoveUnit.Point,
                        request.AttackMoveUnit.Queue);
                    break;
                case CombatRequest.CommandOneofCase.AttackMoveSquad:
                    _manager.AttackMove(_repository.Get().First(s => s.Id.Equals(request.AttackMoveSquad.Squad)),
                        request.AttackMoveSquad.Point, request.AttackMoveSquad.Queue);
                    break;
                case CombatRequest.CommandOneofCase.AttackUnit:
                    _manager.Attack(request.AttackUnit.SourceUnit, request.AttackUnit.TargetUnit,
                        request.AttackUnit.Queue);
                    break;
                case CombatRequest.CommandOneofCase.AttackSquad:
                    _manager.Attack(_repository.Get().First(s => s.Id.Equals(request.AttackSquad.Squad)),
                        request.AttackSquad.TargetUnit, request.AttackSquad.Queue);
                    break;
                case CombatRequest.CommandOneofCase.UseTargetedAbilityUnit:
                    _manager.UseTargetedAbility(request.UseTargetedAbilityUnit.AbilityId,
                        request.UseTargetedAbilityUnit.SourceUnit, request.UseTargetedAbilityUnit.TargetUnit,
                        request.UseTargetedAbilityUnit.Queue);
                    break;
                case CombatRequest.CommandOneofCase.UseTargetedAbilitySquad:
                    _manager.UseTargetedAbility(request.UseTargetedAbilitySquad.AbilityId,
                        _repository.Get().First(s => s.Id.Equals(request.UseTargetedAbilitySquad.Squad)),
                        request.UseTargetedAbilitySquad.TargetUnit, request.UseTargetedAbilitySquad.Queue);
                    break;
                case CombatRequest.CommandOneofCase.UsePointCenteredAbilityUnit:
                    _manager.UsePointCenteredAbility(request.UsePointCenteredAbilityUnit.AbilityId,
                        request.UsePointCenteredAbilityUnit.SourceUnit, request.UsePointCenteredAbilityUnit.Point);
                    break;
                case CombatRequest.CommandOneofCase.UsePointCenteredAbilitySquad:
                    _manager.UsePointCenteredAbility(request.UsePointCenteredAbilitySquad.AbilityId,
                        _repository.Get().First(s => s.Id.Equals(request.UsePointCenteredAbilitySquad.Squad)),
                        request.UsePointCenteredAbilitySquad.Point, request.UsePointCenteredAbilitySquad.Queue);
                    break;
                case CombatRequest.CommandOneofCase.UseTargetlessAbilityUnit:
                    _manager.UseTargetlessAbility(request.UseTargetlessAbilityUnit.AbilityId,
                        request.UseTargetlessAbilityUnit.SourceUnit, request.UseTargetlessAbilityUnit.Queue);
                    break;
                case CombatRequest.CommandOneofCase.UseTargetlessAbilitySquad:
                    _manager.UseTargetlessAbility(request.UseTargetedAbilitySquad.AbilityId,
                        _repository.Get().First(s => s.Id.Equals(request.UseTargetedAbilitySquad.Squad)),
                        request.UseTargetedAbilitySquad.Queue);
                    break;
                case CombatRequest.CommandOneofCase.SmartMoveUnit:
                    if (_intel.TryGet(request.SmartMoveUnit.UnitTag, out IUnit unit))
                        _manager.SmartMove(unit, request.SmartMoveUnit.Point, request.SmartMoveUnit.Queue);
                    break;
                case CombatRequest.CommandOneofCase.SmartMoveSquad:
                    _manager.SmartMove(_repository.Get().First(s => s.Id.Equals(request.SmartMoveSquad.Squad)),
                        request.SmartMoveSquad.Point, request.SmartMoveSquad.Queue);
                    break;
                case CombatRequest.CommandOneofCase.SmartAttackMoveUnit:
                    if(_intel.TryGet(request.SmartMoveUnit.UnitTag,out IUnit u))
                        _manager.SmartAttackMove(u,request.SmartAttackMoveUnit.Point,request.SmartAttackMoveUnit.Queue);
                    break;
                case CombatRequest.CommandOneofCase.SmartAttackMoveSquad:
                    _manager.SmartAttackMove(
                        _repository.Get().First(s => s.Id.Equals(request.SmartAttackMoveSquad.Squad)),
                        request.SmartAttackMoveSquad.Point, request.SmartAttackMoveSquad.Queue);
                    break;
                case CombatRequest.CommandOneofCase.SmartAttackUnit:
                    if(_intel.TryGet(request.SmartAttackUnit.SourceUnit,out IUnit un))
                        _manager.SmartAttack(un, request.SmartAttackUnit.TargetUnit, request.SmartAttackUnit.Queue);
                    break;
                case CombatRequest.CommandOneofCase.SmartAttackSquad:
                    _manager.SmartAttack(_repository.Get().First(s => s.Id.Equals(request.SmartAttackSquad.Squad)),
                        request.SmartAttackSquad.TargetUnit, request.SmartAttackSquad.Queue);
                    break;
                case CombatRequest.CommandOneofCase.SquadRequest:
                    var addUnits = request.SquadRequest.AddUnits;
                    var removeUnits = request.SquadRequest.RemoveUnits;
                    var createSquad = request.SquadRequest.CreateSquad;
                    var removeSquad = request.SquadRequest.RemoveSquad;
                    if (request.SquadRequest.AddUnits!=null)
                    {
                        foreach (var tag in addUnits.Tags){
                            if(_intel.TryGet(tag, out var addedUnit))
                            {
                                _repository.Get().First(s => s.Id.Equals(request.SquadRequest.AddUnits.SquadId))
                                    .AddUnit(addedUnit);
                            }
                        }
                    }
                    if (removeUnits != null)
                    {
                        foreach(var tag in removeUnits.Tags) {
                            if(_intel.TryGet(tag,out var removedUnit)) {
                                _repository.Get().First(s => s.Id.Equals(request.SquadRequest.RemoveUnits.SquadId))
                                    .RemoveUnit(removedUnit);
                            }
                        }
                    }
                    if (createSquad != null)
                    {
                        _repository.Create(createSquad.Squad.Name, createSquad.Squad.SquadId);
                    }
                    if (removeSquad != null)
                    {
                        _repository.Remove(removeSquad.SquadId);
                    }
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }
    }
}

