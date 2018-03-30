using Abathur.Model;

namespace Abathur.Modules.Services {
    public interface ICombatManagerService {
        void Execute(CombatRequest request);
    }
}