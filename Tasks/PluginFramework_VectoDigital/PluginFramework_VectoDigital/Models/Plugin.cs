using PluginFramework_VectoDigital.Data.Enums;

namespace PluginFramework_VectoDigital.Models
{
    public record Plugin(string ID, double Radius, double Size, PluginEffects Effect,
        string ImageAddress);
}
