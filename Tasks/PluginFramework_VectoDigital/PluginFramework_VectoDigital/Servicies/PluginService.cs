using PluginFramework_VectoDigital.Data.Enums;
using PluginFramework_VectoDigital.Helpers;
using PluginFramework_VectoDigital.Models;
using PluginFramework_VectoDigital.Models.DTO;
using System.Collections.Concurrent;
using System.Text.Json;

namespace PluginFramework_VectoDigital.Servicies
{
    public class PluginService
    {
        private const string ConfigFilePath = "PluginConfig.json";
        private ConcurrentDictionary<string, Plugin> plugins;

        public PluginService()
        {
            if (File.Exists(ConfigFilePath))
            {
                var json = File.ReadAllText(ConfigFilePath);
                plugins = JsonSerializer.Deserialize<ConcurrentDictionary<string, Plugin>>(json)
                    ?? new ConcurrentDictionary<string, Plugin>();                
            }
            else
            {
                throw new Exception("Configuration file doesn't exist!");
            }
        }

        public ConcurrentDictionary<string, Plugin> GetAllPlugins() => plugins;

        public async Task AddPlugin(Plugin plugin)
        {
            plugins[plugin.ID] = plugin;
            await SaveChanges();
        }

        public async Task<bool> DeletePlugin(string pluginID)
        {
            if (plugins.Remove(pluginID, out _))
            {
                await SaveChanges();
                return true;
            }

            return false;
        }

        private async Task SaveChanges()
        {
            var json = JsonSerializer.Serialize(plugins);
            await File.WriteAllTextAsync(ConfigFilePath, json);
        }
    }

}
