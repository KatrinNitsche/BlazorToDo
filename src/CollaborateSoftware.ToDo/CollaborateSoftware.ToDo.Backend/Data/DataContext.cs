using Microsoft.EntityFrameworkCore;
using System;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {

        }

        public DbSet<ToDoListEntry> ToDoList { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<NotesEntry> Notes { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
    }
}
