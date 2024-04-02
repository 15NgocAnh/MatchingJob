using AutoMapper;
using MatchingJob.Entities;
using MatchingJob.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing.Printing;
using System.Security.Cryptography;
using System.Text;
using MatchingJob.DAL;
using static System.Net.Mime.MediaTypeNames;

namespace MatchingJob.BLL
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public static int PAGE_SIZE { get; set; } = 3;

        public UserRepository(AppDbContext context, IMapper mapper) 
        { 
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<UsersDTO>> findAllActive()
        {
            var listUsers = await _context.Users!
                .Where(u => u.IsDeleted == false)
                .ToListAsync(); ;
            return _mapper.Map<List<UsersDTO>>(listUsers);
        }

        public async Task<UsersDTO> findById(Guid id)
        {
            var user = await _context.Users!.FindAsync(id);
            return _mapper.Map<UsersDTO>(user);
        }

        public async Task<UsersDTO> save(UsersDTO userDTO)
        {
            var user = _mapper.Map<User>(userDTO);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return _mapper.Map<UsersDTO>(user);
        }

        public async Task update(Guid id, UsersDTO userDTO)
        {
            var user = await _context.Users!.SingleOrDefaultAsync(u => u.Id == id);
            if (user != null)
            {
                _context.Users.Update(_mapper.Map<User>(userDTO));
                await _context.SaveChangesAsync();
            }
        }

        public async Task delete(Guid id)
        {
            var user = await _context.Users!.SingleOrDefaultAsync(u => u.Id == id);
            if (user != null)
            {
                user.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task active(Guid id)
        {
            var user = await _context.Users!.SingleOrDefaultAsync(u => u.Id == id);
            if (user != null)
            {
                user.IsDeleted = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<UsersDTO>> search(string? keyword, string? sortBy, bool? gender, int? pageNumber) // true: male, false: female
        {
            var listUsers = _context.Users.AsQueryable();

            #region searching
            if (!string.IsNullOrEmpty(keyword))
            {
                listUsers = listUsers.Where(u => u.FirstName.Contains(keyword) || u.LastName.Contains(keyword));
            }
            #endregion

            #region sorting
            //default sorting by name
            listUsers = listUsers.OrderBy(u => u.FirstName + u.LastName);
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch(sortBy)
                {
                    case "location_desc": listUsers = listUsers.OrderByDescending(u => u.Location); break;
                    case "education_asc": listUsers = listUsers.OrderBy(u => u.Education); break;
                }
            }
            #endregion

            #region filtering
            if(gender != null)
            {
                listUsers = listUsers.Where(u => u.IsMale == gender);
            }
            #endregion

            #region paging
            var result = await PaginatedList<User>.CreateAsync(listUsers, pageNumber ?? 1, PAGE_SIZE);
            #endregion

            List<UsersDTO> listUsersDTO = _mapper.Map<List<UsersDTO>>(result);
            return listUsersDTO;
        }

        public async Task<List<UsersDTO>> getUsersByName(string? name)
        {
            var users = await _context.Users
                                    .FromSql($"getUsersByName {name}")
                                    .ToListAsync();
            List<UsersDTO> usersDTOs = _mapper.Map<List<UsersDTO>>(users);
            return usersDTOs;
        }
    }
}
