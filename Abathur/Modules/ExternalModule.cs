using Abathur.Core;
using Abathur.Modules.Services;
using NydusNetwork;
using NydusNetwork.API.Protocol;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Abathur.Modules
{
    public class ExternalModule : IModule {
        private const int TIMEOUT = 100000000; //ms

        public string Command { get; set; }
        private NydusWormConnection _connection;
        private IntelRequest _intelSettings;
        private Task _marker;
        private ICombatManagerService _combatService;
        private IIntelManagerService _intelService;
        private IProductionManagerService _productionService;
        private IRawManagerService _rawService;
        
        public ExternalModule(ICombatManagerService combatService, IIntelManagerService intelService, IProductionManagerService productionService, IRawManagerService rawService) {
            _combatService = combatService;
            _intelService = intelService;
            _productionService = productionService;
            _rawService = rawService;
        }

        void IModule.Initialize() {
            _intelSettings = new IntelRequest {
                Map = true,
                Score = true,
                Common = true,
                UpgradesSelf = true,
                BuildingsSelf = true,
                WorkersSelf = true,
                UnitsSelf = true,
                StructuresEnemy = true,
                UnitsEnemy = true,
                WorkersEnemy = true,
                PrimaryColony = true,
                Colonies = true,
                MineralFields = true,
                VespeneGeysers = true,
                Destructibles = true,
                ProductionQueue = true,
                Squads = true,
            };
            _connection = new NydusWormConnection(b => RequestHandler(b));
            _connection.Initialize();

            Process.Start(new ProcessStartInfo {
                FileName = "CMD.exe",
                Arguments = $"/k{Command} {_connection.Port}",
                UseShellExecute = true
            });

            _connection.Connect();
            _connection.SendMessage(new AbathurResponse { Notification = new Notification { Type = NotificationType.Initialize } });
        }

        private bool WaitForGameStep(NotificationType type = NotificationType.GameStep) {
            _marker = new Task(() => { });
            _connection.SendMessage(new AbathurResponse {
                Notification = new Notification {
                    Type = type
                },
                Intel = _intelService.BundleIntel(_intelSettings)
            });
            return _marker.Wait(TIMEOUT);
        }

        void IModule.OnStart()    => WaitForGameStep(NotificationType.GameStart);
        void IModule.OnStep()     => WaitForGameStep();
        void IModule.OnGameEnded()    => _connection.SendMessage(new AbathurResponse { Notification = new Notification { Type = NotificationType.GameEnded } });
        void IModule.OnRestart()      => _connection.SendMessage(new AbathurResponse { Notification = new Notification { Type = NotificationType.Restart } });
        private void RequestHandler(byte[] data) {
            var request = AbathurRequest.Parser.ParseFrom(data);
            if(request.Intel != null)
                _intelSettings = request.Intel;
            foreach (var combatRequest in request.Combat)
                _combatService.Execute(combatRequest);
            foreach (var productionRequest in request.Production)
                _productionService.Execute(productionRequest);
            if (request.Raw != null)
                _rawService.Execute(request.Raw);
            if (_marker != null)
                _marker.RunSynchronously();
        }
    }
}
