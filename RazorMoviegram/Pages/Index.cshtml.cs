using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorMoviegram.Model;

namespace RazorMoviegram.Pages;

public class IndexModel(MovieContext context) : PageModel
{
    public IEnumerable<Movie> Movies { get; set; } = [];

    public async Task OnGetAsync()
    {
        Movies = await context.Movies
            .AsNoTracking()
            .ToListAsync();
    }
}