using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.GenericRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : Entity
    {
        protected AppDbContext _Context;

        public GenericRepository(AppDbContext context)
        {
            _Context = context;
        }

       
        public Task GetAllAsync()
        {
            throw new NotImplementedException();
        }
        public IQueryable<T> GetAllQueryable()
        {
            IQueryable<T> query = _Context.Set<T>();
            
            return query.AsQueryable();
        }

        public async Task<T> GetById(int id)
        {
            return await _Context.Set<T>().FindAsync(id);
        }
      
      
       
        public async Task AddAsync(T entity)
        {
            try
            {
                await _Context.Set<T>().AddAsync(entity);

            }
            catch (Exception e)
            {

                throw;
            }
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        
    }
}
