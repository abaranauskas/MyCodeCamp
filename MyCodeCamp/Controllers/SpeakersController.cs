using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

    [Route("api/camps/{moniker}/speakers")]
    [ValidateModel]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    public class SpeakersController : BaseController
    {
        protected ICampRepository _repo;
        protected ILogger<SpeakersController> _logger;
        protected IMapper _mapper;
        protected UserManager<CampUser> _userManager;

        public SpeakersController(ICampRepository repo,
            ILogger<SpeakersController> logger,
            IMapper mapper,
            UserManager<CampUser> userManager)
        {
            _repo = repo;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        public IActionResult Get(string moniker, bool incluedeTaks = false)
        {
            var speakers = incluedeTaks ? _repo.GetSpeakersByMonikerWithTalks(moniker) : _repo.GetSpeakersByMoniker(moniker);

            //if (speakers == null) return NotFound($"Unable to find speakers by moniker: {moniker}");

            return Ok(_mapper.Map<IEnumerable<SpeakerModel>>(speakers));
        }


        [HttpGet]
        [MapToApiVersion("1.1")]
        public virtual IActionResult GetWithCount(string moniker, bool incluedeTaks = false)
        {
            var speakers = incluedeTaks ? _repo.GetSpeakersByMonikerWithTalks(moniker) : _repo.GetSpeakersByMoniker(moniker);

            //if (speakers == null) return NotFound($"Unable to find speakers by moniker: {moniker}");

            return Ok(new { count = speakers.Count(), results = _mapper.Map<IEnumerable<SpeakerModel>>(speakers) });
        }


        [HttpGet("{id}", Name = "GetSpeaker")]
        public IActionResult Get(string moniker, int id, bool incluedeTaks = false)
        {
            Speaker speaker;
            if (incluedeTaks)
            {
                speaker = _repo.GetSpeakerWithTalks(id);
            }
            else
            {
                speaker = _repo.GetSpeaker(id);
            }

            if (speaker == null) return NotFound($"Unable to find speaker by id: {id}");
            if (speaker.Camp.Moniker != moniker) return BadRequest("Speaker not in specified Camp");

            return Ok(_mapper.Map<SpeakerModel>(speaker));
        }

        [HttpPost]
        [ValidateModel]
        [Authorize]
        public async Task<IActionResult> Post(string moniker, [FromBody]SpeakerModel model)
        {
            try
            {
                var camp = _repo.GetCampByMoniker(moniker);
                if (camp == null) return BadRequest("Could not find camp");

                var speaker = _mapper.Map<Speaker>(model);
                speaker.Camp = camp;

                var campUser = await _userManager.FindByNameAsync(this.User.Identity.Name);
                if (campUser != null)
                {
                    speaker.User = campUser;
                    _logger.LogInformation("Creating a new Code Camp");

                    _repo.Add(speaker);

                    if (await _repo.SaveAllAsync())
                    {
                        var newUri = Url.Link("SpeakerGet", new { moniker = speaker.Camp.Moniker, id = speaker.Id });
                        return Created(newUri, _mapper.Map<SpeakerModel>(speaker));
                    }
                    else
                    {
                        _logger.LogWarning("could not save speaker to tadabase");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exeption throen while adding speaker: {ex}");
                return BadRequest();
            }

            return BadRequest("Unable to add new speaker!");
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(string moniker, int id, [FromBody]SpeakerModel model)
        {
            try
            {

                var speaker = _repo.GetSpeaker(id);
                if (speaker == null) return NotFound();
                if (moniker != speaker.Camp.Moniker) return BadRequest("Speaker and Camp do not match!");

                if (speaker.User.UserName != this.User.Identity.Name) return Forbid();

                _mapper.Map(model, speaker);

                if (await _repo.SaveAllAsync())
                {
                    return Ok(_mapper.Map<SpeakerModel>(speaker));
                }
                else
                {
                    _logger.LogWarning("could not save update to tadabase");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exeption throen while updating speaker: {ex}");
                return BadRequest();
            }
            return BadRequest("Unable to update speaker!");
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string moniker, int id)
        {
            try
            {
                var speaker = _repo.GetSpeaker(id);
                if (speaker == null) return NotFound();
                if (speaker.Camp.Moniker != moniker) return BadRequest("Speaker and Camp do not match!");

                if (speaker.User.UserName != this.User.Identity.Name) return Forbid();

                _repo.Delete(speaker);

                if (await _repo.SaveAllAsync())
                {
                    return Ok();
                }
                else
                {
                    _logger.LogWarning("could not save dalete to tadabase");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exeption throen while deleting speaker: {ex}");
                return BadRequest();
            }

            return BadRequest("Unable to delete speaker!");
        }
    }
}
