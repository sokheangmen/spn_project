using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models;

namespace MyAPI.Services
{
    public class PostService
    {
        private readonly ApplicationDbContext _db;

        public PostService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Posts>> GetAllPosts()
        {
            return await _db.Posts
                .Include(p => p.User)
                .Where(p => p.status)
                .OrderByDescending(p => p.created_at)
                .ToListAsync();
        }

        public async Task<Posts?> GetPostById(int id)
        {
            return await _db.Posts.Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id && p.status);
        }
    }
}
