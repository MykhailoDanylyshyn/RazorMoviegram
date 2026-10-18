using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorMoviegram.Model;

namespace RazorMoviegram.Pages;

public class EditModel(
    MovieContext context,
    IWebHostEnvironment appEnvironment) : PageModel
{
    [BindProperty]
    public Movie Movie { get; set; } = new();

    [BindProperty]
    public IFormFile? PosterFile { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var movie = await context.Movies.FindAsync(id);

        if (movie is null)
            return NotFound();

        Movie = movie;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (id != Movie.Id)
            return NotFound();

        var existingMovie = await context.Movies.FindAsync(id);

        if (existingMovie is null)
            return NotFound();

        if (PosterFile is not null && PosterFile.Length > 0)
        {
            var postersPath = Path.Combine(
                appEnvironment.WebRootPath, "posters");

            Directory.CreateDirectory(postersPath);

            var extension = Path.GetExtension(PosterFile.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var absolutePath = Path.Combine(postersPath, fileName);

            await using var fileStream = new FileStream(
                absolutePath, FileMode.Create);

            await PosterFile.CopyToAsync(fileStream);

            Movie.PosterPath = $"/posters/{fileName}";

            ModelState.Remove("Movie.PosterPath");
        }
        else
        {
            Movie.PosterPath = existingMovie.PosterPath;
        }

        if (!ModelState.IsValid)
            return Page();

        existingMovie.Title = Movie.Title;
        existingMovie.Year = Movie.Year;
        existingMovie.Genre = Movie.Genre;
        existingMovie.Director = Movie.Director;
        existingMovie.Description = Movie.Description;
        existingMovie.PosterPath = Movie.PosterPath;

        await context.SaveChangesAsync();

        return RedirectToPage("/Index");
    }
}