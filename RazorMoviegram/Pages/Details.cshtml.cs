using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorMoviegram.Model;

namespace RazorMoviegram.Pages;

public class DetailsModel(MovieContext context) : PageModel
{
    public Movie Movie { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var movie = await context.Movies
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (movie is null)
            return NotFound();

        Movie = movie;

        return Page();
    }
}