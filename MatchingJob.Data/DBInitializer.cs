using MatchingJob.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchingJob.Data
{
    public class DBInitializer
    {
        private readonly ModelBuilder _modelBuilder;

        public DBInitializer(ModelBuilder modelBuilder) {
            _modelBuilder = modelBuilder;
        }

        public void seedData()
        {
            _modelBuilder.Entity<User>().HasData(
                new User()
                {
                    Id = Guid.NewGuid(),
                    FirstName = "John",
                    LastName = "Doe",
                    UserName = "johndoe",
                    Password = "John123@doe",
                    Email = "johndoe@example.com",
                    BirthDay = new DateTime(2015, 12, 25),
                    Education = "Bachelor's Degree",
                    Location = "New York",
                    Experience = "1 years as a Fontend Dev",
                    PhoneNumber = "+1 123-456-7890",
                    Skills = null,
                    IsDeleted = false,
                    IsEmailConfirmed = false,
                    IsLocked = false,
                    IsMale = true,
                    Roles = null
                },
                new User()
                {
                    Id = Guid.NewGuid(),
                    FirstName = "A",
                    LastName = "Nguyen Van",
                    UserName = "nguyena123",
                    Password = "12345NguyenA@",
                    Email = "nguyenvana@gmail.com",
                    BirthDay = new DateTime(2015, 12, 25),
                    Education = "Hue University",
                    Location = "Hue, Viet Nam",
                    Experience = "3 years as a Backend Developer",
                    PhoneNumber = "086 3458 471",
                    Skills = null,
                    IsDeleted = false,
                    IsEmailConfirmed = false,
                    IsLocked = false,
                    IsMale = true,
                    Roles = null
                },
                new User()
                {
                    Id = Guid.NewGuid(),
                    FirstName = "B",
                    LastName = "Nguyen Thi",
                    UserName = "nguyenthib123",
                    Password = "12345ThiB@",
                    Email = "nguyenthib123@gmail.com",
                    BirthDay = new DateTime(2015, 12, 25),
                    Education = "Hue University",
                    Location = "Hue, Viet Nam",
                    Experience = "4 years as a Backend Developer",
                    PhoneNumber = "086 3643 874",
                    Skills = null,
                    IsDeleted = false,
                    IsEmailConfirmed = false,
                    IsLocked = false,
                    IsMale = false,
                    Roles = null
                }
            );
            _modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "ROLE_ADMIN" },
                new Role { Id = 2, Name = "ROLE_USER" },
                new Role { Id = 3, Name = "ROLE_GUEST" }
            );
            _modelBuilder.Entity<Skill>().HasData(
                new Skill { Id = 1, Name = "BACKEND" },
                new Skill { Id = 2, Name = "FONTEND" }
            );
        }
    }
}
