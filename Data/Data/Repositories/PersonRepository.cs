using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Data.Data;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        readonly TradeMarketDbContext dbContext;
        public PersonRepository(TradeMarketDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task AddAsync(Person entity)
        {
            await dbContext.Persons.AddAsync(entity);
            await dbContext.SaveChangesAsync();
        }

        public void Delete(Person entity)
        {
            dbContext.Persons.Remove(entity);
            dbContext.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var person = await dbContext.Persons.FindAsync(id);

            if (person != null)
            {
                dbContext.Persons.Remove(person);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            return await dbContext.Persons.ToListAsync();
        }

        public async Task<Person> GetByIdAsync(int id)
        {
            var person = await dbContext.Persons.FindAsync(id);
            return person;
        }

        public void Update(Person entity)
        {
            dbContext.Attach(entity);

            dbContext.Entry(entity).State = EntityState.Modified;

            dbContext.SaveChanges();
        }
    }
}
