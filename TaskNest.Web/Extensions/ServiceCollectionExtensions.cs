namespace TaskNest.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Database");

            // Register DbContext 
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));


            // Register your services
            services.AddScoped<IBoardService, BoardService>();
            services.AddScoped<IBoardColumnService, BoardColumnService>();
            services.AddScoped<ITaskItemService, TaskItemService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IBoardUserService, BoardUserService>();


            // Register Generic Repository
            services.AddScoped(typeof(IRepositories.IGenericRepository<>), typeof(Repositories.GenericRepository<>));


            // Register AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));



            return services;
        }
    }
}
