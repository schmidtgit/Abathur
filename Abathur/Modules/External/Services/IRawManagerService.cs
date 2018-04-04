using Abathur.Model;

namespace Abathur.Modules.External.Services {
    public interface IRawManagerService {
        void Execute(RawRequest request);
    }
}