namespace TodoListApp.WebApp.Services;

public class WebApiAuthDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public WebApiAuthDelegatingHandler(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = this.httpContextAccessor.HttpContext?.Session.GetString("WebApiToken");
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
