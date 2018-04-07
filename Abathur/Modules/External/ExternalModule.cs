using System.Diagnostics;
using System.Threading.Tasks;
using Abathur.Model;
using Abathur.Modules.External.Services;

namespace Abathur.Modules.External {
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
        private bool _onlyAsync;
        private bool _restart;
        private IAbathur _abathur;

        public ExternalModule(ICombatManagerService combatService,IIntelManagerService intelService,IProductionManagerService productionService,IRawManagerService rawService, IAbathur abathur) {
            _combatService = combatService;
            _intelService = intelService;
            _productionService = productionService;
            _rawService = rawService;
            _abathur = abathur;
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
            _connection = new NydusWormConnection(RequestHandler);
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
            if(_onlyAsync)
                return true;

            _marker = new Task(() => { });
            _connection.SendMessage(new AbathurResponse {
                Notification = new Notification {
                    Type = type
                },
                Intel = _intelService.BundleIntel(_intelSettings)
            });
            return _marker.Wait(TIMEOUT);
        }

        void IModule.OnStart()
        {
            _restart = false;
            _intelService.RegisterForEvents();
            _marker = new Task(() => { });
            _connection.SendMessage(new AbathurResponse {
                Notification = new Notification {
                    Type = NotificationType.GameStart
                },
                Intel = _intelService.BundleIntel(_intelSettings)
            });
            _marker.Wait(TIMEOUT);
        }

        void IModule.OnStep()
        {
            WaitForGameStep();
            if(_restart) {
                _abathur.Restart();
            }
        } 

        void IModule.OnGameEnded()
        {
            _marker = new Task(() => { });
            _connection.SendMessage(new AbathurResponse { Notification = new Notification { Type = NotificationType.GameEnded } });
            _marker.Wait(TIMEOUT);
            if(_restart) {
                _abathur.Restart();
            }
        }

        void IModule.OnRestart()
        {
            _marker = new Task(() => { });
            _connection.SendMessage(new AbathurResponse { Notification = new Notification { Type = NotificationType.Restart } });
            _marker.Wait(TIMEOUT);
            _restart = false;
        } 
        private void RequestHandler(byte[] data) {
            var request = AbathurRequest.Parser.ParseFrom(data);
            /*
            if(request.OnlyAsync && request.Restart == null) {
                _onlyAsync = true;
                if(!_marker.IsCompleted)
                    _marker.RunSynchronously();
            }
            */
            if(_onlyAsync && request.Intel != null) {
                _intelSettings = request.Intel;
                _connection.SendMessage(new AbathurResponse {
                    Notification = new Notification {
                        Type = NotificationType.GameStep
                    },
                    Intel = _intelService.BundleIntel(_intelSettings)
                });
            }

            foreach(var combatRequest in request.Combat)
                _combatService.Execute(combatRequest);
            foreach(var productionRequest in request.Production)
                _productionService.Execute(productionRequest);

            if(request.Raw != null) {
                var resp = _rawService.Execute(request.Raw);
                if(resp != null) {
                    _connection.SendMessage(new AbathurResponse { RawResponse = resp });
                }
            } else if(_marker != null && !_onlyAsync)
                _marker.RunSynchronously();
            /*
            if (request.Restart != null)
            {
                _restart = true;
                if(_marker != null && !_marker.IsCompleted)
                    _marker.RunSynchronously();
            }
            */
        }
    }
}
