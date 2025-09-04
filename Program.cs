using MedicalSystem.Repositories;
using MedicalSystem.Repositories.Interfaces;
using MedicalSystem.Services;
using MedicalSystem.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);


// Register Repository Factory (Factory pattern) - NO Entity Framework
builder.Services.AddSingleton<IRepositoryFactory, RepositoryFactory>();

// Register individual repositories through factory (temporary for controllers that inject directly)
builder.Services.AddScoped<IPatientRepository>(provider =>
{
    var factory = provider.GetRequiredService<IRepositoryFactory>();
    return factory.CreatePatientRepository();
});

builder.Services.AddScoped<IMedicalDocumentationRepository>(provider =>
{
    var factory = provider.GetRequiredService<IRepositoryFactory>();
    return factory.CreateMedicalDocumentationRepository();
});

builder.Services.AddScoped<IExaminationRepository>(provider =>
{
    var factory = provider.GetRequiredService<IRepositoryFactory>();
    return factory.CreateExaminationRepository();
});

builder.Services.AddScoped<IPrescriptionRepository>(provider =>
{
    var factory = provider.GetRequiredService<IRepositoryFactory>();
    return factory.CreatePrescriptionRepository();
});

builder.Services.AddScoped<IMedicalImageRepository>(provider =>
{
    var factory = provider.GetRequiredService<IRepositoryFactory>();
    return factory.CreateMedicalImageRepository();
});

// Register Services (Business logic layer)
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IMedicalImageService, MedicalImageService>();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();

// Add model validation
builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );
        return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(new
        {
            Message = "Validation failed",
            Errors = errors
        });
    };
});

// Add CORS for frontend communication
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// VAŽNO: Redoslijed je bitan!
app.UseDefaultFiles();     // Premjesti prije UseStaticFiles
app.UseStaticFiles();      // Ovo mora biti nakon UseDefaultFiles

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();