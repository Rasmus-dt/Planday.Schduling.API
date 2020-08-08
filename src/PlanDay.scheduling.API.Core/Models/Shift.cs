using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanDay.scheduling.API.Core.Models
{
    public class Shift
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShiftId { get; set; }
        [Required]
        public int EmployeeId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public class SwapShiftsModel
    {
        [NotMapped]
        public int ShiftId1 { get; set; }
        [NotMapped]
        public int ShiftId2 { get; set; }
    }
}
