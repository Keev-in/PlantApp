using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;

namespace PlantApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
//			.ConfigureLifecycleEvents(x =>
//			{
//#if ANDROID
//#elif IOS14_0_OR_GREATER
//#endif

//			})
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
			builder.Services.AddSingleton<DataSyncService>(new DataSyncService(true));

		return builder.Build();
	}
}
