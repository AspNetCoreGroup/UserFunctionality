using System;
namespace BackendCommonLibrary.Interfaces.BackgroundServices
{
	public interface IDataEventsService
	{
        Task WorkAsync(CancellationToken stoppingToken);
    }
}

