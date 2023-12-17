using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIMusicaAuth_SerafinParedesAlejandro.Data;
using APIMusicaAuth_SerafinParedesAlejandro.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace APIMusicaAuth_SerafinParedesAlejandro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumsController : ControllerBase
    {
        private readonly ChinookContext _context;

        public AlbumsController(ChinookContext context)
        {
            _context = context;
        }

        // GET: api/Albums
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Album>>> GetAlbums()
        {
          if (_context.Albums == null)
          {
              return NotFound();
          }
          var guardarAlbums = await _context.Albums
                // incluir las canciones
                .Include(t => t.Tracks)
                // incluir los artistas
                .Include(a => a.Artist)
                // ordenar de manera descendente los titulos de los albums
                .OrderByDescending(a => a.Title)
                // recoger solo 10 registros
                .Take(10)
                .Select(a => new
                {
                    // cuando se solicite el get estos seran los datos a mostrar de album
                    a.AlbumId,
                    a.Title,
                    // de artista y de album se mostraran los nombres y el id
                    Artist = new {a.Artist.ArtistId,a.Artist.Name},
                    Tracks =  a.Tracks.Select(t => new
                    {
                        t.TrackId,
                        t.Name
                    })
                })
                .ToListAsync();

            return Ok(guardarAlbums);
        }

        // GET: api/Albums/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<Album>> GetAlbum(int id)
        {
          if (_context.Albums == null)
          {
              return NotFound();
          }
            var album = await _context.Albums.FindAsync(id);

            if (album == null)
            {
                return NotFound();
            }

            return album;
        }

        // PUT: api/Albums/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Roles = "ADMINISTRADOR")]
        public async Task<IActionResult> PutAlbum(int id, CreateAlbum albumP)
        {
            // recoger todos los datos del album en base al id
            var album = await _context.Albums.FindAsync(id);
            if (album == null)
            {
                return NotFound();
            }
            // asignar el titulo del album
            album.Title = albumP.Title;
            _context.Entry(album).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlbumExists(id))
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

        // POST: api/Albums
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<Album>> PostAlbum(CreateAlbum albumP)
        {
          if (_context.Albums == null)
          {
              return Problem("Entity set 'ChinookContext.Albums'  is null.");
          }
          // almacenar el titulo y id del album a postear con el id autoincrementado
            var album = new Album { Title = albumP.Title, AlbumId = _context.Albums.Max(a => a.AlbumId) + 1 };
            _context.Albums.Add(album);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AlbumExists(album.AlbumId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAlbum", new { id = album.AlbumId }, album);
        }

        // DELETE: api/Albums/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteAlbum(int id)
        {
            if (_context.Albums == null)
            {
                return NotFound();
            }
            var album = await _context.Albums.FindAsync(id);
            if (album == null)
            {
                return NotFound();
            }

            _context.Albums.Remove(album);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AlbumExists(int id)
        {
            return (_context.Albums?.Any(e => e.AlbumId == id)).GetValueOrDefault();
        }
    }
}
