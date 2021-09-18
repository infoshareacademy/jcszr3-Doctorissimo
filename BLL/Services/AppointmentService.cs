﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BLL.DTO;
using BLL.IServices;
using DAL.Enums;
using DAL.IRepositories;
using DAL.Models;

namespace BLL.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IMapper _mapper;

        public AppointmentService(IAppointmentRepository appointmentRepository, IPatientRepository patientRepository, IMapper mapper, IDoctorRepository doctorRepository, IRoomRepository roomRepository)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _mapper = mapper;
            _doctorRepository = doctorRepository;
            _roomRepository = roomRepository;
        }

        public async Task<List<AppointmentDto>> GetAllAppointmentsAsync()
        {
            var appointments = await _appointmentRepository.GetAllAppointmentsAsync();
            return _mapper.Map<List<Appointment>, List<AppointmentDto>>(appointments);
        }

        public bool CheckIfAppointmentIsBooked(int id)
        {
            var appointmentStatus = _appointmentRepository.GetSelectedAppointmentStatus(id);
            return appointmentStatus == AppointmentStatus.Booked;
        }

        public async Task<AppointmentDto> GetByIdAsync(int? id)
        {
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(id);
            var appointmentDto = _mapper.Map<Appointment, AppointmentDto>(appointment);
            var doctor = await _doctorRepository.GetDoctorByIdAsyncTask(appointment.DoctorId);
            var patient = await _patientRepository.GetPatientByIdAsync(appointment.PatientId);
            var room = await _roomRepository.GetRoomByIdAsync(appointment.RoomId);
            appointmentDto.DoctorDto = _mapper.Map<Doctor, DoctorDto>(doctor);
            appointmentDto.PatientDto = _mapper.Map<Patient, PatientDto>(patient);
            appointmentDto.RoomDto = _mapper.Map<Room, RoomDto>(room);
            return appointmentDto;
        }

     public Task CreateAsync(AppointmentDto appointmentDto)
        {
            var appointment = new Appointment
            {
                AppointmentTime = appointmentDto.AppointmentTime,
                RoomId = appointmentDto.RoomId,
                DoctorId = appointmentDto.DoctorId,
                AppointmentStatus = AppointmentStatus.Available
            };
            return _appointmentRepository.CreateNewAppointmentAsync(appointment);
        }
        public Task DeleteAsync(int id) => _appointmentRepository.DeleteAppointmentAsync(id);
        public Task UpdateAsync(int id, AppointmentDto appointmentDto)
        {
            var appointment = new Appointment()
            {
                RoomId = appointmentDto.RoomDto.Id,
                AppointmentStatus = appointmentDto.AppointmentStatus,
                AppointmentTime = appointmentDto.AppointmentTime,
                DoctorId = appointmentDto.DoctorDto.Id,
                Id = appointmentDto.Id,
                PatientId = appointmentDto.PatientDto.Id,
            };
            return _appointmentRepository.UpdateAppointmentAsync(id, appointment);
        }

        public bool CheckIfExists(int? id) => _appointmentRepository.CheckIfAppointmentExists(id);
        public async Task AssignPatientToAppointment(int id, int patientId)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            var patient = await _patientRepository.GetPatientByIdAsync(patientId);
            appointment.PatientId = patient.Id;
            appointment.AppointmentStatus = AppointmentStatus.Booked;
            await _appointmentRepository.UpdateAppointmentAsync(id, appointment);
        }
    }
}
