using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIMusicaAuth_SerafinParedesAlejandro.Data;
using APIMusicaAuth_SerafinParedesAlejandro.Models;

namespace APIMusicaAuth_SerafinParedesAlejandro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistsController : ControllerBase
    {
        private readonly ChinookContext _context;

        public ArtistsController(ChinookContext context)
        {
            _context = context;
        }

        // GET: api/Artists
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Artist>>> GetArtists()
        {
          if (_context.Artists == null)
          {
              return NotFound();
          }
           var guardarArtistas =  await _context.Artists
                //incluir los albums
                .Include(a => a.Albums)
                //ordernarlo por nombre
                .OrderBy(n => n.Name)
                //recoger solo 10
                .Take(10)
                //seleccionar los id de artista, nombre y los albums id y titulo
                .Select(s => new
                {
                    s.ArtistId,
                    s.Name,
                    Albums = s.Albums.Select(a => new {a.AlbumId,a.Title})
                })
                .ToListAsync();
            
            return Ok(guardarArtistas);
        }

        // GET: api/Artists/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Artist>> GetArtist(int id)
        {
          if (_context.Artists == null)
          {
              return NotFound();
          }
            var artist = await _context.Artists.FindAsync(id);

            if (artist == null)
            {
                return NotFound();
            }

            return Ok(artist);
        }

        // PUT: api/Artists/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> PutArtist(int id, CreateArtist artistP)
        {
            //guardamos todos los datos de artista en base a la id solicitada
            var artista = await _context.Artists.FindAsync(id); 
            if (artista == null)
            {
                return NotFound();
            }
            //asignamos el nombre del artista al artista
            artista.Name = artistP.Name;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArtistExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Artists
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Artist>> PostArtist(CreateArtist artistP)
        {
          if (_context.Artists == null)
          {
              return Problem("Entity set 'ChinookContext.Artists'  is null.");
          }
          //asignar el nombre e autoincrementar la id del mismo
            var artista = new Artist { Name = artistP.Name, ArtistId = _context.Artists.Max(a => a.ArtistId)+1};
            _context.Artists.Add(artista);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ArtistExists(artista.ArtistId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtAction("GetArtist", new { id = artista.ArtistId }, artista);
        }

        // DELETE: api/Artists/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteArtist(int id)
        {
            if (_context.Artists == null)
            {
                return NotFound();
            }
            // borrado en cascada
            var artist = await _context.Artists.Include(a => a.Albums).ThenInclude(a => a.Tracks).FirstOrDefaultAsync(a => a.ArtistId == id);
            if (artist == null)
            {
                return NotFound();
            }

            _context.Artists.Remove(artist);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ArtistExists(int id)
        {
            return (_context.Artists?.Any(e => e.ArtistId == id)).GetValueOrDefault();
        }
    }
}
