using BaganBari.Data;
using BaganBari.Dto;
using BaganBari.Logging;
using BaganBari.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace BaganBari.Controllers
{
    // [Route("api/[controller]")]
    [Route("api/VillaAPI")]
    [ApiController]

    public class VillaAPIController : ControllerBase
    {
        private readonly ApplicationDbcontext _db;

        public VillaAPIController(ApplicationDbcontext db)
        {
            _db = db;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
       // [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<VillaDto>> GetVillas()
        {
            return Ok(_db.Villas.ToList());
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
                return BadRequest();
            }
            if (id == null)
            {
                return NotFound();
            }
            var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
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

            if (_db.Villas
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
            Villa model = new Villa()
            {
                Amenity=villaDto.Amenity,
                Details=villaDto.Details,
                Id=villaDto.Id,
                Name=villaDto.Name,
                ImageUrl=villaDto.ImageUrl,
                Occupancy=villaDto.Occupancy,
                Rate=villaDto.Rate,
                Sqft=villaDto.Sqft

            };
                _db.Villas.Add(model);
            _db.SaveChanges();
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

            var villa=_db.Villas.FirstOrDefault(u=>u.Id == id);
 
            if(villa == null)
                return NotFound();
            _db.Villas.Remove(villa);
            _db.SaveChanges();
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

            //var villa = _db.Villas
            //    .FirstOrDefault(u => u.Id == id);
            //villa.Name=villaDto.Name;
            //villa.Sqft = villaDto.Sqft;
            //villa.Occupancy = villaDto.Occupancy;
            Villa model = new Villa()
            {
                Amenity = villaDto.Amenity,
                Details = villaDto.Details,
                Id = villaDto.Id,
                Name = villaDto.Name,
                ImageUrl = villaDto.ImageUrl,
                Occupancy = villaDto.Occupancy,
                Rate = villaDto.Rate,
                Sqft = villaDto.Sqft

            };
            _db.Villas.Update(model);
            _db.SaveChanges();
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
            var villa=_db.Villas.AsNoTracking().FirstOrDefault(u => u.Id == id);
            VillaDto villaDto = new ()
            {
                Amenity = villa.Amenity,
                Details = villa.Details,
                Id = villa.Id,
                Name = villa.Name,
                ImageUrl = villa.ImageUrl,
                Occupancy = villa.Occupancy,
                Rate = villa.Rate,
                Sqft = villa.Sqft

            };
            if (villa==null)
                return BadRequest();
            patcDto.ApplyTo(villaDto,ModelState);
            Villa model = new Villa()
            {
                Amenity = villaDto.Amenity,
                Details = villaDto.Details,
                Id = villaDto.Id,
                Name = villaDto.Name,
                ImageUrl = villaDto.ImageUrl,
                Occupancy = villaDto.Occupancy,
                Rate = villaDto.Rate,
                Sqft = villaDto.Sqft

            };
            _db.Update(model);
            _db.SaveChanges();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return NoContent();
        }

    }
}

       