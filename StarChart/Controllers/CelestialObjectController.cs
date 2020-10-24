using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);

            if (celestialObject is null)
            {
                return NotFound();
            }

            FillSatellites(celestialObject);

            return Ok(celestialObject);
        }


        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = GetCelestialObjectsWithSatellites(obj => obj.Name == name);

            if (!celestialObjects.Any())
            {
                return NotFound();
            }

            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = GetCelestialObjectsWithSatellites(obj => true);

            if (!celestialObjects.Any())
            {
                return NotFound();
            }

            return Ok(celestialObjects);
        }

        private void FillSatellites(Models.CelestialObject celestialObject)
        {
            celestialObject.Satellites = _context.CelestialObjects.Where(obj => obj.OrbitedObjectId == celestialObject.Id).ToList();
        }

        private List<Models.CelestialObject> GetCelestialObjectsWithSatellites(Expression<Func<Models.CelestialObject, bool>> predicate)
        {
            var celestialObjects = _context.CelestialObjects.Where(predicate).ToList();

            celestialObjects.ForEach(celestialObject => FillSatellites(celestialObject));
            return celestialObjects;
        }

    }
}
