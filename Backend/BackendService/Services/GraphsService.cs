using BackendCommonLibrary.Interfaces.Services;
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

        private IRequestsService RequestsService { get; }


        public GraphsService(ILoggerFactory loggerFactory, IConfiguration configuration, IRequestsService requestsService)
        {
            Logger = loggerFactory.CreateLogger<GraphsService>();
            Configuration = configuration;
            RequestsService = requestsService;
        }

        public async Task<GraphResponseWrapper> GetGraph(GraphRequestWrapper request)
        {
            await Task.CompletedTask;

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
                .Column<int, int, int, IConvertible, IConvertible>
                (
                    Keys: new[] { 1, 2, 3, 4, 5, 6 },
                    values: new[] { 1, 3, 2, 12, 3, 4 }
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