namespace Producer.Infra.Repository;

public class BaseRepository<E, T> : IBaseRepository<E, Guid> where E : BaseEntity
{
    private readonly AppDbContext _context;

    public BaseRepository(AppDbContext context)
    {
        _context = context;
    }

    private async Task Commit() =>
        await _context.SaveChangesAsync();

    private async Task Rollback() =>
        await (await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted))
            .RollbackAsync();

    public async Task<IEnumerable<E>> GetAsync() => await _context.Set<E>().ToListAsync();

    public async Task<E> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _context.Set<E>().SingleOrDefaultAsync(_ => _.Id == id);
            return entity is not null ? entity : null;
        }
        catch (Exception ex)
        {
            throw ex.InnerException;
        }
    }

    public async Task<E> AddAsync(E entity)
    {
        try
        {
            entity.CreatedAt = DateTime.UtcNow;

            _context.Set<E>().Add(entity);
        }
        catch (Exception ex)
        {
            await Rollback();
            throw ex.InnerException!;
        }

        await Commit();
        return entity;
    }

    public async Task<E> UpdateAsync(E entity)
    {
        try
        {
            var result = await GetByIdAsync(entity.Id);
            if (result is null) return null;

            entity.UpdatedAt = DateTime.UtcNow;
            entity.CreatedAt = result.CreatedAt;

            _context.Entry(result).CurrentValues.SetValues(entity);
        }
        catch (Exception ex)
        {
            await Rollback();
            throw ex.InnerException!;
        }

        await Commit();
        return entity;
    }

    public async Task<bool> RemoveAsync(Guid id)
    {
        try
        {
            var result = await GetByIdAsync(id);
            if (result is null) return false;

            _context.Set<E>().Remove(result);
        }
        catch (Exception ex)
        {
            await Rollback();
            throw ex.InnerException!;
        }

        await Commit();
        return true;
    }
}
