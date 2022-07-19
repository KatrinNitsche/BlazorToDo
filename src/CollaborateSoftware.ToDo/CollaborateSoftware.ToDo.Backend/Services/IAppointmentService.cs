using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Services
{
    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetAll(Guid userId);
        Task<IEnumerable<Appointment>> GetAll(Guid userId, DateTime from, DateTime to);
        Task<Appointment> Add(Appointment newTask);       
        Task<Appointment> GetById(int idNumber);
        Task<Appointment> Update(Appointment Appointment);
        Task<bool> Remove(int id);
    }
}
