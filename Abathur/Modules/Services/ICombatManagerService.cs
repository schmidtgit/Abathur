using NydusNetwork.API.Protocol;

namespace Abathur.Modules.Services {
    public interface ICombatManagerService {
        void Execute(CombatRequest request);
    }
}