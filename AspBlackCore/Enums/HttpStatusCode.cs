using AspBlackCore.Attributes;

namespace AspBlackCore.Enums;

public enum HttpStatusCode
{
    [StatusDescription("OK")]
    Ok = 200,
    [StatusDescription("Not Found")]
    NotFound = 404,
    [StatusDescription("Request Entity Too Large")]
    RequestEntityTooLarge = 413,
    [StatusDescription("Bad Request")]
    BadRequest = 400,
    [StatusDescription("Internal Server Error")]
    InternalServerError = 500
}