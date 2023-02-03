using Backend_Bank.Requirements;
using Backend_Bank.Requirements.Handlers;
using Backend_Bank.Tokens;
using Database;
using Database.Interfaces;
using Database.Models;
using Database.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseNpgsql(@$"Host={Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost"};
                        Port=5432;
                        Database={Environment.GetEnvironmentVariable("POSTGRES_NAME") ?? "backend_db2"};
                        Username={Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "backend_user"};
                        Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "backend_pass"}"));

builder.Services.AddTransient<IUsersRepository, UsersRepository>();
builder.Services.AddTransient<IOrganisationsRepository, OrganisationsRepository>();
builder.Services.AddTransient<IBranchesRepository, BranchesRepository>();
builder.Services.AddTransient<IServiceRepository, ServiceRepository>();
builder.Services.AddTransient<ITokenRepository, TokenRepository>();
builder.Services.AddTransient<ILoansRepository, LoansRepository>();

builder.Services.AddTransient<IAuthorizationHandler, ObjectReqHandler>();
builder.Services.AddTransient<IAuthorizationHandler, TokenReqHandler>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy => { policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = AuthOptions.ISSUER,

                            ValidateAudience = true,
                            ValidAudience = AuthOptions.AUDIENCE,
                            ValidateLifetime = true,

                            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                            ValidateIssuerSigningKey = true,
                        };
                    });
builder.Services.AddControllersWithViews();

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Fefu Bank Project Api", Version = "v1" });
    opt.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Scheme = "bearer"
    });
    opt.OperationFilter<AuthenticationRequirementsOperationFilter>();

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policy.Access, policy =>
    {
        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new TokenRequirement(Token.Access));
    });
    options.AddPolicy(Policy.Refresh, policy =>
    {
        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new TokenRequirement(Token.Refresh));
    });

    options.AddPolicy(Policy.UserAccess, policy =>
    {
        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new ObjectRequirement(ObjectType.User));
        policy.Requirements.Add(new TokenRequirement(Token.Access));
    });
    options.AddPolicy(Policy.OrgAccess, policy =>
    {
        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new ObjectRequirement(ObjectType.Organisation));
        policy.Requirements.Add(new TokenRequirement(Token.Access));
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

//app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();
