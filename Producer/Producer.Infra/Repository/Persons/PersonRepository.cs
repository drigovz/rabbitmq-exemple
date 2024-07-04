namespace Producer.Infra.Repository.Persons;

public class PersonRepository : BaseRepository<Person, Guid>, IPersonRepository
{
    public PersonRepository(AppDbContext context)
        : base(context)
    { }
}
