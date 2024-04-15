using System.Text.Json.Serialization;

namespace Portmoneu.Api.Extensions
{
    public static class ControllerExtenstions
    {
        public static IServiceCollection AddControllerExtended(this IServiceCollection services) {
            services.AddControllers()
                .AddJsonOptions(options => {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });
            return services;
        }
    }
}
