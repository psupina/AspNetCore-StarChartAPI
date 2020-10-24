using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

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

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);

            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject updatedCelestialObject)
        {
            var celestialObject = _context.CelestialObjects.Find(id);

            if (celestialObject is null)
            {
                return NotFound();
            }

            celestialObject.Name = updatedCelestialObject.Name;
            celestialObject.OrbitalPeriod = updatedCelestialObject.OrbitalPeriod;
            celestialObject.OrbitedObjectId = updatedCelestialObject.OrbitedObjectId;

            _context.CelestialObjects.Update(celestialObject);

            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialObject = _context.CelestialObjects.Find(id);

            if (celestialObject is null)
            {
                return NotFound();
            }

            celestialObject.Name = name;

            _context.CelestialObjects.Update(celestialObject);

            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects.Where(obj => obj.Id == id || obj.OrbitedObjectId == id);

            if (!celestialObjects.Any())
            {
                return NotFound();
            }

            _context.CelestialObjects.RemoveRange(celestialObjects);

            _context.SaveChanges();

            return NoContent();
        }


        private void FillSatellites(CelestialObject celestialObject)
        {
            celestialObject.Satellites = _context.CelestialObjects.Where(obj => obj.OrbitedObjectId == celestialObject.Id).ToList();
        }

        private List<CelestialObject> GetCelestialObjectsWithSatellites(Expression<Func<Models.CelestialObject, bool>> predicate)
        {
            var celestialObjects = _context.CelestialObjects.Where(predicate).ToList();

            celestialObjects.ForEach(celestialObject => FillSatellites(celestialObject));
            return celestialObjects;
        }
    }
}
