using BackendCommonLibrary.Interfaces.Services;
using BackendModelLibrary.Requests;
using BackendModelLibrary.Responses;

namespace BackendService.Services
{
    public class GraphsService : IGraphsService
    {
        private ILogger Logger { get; }

        private IConfiguration Configuration { get; }

        private IRequestsService RequestsService { get; }


        public GraphsService(ILogger logger, IConfiguration configuration, IRequestsService requestsService)
        {
            Logger = logger;
            Configuration = configuration;
            RequestsService = requestsService;
        }

        public async Task<GraphResponseWrapper> GetGraph(GraphRequestWrapper request)
        {
            return null;

            //LinearAxis xAxis = new LinearAxis();
            //xAxis.SetValue("title", "xAxis");
            //xAxis.SetValue("zerolinecolor", "#ffff");
            //xAxis.SetValue("gridcolor", "#ffff");
            //xAxis.SetValue("showline", true);
            //xAxis.SetValue("zerolinewidth", 2);

            //LinearAxis yAxis = new LinearAxis();
            //yAxis.SetValue("title", "yAxis");
            //yAxis.SetValue("zerolinecolor", "#ffff");
            //yAxis.SetValue("gridcolor", "#ffff");
            //yAxis.SetValue("showline", true);
            //yAxis.SetValue("zerolinewidth", 2);

            //Layout layout = new Layout();
            //layout.SetValue("xaxis", xAxis);
            //layout.SetValue("yaxis", yAxis);
            //layout.SetValue("title", "A Figure Specified by DynamicObj");
            //layout.SetValue("plot_bgcolor", "#e5ecf6");
            //layout.SetValue("showlegend", true);

            //Trace trace = new Trace("bar");
            //trace.SetValue("x", new[] { 1, 2, 3 });
            //trace.SetValue("y", new[] { 1, 3, 2 });


            //GenericChart.toFigure(ListModule.OfSeq(new[] { trace }));

            //var fig = GenericChart.Figure.create(ListModule.OfSeq(new[] { trace }), layout);
            //GenericChart.fromFigure(fig);
        }

        public async Task TastAsync()
        {
            //RequestsService.SendRequestAsync<string,>();


            await Task.CompletedTask;
        }
    }
}