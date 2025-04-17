using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using tvscheduler.Models;
using Microsoft.Extensions.Configuration;

namespace tvscheduler.Utilities;

/// <summary>
/// Manages image processing operations for TV show images, including resizing and storage
/// </summary>
public class ImageManager
{
    private readonly IWebHostEnvironment _environment;
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;
    private const int MaxWidth = 360;  // Maximum dimensions for resized images
    private const int MaxHeight = 200;
    private readonly string _baseImageUrl;
    private readonly string _uploadsPath;

    public ImageManager(IWebHostEnvironment environment, AppDbContext context, IConfiguration configuration)
    {
        _environment = environment;
        _context = context;
        _httpClient = new HttpClient();
        _baseImageUrl = configuration["ImageServer:BaseUrl"] ?? "https://tvguide.co.uk";
        
        // Initialize uploads directory for resized images
        _uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "resized");
        Directory.CreateDirectory(_uploadsPath);
    }


    /// Downloads, resizes, and saves an image while maintaining aspect ratio
    public async Task<string> ResizeAndSaveImage(string imageUrl)
    {
        try
        {
            // Construct absolute URL from relative path
            string absoluteUrl = $"{_baseImageUrl.TrimEnd('/')}/{imageUrl.TrimStart('/')}";

            // Download the image
            var imageBytes = await _httpClient.GetByteArrayAsync(absoluteUrl);
            
            // Generate a unique filename
            var fileName = $"{Guid.NewGuid()}.jpg";
            var filePath = Path.Combine(_uploadsPath, fileName);

            // Process and save the image
            using (var image = Image.Load(imageBytes))
            {
                // Calculate dimensions while preserving aspect ratio
                var ratioX = (double)MaxWidth / image.Width;
                var ratioY = (double)MaxHeight / image.Height;
                var ratio = Math.Min(ratioX, ratioY);

                var newWidth = (int)(image.Width * ratio);
                var newHeight = (int)(image.Height * ratio);

                image.Mutate(x => x.Resize(newWidth, newHeight));
                await image.SaveAsJpegAsync(filePath);
            }
            
            return $"/uploads/resized/{fileName}";
        }
        catch (Exception ex)
        {
            // Log the error and return null
            Console.WriteLine($"Error processing image {imageUrl}: {ex.Message}");
            return null;
        }
    }


    /// Processes and resizes images for all shows in the database
    public async Task ProcessAllShowImages()
    {
        var shows = await _context.Shows.ToListAsync();
        
        foreach (var show in shows)
        {
            if (string.IsNullOrEmpty(show.ResizedImageUrl) && !string.IsNullOrEmpty(show.ImageUrl))
            {
                var resizedUrl = await ResizeAndSaveImage(show.ImageUrl);
                show.ResizedImageUrl = resizedUrl;
            }
        }
        await _context.SaveChangesAsync();
    }


    /// Removes all resized images from storage and clears references in the database
    public async Task DeleteAllResizedImages()
    {
        try
        {
            if (Directory.Exists(_uploadsPath))
            {
                var files = Directory.GetFiles(_uploadsPath);
                foreach (var file in files)
                {
                    File.Delete(file);
                }
                Console.WriteLine($"Deleted {files.Length} resized image files");
            }
            else
            {
                Console.WriteLine("Resized images directory does not exist");
            }

            // Clear image references from database
            var shows = await _context.Shows.ToListAsync();
            int updatedCount = 0;
            
            foreach (var show in shows)
            {
                if (!string.IsNullOrEmpty(show.ResizedImageUrl))
                {
                    show.ResizedImageUrl = null;
                    updatedCount++;
                }
            }

            await _context.SaveChangesAsync();
            Console.WriteLine($"Cleared ResizedImageUrl from {updatedCount} shows in the database");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting resized images: {ex.Message}");
            throw;
        }
    }
} 