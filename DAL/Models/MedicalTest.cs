using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DAL.Entities;

namespace DAL.Models
{
    public class MedicalTest:IEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [DisplayName("Test time")]
        public DateTime TestDateTime { get; set; }
        [DisplayName("Appointment id")]
        public int AppointmentId { get; set; }
        public string Patient { get; set; }
        public string Doctor { get; set; }
        public string Operator { get; set; }
    }
}
