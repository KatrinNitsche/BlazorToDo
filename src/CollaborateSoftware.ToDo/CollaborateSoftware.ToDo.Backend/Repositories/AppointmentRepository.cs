using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Repositories
{
    public class AppointmentRepository : BaseRepository<Appointment>
    {
        public AppointmentRepository(DataContext context) : base(context) { }

        public override IEnumerable<Appointment> GetAll() => Context.Appointments.Include(c => c.Category);

        public override IQueryable<Appointment> GetAll(bool asyn = true) => Context.Appointments.Include(c => c.Category);

        public override Appointment GetById(int id) => Context.Appointments.FirstOrDefault(a => a.Id == id);

        public override Appointment ToggleActive(int id)
        {
            throw new NotImplementedException();
        }
    }
}
