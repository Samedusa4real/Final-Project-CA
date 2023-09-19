using Backend___Putka.DAL;

namespace Backend___Putka.Services
{
    public class LayoutService
    {
        private readonly PutkaDbContext _context;

        public LayoutService(PutkaDbContext context)
        {
            _context = context;
        }
        public Dictionary<string, string> GetSettings()
        {
            return _context.Settings.ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
