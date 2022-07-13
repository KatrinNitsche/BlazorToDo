using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using CollaborateSoftware.MyLittleHelpers.Backend.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Services
{
    public class AppointmentService : IAppointmentService
    {
        private AppointmentRepository appointmentRepository;

        public AppointmentService(DataContext context)
        {
            appointmentRepository = new AppointmentRepository(context);
        }

        public async Task<Appointment> Add(Appointment appointment)
        {
            appointmentRepository.Add(appointment);
            appointmentRepository.Commit();
            return appointment;
        }

        public async Task<IEnumerable<Appointment>> GetAll() => appointmentRepository.GetAll();

        public async Task<IEnumerable<Appointment>> GetAll(DateTime from, DateTime to)
        {
            var result = appointmentRepository.GetAll();

           // var regularAppointments = result.Where(a => a.re)

            return result;
        }

        public async Task<Appointment> GetById(int idNumber) => appointmentRepository.GetById(idNumber);

        public async Task<bool> Remove(int id)
        {
            try
            {
                appointmentRepository.Remove(id);
                appointmentRepository.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Appointment> Update(Appointment appointment) => appointmentRepository.Update(appointment);
    }
}
