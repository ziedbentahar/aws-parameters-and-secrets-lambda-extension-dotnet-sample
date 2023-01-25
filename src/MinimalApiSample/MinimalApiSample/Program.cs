var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddHttpClient<ISecretsProvider, SecretsProvider>(c =>
{
    c.BaseAddress = new Uri("http://localhost:2773");
});

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", async (ISecretsProvider secretsProvider) =>
{
    var result = await secretsProvider.GetSecretAsync(Environment.GetEnvironmentVariable("SECRET_NAME"));
    return result;
});


app.Run();