using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCodeCamp.Data;
using MyCodeCamp.Data.Entities;
using MyCodeCamp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCodeCamp.Controllers
{
    [Route("api/camps/{moniker}/Speakers")]
    [ApiVersion("2.0")]
    public class Speakers2Controller : SpeakersController
    {
        public Speakers2Controller(ICampRepository repo,
            ILogger<SpeakersController> logger,
            IMapper mapper,
            UserManager<CampUser> userManager)
            : base(repo, logger, mapper, userManager)
        {            
        }

        public override IActionResult GetWithCount(string moniker, bool incluedeTaks = false)
        {
            var speakers = incluedeTaks ? _repo.GetSpeakersByMonikerWithTalks(moniker) : _repo.GetSpeakersByMoniker(moniker);

            //if (speakers == null) return NotFound($"Unable to find speakers by moniker: {moniker}");

            return Ok(new
            {
                currentTime = DateTime.UtcNow,
                count = speakers.Count(),
                results = _mapper.Map<IEnumerable<Speaker2Model>>(speakers)
            });

        }
    }
}
