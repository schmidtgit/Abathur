using Abathur.Model;

namespace Abathur.Modules.Services {
    public interface IRawManagerService {
        void Execute(RawRequest request);
    }
}