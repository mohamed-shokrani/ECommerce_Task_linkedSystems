namespace Api.Errors;

public class ApiResponse
{
	public int StatusCode { get; set; }
    public string Message { get; set; }


    public ApiResponse( int statusCode,string message=null)
	{
        StatusCode = statusCode;
        Message = message ?? GetDefaulMessageForStatusCode(statusCode);

    }

    private string GetDefaulMessageForStatusCode(int statusCode)
    {
        return statusCode switch
        {
            400 => "A bad request, you have made",
            401 => "Authorized, you are not",
            404 => "Resource Found, it was not",
            500 => "Server Error, the errors are the path to the dark side errors need to anger" +
            " anger needs to hate hate leeds to career change",
            _ =>""
        };
    }
}
