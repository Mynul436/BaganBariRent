using BaganBari.Data;
using BaganBari.Dto;
using BaganBari.Logging;
using BaganBari.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BaganBari.Controllers
{
    // [Route("api/[controller]")]
    [Route("api/VillaAPI")]
    [ApiController]

    public class VillaAPIController : ControllerBase
    {
        private readonly ILogging logger;

        public VillaAPIController(ILogging logger)
        {
            this.logger = logger;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<VillaDto>> GetVillas()
        {
            logger.Log("Getting all villas","");
            return VillaStore.villaList;
        }
        [HttpGet("{id:int}", Name = "GetVilla")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //  [ProducesResponseType(200)]
        public ActionResult<VillaDto> GetById(int id)
        {
            if (id == 0)
            {
                logger.Log("Get error with the Id " + id,"error");
                return BadRequest();
            }
            if (id == null)
            {
                return NotFound();
            }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            return Ok(villa);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDto> CreateDto([FromBody] VillaDto villaDto)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if (VillaStore.villaList
                .FirstOrDefault(u => u.Name.ToLower() ==
            villaDto.Name.ToLower()) != null)
            {
                ModelState.AddModelError("Custom", "Villa Alreay exist");
                return BadRequest(ModelState);
            }


            if (villaDto == null)
            {
                return BadRequest(villaDto);
            }
            if (villaDto.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            villaDto.Id = VillaStore.villaList
                .OrderByDescending(u => u.Id)
                .FirstOrDefault().Id + 1;
            VillaStore.villaList.Add(villaDto);
            return CreatedAtRoute("GetVilla", new { id = villaDto.Id }, villaDto);

        }
        
        

        [HttpDelete("{id:int}", Name = "DeleteVila")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeleteVila(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var villa=VillaStore.villaList.FirstOrDefault(u=>u.Id == id);
 
            if(villa == null)
                return NotFound();
            VillaStore.villaList.Remove(villa);

            return NoContent();
        }

        
        [HttpPut("{id:int}", Name = "DeleteVila")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]

        public IActionResult UpdateVilla(int id, [FromBody]VillaDto villaDto)
        {
            if(villaDto==null|| id != villaDto.Id)
            {
                return BadRequest();
            }

            var villa = VillaStore.villaList
                .FirstOrDefault(u => u.Id == id);
            villa.Name=villaDto.Name;
            villa.Sqft = villaDto.Sqft;
            villa.Occupancy = villaDto.Occupancy;

            return NoContent();

        }

        [HttpPatch("{id:int}", Name = "UpdatePartialVila")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdatePartialVila(int id,JsonPatchDocument<VillaDto> patcDto)
        {
            if (patcDto == null || id == 0)
            {
                return BadRequest();
            }
            var villa=VillaStore.villaList.FirstOrDefault(u => u.Id == id); 
            if(villa==null)
                return BadRequest();
            patcDto.ApplyTo(villa,ModelState);
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            return NoContent();
        }

    }
}

       