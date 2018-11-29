using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCodeCamp.Data;
using MyCodeCamp.Data.Entities;
using MyCodeCamp.Filters;
using MyCodeCamp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCodeCamp.Controllers
{
    [Authorize]
    [EnableCors("AnyGET")]
    [Route("api/[controller]")]
    [ValidateModel]
    public class CampsController : BaseController
    {
        private ICampRepository _repo;
        private ILogger<CampsController> _logger;
        private IMapper _mapper;

        public CampsController(ICampRepository repo, ILogger<CampsController> logger,
            IMapper mapper)
        {
            _repo = repo;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            var camps = _repo.GetAllCamps();

            return Ok(_mapper.Map<IEnumerable<CampModel>>(camps));
        }

        [HttpGet("{moniker}", Name = "CampGet")]
        public IActionResult Get(string moniker, bool includeSpeakers = false)
        {
            try
            {
                Camp camp = null;
                if (includeSpeakers) camp = _repo.GetCampByMonikerWithSpeakers(moniker);
                else camp = _repo.GetCampByMoniker(moniker);

                if (camp == null) return NotFound($"Camp {moniker} not found!");


                return Ok(_mapper.Map<CampModel>(camp));
            }
            catch (Exception)
            {
            }

            return BadRequest();
        }

        [EnableCors("Wildermuth")]
        [Authorize(Policy ="SuperUsers")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CampModel model)
        {
            try
            {
                //if (ModelState.IsValid) return BadRequest(ModelState);
                //pakesitas per Filters ValideteModelAttribut
                //ir pridejus [ValidateModel] atributa visam kontroleriui

                _logger.LogInformation("Creating a new Code Camp");

                var camp = _mapper.Map<Camp>(model);

                _repo.Add(camp);
                if (await _repo.SaveAllAsync())
                {
                    var newUri = Url.Link("CampGet", new { moniker = camp.Moniker });
                    return Created(newUri, _mapper.Map<CampModel>(camp));
                }
                else
                {
                    _logger.LogWarning("could not save camp to tadabase");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Threw exception while saving Camp: {ex}");
            }

            return BadRequest();
        }

        //[HttpPatch("{id}")]
        [HttpPut("{moniker}")]
        public async Task<IActionResult> Put(string moniker, [FromBody] CampModel model)
        {
            try
            {
                if (ModelState.IsValid) return BadRequest(ModelState);
                var oldCamp = _repo.GetCampByMoniker(moniker);
                if (oldCamp == null) return NotFound($"Could not find Camp with moniker: {moniker}");

                _mapper.Map(model, oldCamp);




                //oldCamp.Name = model.Name ?? oldCamp.Name;
                //oldCamp.Description = model.Description ?? oldCamp.Description;
                //oldCamp.Location = model.Location ?? oldCamp.Location;
                //oldCamp.Length = model.Length > 0 ? model.Length : oldCamp.Length;
                //oldCamp.EventDate = model.EventDate != DateTime.MinValue ? model.EventDate : oldCamp.EventDate;

                if (await _repo.SaveAllAsync())
                {
                    return Ok(_mapper.Map<CampModel>(oldCamp));
                }
                else
                {
                    _logger.LogWarning("could not update camp");
                }
            }
            catch (Exception)
            {


            }

            return BadRequest("Couldnt update Camp");
        }

        [HttpDelete("{moniker}")]
        public async Task<IActionResult> Delete(string moniker)
        {
            try
            {
                var camp = _repo.GetCampByMoniker(moniker);
                if (camp == null) return NotFound($"Could not find Camp with Moniker: {moniker}");

                _repo.Delete(camp);

                if (await _repo.SaveAllAsync())
                {
                    return Ok();
                }
                else
                {
                    _logger.LogWarning("could not delete camp");
                }
            }
            catch (Exception)
            {

            }
            return BadRequest("Couldnt delete Camp");
        }
    }
}
