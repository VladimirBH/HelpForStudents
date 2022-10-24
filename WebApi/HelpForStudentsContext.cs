using Microsoft.EntityFrameworkCore;

namespace WebApi
{
    public class HelpForStudentsContext : DbContext
    {
        public HelpForStudentsContext (DbContextOptions<HelpForStudentsContext> options)
            : base(options)    
        {
            
        }
    }
}