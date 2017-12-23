using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Threading;
using System.Threading.Tasks;
using Coralcode.EntityFramework.ModelConfiguration;
using Coralcode.Framework.Data;
using Coralcode.Framework.Data.SofeDelete;
using EntityFramework.DynamicFilters;

namespace Coralcode.EntityFramework.UnitOfWork
{
    public class CoralDbContext : DbContext, IDbModelCacheKeyProvider
    {
        private readonly DbInitContext _context;

        //private ConcurrentDictionary<string, object> _allSet = new ConcurrentDictionary<string, object>();

        public CoralDbContext(DbInitContext context)
            : base(context.ConnectiongString)
        {
            _context = context;
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = true;
            if (context.Config.CommandTimeout != 0)
                Database.CommandTimeout = context.Config.CommandTimeout;


            if (context.Config.AutoMerageDataBase)
                Database.SetInitializer(new MigrateDatabaseToLatestVersion<CoralDbContext, DbMigrationsConfiguration<CoralDbContext>>(true, new DbMigrationsConfiguration<CoralDbContext>
                {
                    AutomaticMigrationsEnabled = true,
                    AutomaticMigrationDataLossAllowed = false
                }));
            else
                Database.SetInitializer<CoralDbContext>(null);
        }

        public override int SaveChanges()
        {
            ApplyConcepts();
            return base.SaveChanges();
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            ApplyConcepts();
            return base.SaveChangesAsync(cancellationToken);
        }
        //public DbSet<TEntity> CreateSet<TEntity>()
        //    where TEntity : class
        //{
        //    return _allSet.GetOrAdd(typeof(TEntity).FullName, key => Set<TEntity>()) as DbSet<TEntity>;
        //}

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Remove unused conventions
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            //配置类型
            EntityConfigManager.ConfigDbTypes(modelBuilder, _context);
            modelBuilder.Filter("SoftDelete", (ISoftDelete d) => d.IsDeleted, false);
        }

        public string CacheKey => _context.GetIdentity();

        private void ApplyConcepts()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        break;
                    case EntityState.Deleted:
                        HandleSoftDelete(entry);
                        break;
                }
            }
        }

        private void HandleSoftDelete(DbEntityEntry entry)
        {
            if (!(entry.Entity is ISoftDelete))
            {
                return;
            }
            var softDeleteEntry = entry.Cast<ISoftDelete>();
            softDeleteEntry.State = EntityState.Unchanged;
            softDeleteEntry.Entity.IsDeleted = true;
        }

    }

}
