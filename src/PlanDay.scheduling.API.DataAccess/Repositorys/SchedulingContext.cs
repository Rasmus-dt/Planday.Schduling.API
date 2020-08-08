using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PlanDay.scheduling.API.Core.Models;

namespace PlanDay.scheduling.API.DataAccess
{
    public class SchedulingContext : DbContext
    {
        public SchedulingContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Employee> Employees { get; set; }

    }
}
