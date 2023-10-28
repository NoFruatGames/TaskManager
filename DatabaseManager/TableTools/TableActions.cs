using DatabaseManager.Tables;
using Microsoft.EntityFrameworkCore;

namespace DatabaseManager.TableTools
{
    public class TableActions<T> where T : class, IDBTable
    {
        protected readonly DbSet<T> table;
        internal TableActions(DbSet<T> table)
        {
            this.table = table;
        }

        public List<T> GetAll()
        {
            return table.ToList();
        }
        public async Task<List<T>> GetAllAsync()
        {
            return await table.ToListAsync();
        }
        public int GetCount()
        {
            return table.Count();
        }
        public async Task<int> GetCountAsync()
        {
            return await table.CountAsync();
        }

        public void AddRecord(T record)
        {
            table.Add(record);
        }
        public async Task AddRecordAsync(T record)
        {
            await table.AddAsync(record);
        }
        public T? GetRecordById(int id)
        {
            return table.FirstOrDefault(t => t.Id == id);
        }
        public async Task<T?> GetRecordByIdAsync(int id)
        {
            return await table.FirstOrDefaultAsync(t => t.Id == id);
        }

        public void RemoveRecordById(int id)
        {
            T? record = GetRecordById(id);
            if (record is null) return;
            table.Remove(record);
        }
        public async Task RemoveRecordByIdAsync(int id)
        {
            T? record = await GetRecordByIdAsync(id);
            if (record is null) return;
            table.Remove(record);
        }

    }
}
