using System.Linq.Expressions;
using WebApi.DataAccess.Contracts;


namespace WebApi.DataAccess.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly HelpForStudentsContext Context;
        protected readonly IConfiguration Configuration;

        public GenericRepository(HelpForStudentsContext context, IConfiguration configuration)
        {
            Context = context;
            Configuration = configuration;
        }
        public void Add(T entity)
        {
            Context.Set<T>().Add(entity);
        }
        public void AddRange(IEnumerable<T> entities)
        {
            Context.Set<T>().AddRange(entities);
        }
        public IEnumerable<T> Find(Expression<Func<T, bool>> expression)
        {
            return Context.Set<T>().Where(expression);
        }
        public List<T> GetAll()
        {
            return Context.Set<T>().ToList();
        }
        public T GetById(int id)
        {
            return Context.Set<T>().Find(id);
        }
        public void Remove(T entity)
        {
            Context.Set<T>().Remove(entity);
        }
        public void RemoveRange(IEnumerable<T> entities)
        {
            Context.Set<T>().RemoveRange(entities);
        }
        public void Update(T entity)
        {
            Context.Set<T>().Update(entity);
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }
    }
}