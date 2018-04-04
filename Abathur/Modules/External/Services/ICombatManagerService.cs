using Abathur.Model;

namespace Abathur.Modules.External.Services {
    public interface ICombatManagerService {
        void Execute(CombatRequest request);
    }
}