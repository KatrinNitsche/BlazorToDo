using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Data
{
    public class DataContext : IdentityDbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {

        }

        public DbSet<ToDoListEntry> ToDoList { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<NotesEntry> Notes { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Habit> Habits { get; set; }
        public DbSet<BudgetEntry> BudgetEntries { get; set; }
    }
}
