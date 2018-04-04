using Abathur.Core;
using Abathur.Core.Combat;
using Abathur.Core.Intel;
using Abathur.Core.Intel.Map;
using Abathur.Core.Production;
using Abathur.Core.Raw;
using Abathur.Modules;
using Abathur.Modules.External;
using Abathur.Repositories;
using Microsoft.Extensions.DependencyInjection;
using NydusNetwork;
using NydusNetwork.API.Protocol;
using NydusNetwork.Logging;
using NydusNetwork.Model;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using Abathur.Modules.External.Services;

namespace Abathur.Factory {
    public class AbathurFactory {
        private ILogger log;
        public AbathurFactory(ILogger log = null) { this.log = log; }

        /// <summary>
        /// Create a new Abathur.
        /// </summary>
        /// <param name="gameSettings">Used for setting up the game</param>
        /// <param name="essence">Vital patch-dependent information for running the core modules</param>
        /// <param name="logger">Logger used for warnings and errors in Abathur</param>
        /// <param name="assembly">Assembly to fetch IModules from</param>
        /// <param name="modules">Class names of IModules to include in Abathur</param>
        /// <returns></returns>
        public IAbathur Create(GameSettings gameSettings,Essence essence,ILogger logger,Assembly assembly,params string[] modules) {
            var moduleList = new List<Type>();
            var launchStrings = new Queue<string>();
            foreach(string s in modules)
                if(GetType<IModule>(assembly,s,out var type)) {
                    moduleList.Add(type);
                    log?.LogSuccess($"AbathurFactory: {s} resolved to a valid class.");
                } else {
                    launchStrings.Enqueue(s);
                    moduleList.Add(typeof(ExternalModule));
                    log?.LogWarning($"AbathurFactory: [EXTERNAL] {s}");
                }

            var replaceable = GetTypes<IReplaceableModule>(assembly).ToArray();
            var result = (Abathur) Create(gameSettings,essence,logger,moduleList, replaceable);

            foreach(var m in result.Modules)
                if(m is ExternalModule)
                    ((ExternalModule)m).Command = launchStrings.Dequeue();
            return result;
        }

        /// <summary>
        /// Create a new Abathur.
        /// </summary>
        /// <param name="gameSettings">Used for setting up the game</param>
        /// <param name="essence">Vital patch-dependent information for running the core modules</param>
        /// <param name="logger">Logger used for warnings and errors in Abathur</param>
        /// <param name="modules">IModules to include in Abathur</param>
        /// <param name="types">Addition types to include in dependency injection</param>
        /// <returns></returns>
        public IAbathur Create(GameSettings gameSettings, Essence essence, ILogger logger, IEnumerable<Type> modules, params Type[] types) {
            var sp = ConfigureServices(gameSettings, essence, logger, modules, types);
            var result = (Abathur) sp.GetService<IAbathur>();
            result.Modules = sp.GetServices<IModule>().ToList();
            return result;
        }

        private static IEnumerable<Type> GetTypes<T>(Assembly assembly) {
            var info = typeof(T).GetTypeInfo(); // Access everything in Abathur
            return info.Assembly.GetTypes().Concat(assembly.GetTypes()) // and the provided Assembly
                .Where(x => x != typeof(T))
                .Where(x => info.IsAssignableFrom(x));
        }

        private static bool GetType<T>(Assembly assembly, string classname, out Type type) {
            var info = typeof(T).GetTypeInfo(); // Access everything in Abathur
            type = info.Assembly.GetTypes().Concat(assembly.GetTypes()) // and the provided Assembly
                .Where(x => x != typeof(T))
                .Where(x => info.IsAssignableFrom(x))
                .Where(x => x.Name == classname)
                .FirstOrDefault();
            return type == null ? false : true;
        }

        private static IServiceProvider ConfigureServices(GameSettings gameSettings, Essence essence, ILogger log, IEnumerable<Type> modules, params Type[] types) {
            var collection = new ServiceCollection();
            // Add settings, data and logger.
            collection.AddSingleton(essence);
            collection.AddSingleton(gameSettings);
            collection.AddSingleton(log);

            // Add Abathur core modules
            collection.AddSingleton<IGameClient,GameClient>();
            collection.AddSingleton<IIntelManager,IntelManager>();
            collection.AddSingleton<IRawManager,RawManager>();
            collection.AddSingleton<IProductionManager,ProductionManager>();
            collection.AddSingleton<ICombatManager,CombatManager>();
            collection.AddSingleton<ISquadRepository,CombatManager>();
            collection.AddSingleton<IGameMap,GameMap>();
            collection.AddSingleton<ITechTree,TechTree>();

            // Add Abathur data repositories
            collection.AddSingleton<DataRepository,DataRepository>();
            collection.AddSingleton<IUnitTypeRepository>(x => x.GetService<DataRepository>());
            collection.AddSingleton<IUpgradeRepository>(x => x.GetService<DataRepository>());
            collection.AddSingleton<IBuffRepository>(x => x.GetService<DataRepository>());
            collection.AddSingleton<IAbilityRepository>(x => x.GetService<DataRepository>());

            // Add services used for external modules (Python)
            collection.AddSingleton<IRawManagerService,RawManagerService>();
            collection.AddSingleton<IProductionManagerService,ProductionManagerService>();
            collection.AddSingleton<ICombatManagerService,CombatManagerService>();
            collection.AddSingleton<IIntelManagerService,IntelManagerService>();

            // Add all modules used by Abathur
            foreach(var type in modules)
                collection.AddSingleton(typeof(IModule),type);

            // Add additional modules (e.g. replaceble modules)
            foreach(var type in types)
                collection.AddScoped(type,type);

            // Add Abathur
            collection.AddSingleton<IAbathur,Abathur>();
            return collection.BuildServiceProvider();
        }
    }
}
