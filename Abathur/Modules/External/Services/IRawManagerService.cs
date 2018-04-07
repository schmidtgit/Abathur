using Abathur.Model;
using NydusNetwork.API.Protocol;

namespace Abathur.Modules.External.Services {
    public interface IRawManagerService {
        Response Execute(RawRequest request);
    }
}