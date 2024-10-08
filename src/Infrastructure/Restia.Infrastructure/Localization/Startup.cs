using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Localization;
using Restia.Infrastructure.Localization.Services;
using Restia.Infrastructure.Localization.Settings;

namespace Restia.Infrastructure.Localization;

internal static class Startup
{
	/// <summary>
	/// Add portable object localization
	/// </summary>
	/// <param name="services">The services</param>
	/// <param name="config">The configuration</param>
	/// <returns>A <see cref="IServiceCollection"/>.</returns>
	internal static IServiceCollection AddPOLocalization(this IServiceCollection services, IConfiguration config)
	{
		var localizationSettings = config.GetSection(nameof(LocalizationSettings)).Get<LocalizationSettings>();

		if (localizationSettings?.EnableLocalization is true
			&& localizationSettings.ResourcesPath is not null)
		{
			services.AddPortableObjectLocalization(options => options.ResourcesPath = localizationSettings.ResourcesPath);

			services.Configure<RequestLocalizationOptions>(options =>
			{
				if (localizationSettings.SupportedCultures != null)
				{
					var supportedCultures = localizationSettings.SupportedCultures.Select(x => new CultureInfo(x)).ToList();

					options.SupportedCultures = supportedCultures;
					options.SupportedUICultures = supportedCultures;
				}

				options.DefaultRequestCulture = new RequestCulture(localizationSettings.DefaultRequestCulture ?? "en-US");
				options.FallBackToParentCultures = localizationSettings.FallbackToParent ?? true;
				options.FallBackToParentUICultures = localizationSettings.FallbackToParent ?? true;
			});

			services.AddSingleton<ILocalizationFileLocationProvider, RestiaPoFileLocationProvider>();
		}

		return services;
	}
}