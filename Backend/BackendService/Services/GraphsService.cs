using BackendCommonLibrary.Interfaces.Services;
using CommonLibrary.Extensions;
using CommonLibrary.Interfaces.Services;
using ModelLibrary.Requests;
using ModelLibrary.Responses;
using Plotly.NET;
using Plotly.NET.LayoutObjects;

namespace BackendService.Services
{
    public class GraphsService : IGraphsService
    {
        private ILogger Logger { get; }

        private IConfiguration Configuration { get; }

        private IDataStorageService DataStorageService { get; }

        private INetworkDevicesService NetworkDevicesService { get; }

        public GraphsService(ILoggerFactory loggerFactory, IConfiguration configuration, IDataStorageService dataStorageService, INetworkDevicesService networkDevicesService)
        {
            Logger = loggerFactory.CreateLogger<GraphsService>();
            Configuration = configuration;
            DataStorageService = dataStorageService;
            NetworkDevicesService = networkDevicesService;
        }

        public async Task<GraphResponseWrapper> GetGraph(GraphRequestWrapper request)
        {
            var requestingDevices = await GetTargetDevicesAsync(request);

            var measurementsResponses = await requestingDevices
                .Select(async deviceID =>
                {
                    var dataRequest = new GetMeasurementsRequest()
                    {
                        DeviceID = deviceID,
                        MaxDate = request.MaxDateTime,
                        MinDate = request.MinDateTime
                    };

                    return await DataStorageService.GetMeasurementsAsync(dataRequest);
                })
                .WaitAllAsync()
                .ToListAsync();

            var measurements = measurementsResponses
                .SelectMany(response => response.Measurements)
                .ToList();

            var layout = Layout.init<IConvertible>
            (
                Title: Title.init("A Plotly.NET Chart"),
                PlotBGColor: Color.fromString("#e5ecf6")
            );

            var xAxis = LinearAxis.init<IConvertible, IConvertible, IConvertible, IConvertible, IConvertible, IConvertible, IConvertible, IConvertible>
            (
                Title: Title.init("xAxis"),
                ZeroLineColor: Color.fromString("#ffff"),
                GridColor: Color.fromString("#ffff"),
                ZeroLineWidth: 2
            );

            var yAxis = LinearAxis.init<IConvertible, IConvertible, IConvertible, IConvertible, IConvertible, IConvertible, IConvertible, IConvertible>
            (
                Title: Title.init("yAxis"),
                ZeroLineColor: Color.fromString("#ffff"),
                GridColor: Color.fromString("#ffff"),
                ZeroLineWidth: 2
            );

            var chart = Chart2D.Chart
                .Column<double, DateTime, int, IConvertible, IConvertible>
                (
                    Keys: measurements.Select(x => x.DateTime).ToArray(),
                    values: measurements.Select(x => x.Value ?? -1).ToArray()
                )
                .WithXAxis(xAxis)
                .WithYAxis(yAxis)
                .WithLayout(layout);

            var resultHTML = GetPlotlyHtml(chart);

            return new GraphResponseWrapper()
            {
                GraphHTML = resultHTML
            };
        }

        private async Task<IEnumerable<int>> GetTargetDevicesAsync(GraphRequestWrapper request)
        {
            if (request.NetworkID == null && request.DeviceID == null)
            {
                throw new Exception("Необходимо указать NetworkID или DeviceID");
            }

            if (request.DeviceID != null)
            {
                var deviceID = request.DeviceID!.Value;

                return await Task.FromResult(new int[] { deviceID });
            }
            else
            {
                var networkID = request.DeviceID!.Value;
                var networkDevices = await NetworkDevicesService.GetNetworkDevicesAsync(networkID);

                return networkDevices.Select(x => x.DeviceID);
            }
        }

        private static string GetPlotlyHtml(GenericChart chart)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".html");

            chart.SaveHtml(tempPath);

            var html = File.ReadAllText(tempPath);

            File.Delete(tempPath);

            return html;
        }
    }
}