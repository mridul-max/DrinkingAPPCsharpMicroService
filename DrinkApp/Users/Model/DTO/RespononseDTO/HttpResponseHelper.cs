using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
public static class HttpResponseHelper
{
    public static HttpResponseData CreateResponseData(HttpRequestData req, HttpStatusCode statusCode, object content)
    {
        var response = req.CreateResponse();
        if (content != null)
        {
            response.WriteAsJsonAsync(content);
        }
        response.StatusCode = statusCode;
        return response;
    }
}
