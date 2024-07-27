using Microsoft.AspNetCore.Mvc;
using PluginFramework_VectoDigital.Helpers;
using PluginFramework_VectoDigital.Models;
using PluginFramework_VectoDigital.Models.DTO;
using PluginFramework_VectoDigital.Servicies;
using System.Threading.Tasks;

namespace PluginFramework_VectoDigital.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PluginsController : ControllerBase
    {
        private readonly PluginService pluginService;

        public PluginsController(PluginService pluginService)
        {
            this.pluginService = pluginService;
        }

        [HttpGet]
        public IActionResult GetAllPlugins()
        {
            var plugins = pluginService.GetAllPlugins();
            return Ok(plugins);
        }

        [HttpPost]
        public async Task<IActionResult> AddPlugin([FromBody] PluginDTO pluginDTO)
        {
            await pluginService.AddPlugin(pluginDTO.ConvertToPlugin());
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePlugin(string id)
        {
            if (await pluginService.DeletePlugin(id))
                return Ok();

            return NotFound();
        }
    }

}
