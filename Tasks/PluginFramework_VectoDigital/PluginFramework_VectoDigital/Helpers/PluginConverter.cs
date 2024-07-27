using PluginFramework_VectoDigital.Data.Enums;
using PluginFramework_VectoDigital.Models.DTO;
using PluginFramework_VectoDigital.Models;
using System.Collections.Concurrent;

namespace PluginFramework_VectoDigital.Helpers
{
    public static class PluginConverter
    {
        public static Plugin ConvertToPlugin(this PluginDTO pluginDTO)
        {
            return new(
                pluginDTO.Id,
                pluginDTO.Radius,
                pluginDTO.Size,
                Enum.TryParse(pluginDTO.Effect, out PluginEffects result) ? result : default,
                pluginDTO.ImageAddress);
        }

        public static ConcurrentDictionary<string, Plugin> ConvertToPlugins(this List<PluginDTO> pluginDTOs)
        {
            ConcurrentDictionary<string, Plugin> plugins = new();

            foreach (var i in pluginDTOs)
            {
                plugins[i.Id] = i.ConvertToPlugin();
            }

            return plugins;
        }

        public static ConcurrentDictionary<string, Plugin> ConvertToPlugins(this List<Plugin> plugins)
        {
            ConcurrentDictionary<string, Plugin> t = new();

            foreach (Plugin i in plugins)
            {
                t[i.ID] = i;
            }

            return t;
        }
    }
}
