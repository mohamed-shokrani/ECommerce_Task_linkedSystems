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
             => _Context.Set<T>().AsQueryable();

        public async Task<T> GetById(int id)
            =>  await _Context.Set<T>().FindAsync(id);
        
        public async Task AddAsync(T entity)
            => await _Context.Set<T>().AddAsync(entity);

        public async Task<int> DeleteAsync(Expression<Func<T, bool>> expression)
            => await _Context.Set<T>().Where(expression).ExecuteDeleteAsync();

        public async Task<bool> Update( T updatedEntity)
        {
                    _Context.Set<T>().Update(updatedEntity);
                    return await _Context.SaveChangesAsync() > 0;
               
        }

    }
}
