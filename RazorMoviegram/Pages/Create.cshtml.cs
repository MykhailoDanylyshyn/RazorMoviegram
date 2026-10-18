using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorMoviegram.Model;

namespace RazorMoviegram.Pages;

public class CreateModel(
    MovieContext context,
    IWebHostEnvironment appEnvironment) : PageModel
{
    [BindProperty]
    public Movie Movie { get; set; } = new();

    [BindProperty]
    public IFormFile? PosterFile { get; set; }

    public void OnGet()
    {
    }

    [RequestSizeLimit(100_000_000)]
    public async Task<IActionResult> OnPostAsync()
    {
        if (PosterFile is null || PosterFile.Length == 0)
        {
            ModelState.AddModelError(
                "Movie.PosterPath",
                "Будь ласка, виберіть постер");
        }
        else
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

        if (!ModelState.IsValid)
            return Page();

        context.Movies.Add(Movie);
        await context.SaveChangesAsync();

        return RedirectToPage("/Index");
    }
}