using IARS.Data;
using IARS.Models;
using Microsoft.EntityFrameworkCore;

namespace IARS.Services
{
    public class KaizenService
    {
        private readonly ApplicationDbContext _context;

        public KaizenService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Ambil semua data proposal
        public async Task<List<KaizenProposal>> GetAllAsync()
        {
            return await _context.KaizenProposals.ToListAsync();
        }

        // Simpan data baru
        public async Task<bool> CreateAsync(KaizenProposal proposal)
        {
            _context.KaizenProposals.Add(proposal);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}