using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Net.Http
{
	/// <summary>
	/// 1xx Informational
	/// 
	/// Request received, continuing process.[2]
	/// This class of status code indicates a provisional response, consisting only of the Status-Line and optional headers,
	/// and is terminated by an empty line. Since HTTP/1.0 did not define any 1xx status codes, servers must not send a 1xx
	/// response to an HTTP/1.0 client except under experimental conditions.
	/// 
	/// 2xx Success
	/// 
	/// This class of status codes indicates the action requested by the client was received, understood,
	/// accepted and processed successfully.
	/// 
	/// 3xx Redirection
	/// 
	/// The client must take additional action to complete the request.[2]
	/// This class of status code indicates that further action needs to be
	/// taken by the user agent in order to fulfil the request. The action required
	/// may be carried out by the user agent without interaction with the user if and
	/// only if the method used in the second request is GET or HEAD. A user agent should
	/// not automatically redirect a request more than five times, since such redirections
	/// usually indicate an infinite loop.
	/// 
	/// 4xx Client Error
	/// 
	/// The 4xx class of status code is intended for cases in which the client seems to have erred. Except when responding to a HEAD request, the server should include an entity containing an explanation of the error situation, and whether it is a temporary or permanent condition. These status codes are applicable to any request method. User agents should display any included entity to the user.
	///
	/// 5xx Server Error
	/// 
	/// The server failed to fulfill an apparently valid request.[2]
	/// Response status codes beginning with the digit "5" indicate cases in which the server is aware that it has encountered an error or is otherwise incapable of performing the request. Except when responding to a HEAD request, the server should include an entity containing an explanation of the error situation, and indicate whether it is a temporary or permanent condition. Likewise, user agents should display any included entity to the user. These response codes are applicable to any request method.
	/// </summary>
	public enum HttpCode
	{
		/// <summary>
		/// 100 Continue
		/// This means that the server has received the request headers, and that the client
		/// should proceed to send the request body (in the case of a request for which a body
		/// needs to be sent; for example, a POST request). If the request body is large, sending
		/// it to a server when a request has already been rejected based upon inappropriate headers
		/// is inefficient. To have a server check if the request could be accepted based on the
		/// request's headers alone, a client must send Expect: 100-continue as a header in its
		/// initial request[2] and check if a 100 Continue status code is received in response before
		/// continuing (or receive 417 Expectation Failed and not continue).[2]
		/// </summary>
		CONTINUE_100 = 100,

		/// <summary>
		/// 101 Switching Protocols
		/// This means the requester has asked the server to switch protocols and the server is
		/// acknowledging that it will do so.[2]
		/// </summary>
		SWITCHING_PROTOCOLS_101 = 101,

		/// <summary>
		/// 102 Processing (WebDAV) (RFC 2518)
		/// As a WebDAV request may contain many sub-requests involving file operations,
		/// it may take a long time to complete the request. This code indicates that the server
		/// has received and is processing the request, but no response is available yet.[3] This
		/// prevents the client from timing out and assuming the request was lost.
		/// </summary>
		PROCESSING_WEBDAV_102 = 102,

		/// <summary>
		/// 103 Checkpoint
		/// This code is used in the Resumable HTTP Requests Proposal to resume aborted PUT or POST requests.[4]
		/// </summary>
		CHECKPOINT_103 = 103,

		/// <summary>
		/// 122 Request-URI too long
		/// This is a non-standard IE7-only code which means the URI is longer than a maximum of 2083
		/// characters.[5][6] (See code 414.)
		/// </summary>
		REQUEST_URI_TOO_LONG_122 = 122,

		/// <summary>
		/// 200 OK
		/// Standard response for successful HTTP requests. The actual response will depend on the request
		/// method used. In a GET request, the response will contain an entity corresponding to the requested
		/// resource. In a POST request the response will contain an entity describing or containing the
		/// result of the action.[2]
		/// </summary>
		OK_200 = 200,

		/// <summary>
		/// 201 Created
		/// The request has been fulfilled and resulted in a new resource being created.[2]
		/// </summary>
		CREATED_201 = 201,

		/// <summary>
		/// 202 Accepted
		/// The request has been accepted for processing, but the processing has not been completed.
		/// The request might or might not eventually be acted upon, as it might be disallowed when
		/// processing actually takes place.[2]
		/// </summary>
		ACCEPTED_202 = 202,

		/// <summary>
		/// 203 Non-Authoritative Information (since HTTP/1.1)
		/// The server successfully processed the request, but is returning information that
		/// may be from another source.[2]
		/// </summary>
		NON_AUTHORITATIVE_INFORMATION_203 = 203,

		/// <summary>
		/// 204 No Content
		/// The server successfully processed the request, but is not returning any content.[2]
		/// </summary>
		NO_CONTENT_204 = 204,

		/// <summary>
		/// 205 Reset Content
		/// The server successfully processed the request, but is not returning any content. Unlike a 204 response, this response requires that the requester reset the document view.[2]
		/// </summary>
		RESET_CONTENT_205 = 205,

		/// <summary>
		/// 206 Partial Content
		/// The server is delivering only part of the resource due to a range header sent by the client. The range header is used by tools like wget to enable resuming of interrupted downloads, or split a download into multiple simultaneous streams.[2]
		/// </summary>
		PARTIAL_CONTENT_206 = 206,

		/// <summary>
		/// 207 Multi-Status (WebDAV) (RFC 4918)
		/// The message body that follows is an XML message and can contain a number of separate response codes, depending on how many sub-requests were made.[7]
		/// </summary>
		MULTI_STATUS_WEBDAV_207 = 207,

		/// <summary>
		/// 208 Already Reported (WebDAV) (RFC 5842)
		/// The members of a DAV binding have already been enumerated in a previous reply to this request, and are not being included again.
		/// </summary>
		ALREADY_REPORTED_WEBDAV_208 = 208,

		/// <summary>
		/// 226 IM Used (RFC 3229)
		/// The server has fulfilled a GET request for the resource, and the response is a representation of the result of one or more instance-manipulations applied to the current instance. [8]
		/// </summary>
		IM_USED_226 = 226,

		/// <summary>
		/// 300 Multiple Choices
		/// Indicates multiple options for the resource that the client may follow.
		/// It, for instance, could be used to present different format options for video,
		/// list files with different extensions, or word sense disambiguation.[2]
		/// </summary>
		MULTIPLE_CHOICES_300 = 300,
		
		/// <summary>
		/// 301 Moved Permanently
		/// This and all future requests should be directed to the given URI.[2]
		/// </summary>
		MOVED_PERMANENTLY_301 = 301,
		
		/// <summary>
		/// 302 Found
		/// This is an example of industrial practice contradicting the standard.
		/// [2] HTTP/1.0 specification (RFC 1945) required the client to perform a temporary redirect
		/// (the original describing phrase was "Moved Temporarily"),[9] but popular browsers implemented
		/// 302 with the functionality of a 303 See Other. Therefore, HTTP/1.1 added status codes 303 and
		/// 307 to distinguish between the two behaviours.[10] However, some Web applications and frameworks
		/// use the 302 status code as if it were the 303.[citation needed]
		/// </summary>
		FOUND_302 = 302,
		
		/// <summary>
		/// 303 See Other (since HTTP/1.1)
		/// The response to the request can be found under another URI using a GET method.
		/// When received in response to a POST (or PUT/DELETE), it should be assumed that the
		/// server has received the data and the redirect should be issued with a separate GET message.[2]
		/// </summary>
		SEE_OTHER_303 = 303,
		
		/// <summary>
		/// 304 Not Modified
		/// Indicates the resource has not been modified since last requested.[2] Typically,
		/// the HTTP client provides a header like the If-Modified-Since header to provide a
		/// time against which to compare. Using this saves bandwidth and reprocessing on both
		/// the server and client, as only the header data must be sent and received in comparison
		/// to the entirety of the page being re-processed by the server, then sent again using more
		/// bandwidth of the server and client.
		/// </summary>
		NOT_MODIFIED_304 = 304,
		
		/// <summary>
		/// 305 Use Proxy (since HTTP/1.1)
		/// Many HTTP clients (such as Mozilla[11] and Internet Explorer) do not correctly handle
		/// responses with this status code, primarily for security reasons.[2]
		/// </summary>
		USE_PROXY_305 = 305,
		
		/// <summary>
		/// 306 Switch Proxy
		/// No longer used.[2] Originally meant "Subsequent requests should use the specified proxy."[12]
		/// </summary>
		SWITCH_PROXY_306 = 306,
		
		/// <summary>
		/// 307 Temporary Redirect (since HTTP/1.1)
		/// In this occasion, the request should be repeated with another URI, but future requests can still use the original URI.
		/// [2] In contrast to 303, the request method should not be changed when reissuing the original request.
		/// For instance, a POST request must be repeated using another POST request.
		/// </summary>
		TEMPORARY_REDIRECT_307 = 307,

		/// <summary>
		/// 308 Resume Incomplete
		/// This code is used in the Resumable HTTP Requests Proposal to resume aborted PUT or POST requests.[4]
		/// </summary>
		RESUME_INCOMPLETE_308 = 308,

		/// <summary>
		/// 400 Bad Request
		/// The request cannot be fulfilled due to bad syntax.[2]
		/// </summary>
		BAD_REQUEST_400 = 400,

		/// <summary>
		/// 401 Unauthorized
		/// Similar to 403 Forbidden, but specifically for use when authentication is possible but has failed or not yet been provided.[2] The response must include a WWW-Authenticate header field containing a challenge applicable to the requested resource. See Basic access authentication and Digest access authentication.
		/// </summary>
		UNAUTHORIZED_401 = 401,
		
		/// <summary>
		/// 402 Payment Required
		/// Reserved for future use.[2] The original intention was that this code might be used as part of some form of digital cash or micropayment scheme, but that has not happened, and this code is not usually used. As an example of its use, however, Apple's MobileMe service generates a 402 error ("httpStatusCode:402" in the Mac OS X Console log) if the MobileMe account is delinquent.[citation needed]
		/// </summary>
		PAYMENT_REQUIRED_402 = 402,
		
		/// <summary>
		/// 403 Forbidden
		/// The request was a legal request, but the server is refusing to respond to it.[2] Unlike a 401 Unauthorized response, authenticating will make no difference.[2]
		/// </summary>
		FORBIDDEN_403 = 403,
		
		/// <summary>
		/// 404 Not Found
		/// The requested resource could not be found but may be available again in the future.[2] Subsequent requests by the client are permissible.
		/// </summary>
		NOT_FOUND_404 = 404,
		
		/// <summary>
		/// 405 Method Not Allowed
		/// A request was made of a resource using a request method not supported by that resource;[2] for example, using GET on a form which requires data to be presented via POST, or using PUT on a read-only resource.
		/// </summary>
		METHOD_NOT_ALLOWED_405 = 405,
		
		/// <summary>
		/// 406 Not Acceptable
		/// The requested resource is only capable of generating content not acceptable according to the Accept headers sent in the request.[2]
		/// </summary>
		NOT_ACCEPTABLE_406 = 406,
		
		/// <summary>
		/// 407 Proxy Authentication Required
		/// The client must first authenticate itself with the proxy.[2]
		/// </summary>
		PROXY_AUTHENTICATION_REQUIRED_407 = 407,
		
		/// <summary>
		/// 408 Request Timeout
		/// The server timed out waiting for the request.[2] According to W3 HTTP specifications: "The client did not produce a request within the time that the server was prepared to wait. The client MAY repeat the request without modifications at any later time."
		/// </summary>
		REQUEST_TIMEOUT_408 = 408,
		
		/// <summary>
		/// 409 Conflict
		/// Indicates that the request could not be processed because of conflict in the request, such as an edit conflict.[2]
		/// </summary>
		CONFLICT_409 = 409,
		
		/// <summary>
		/// 410 Gone
		/// Indicates that the resource requested is no longer available and will not be available again.[2] This should be used when a resource has been intentionally removed and the resource should be purged. Upon receiving a 410 status code, the client should not request the resource again in the future. Clients such as search engines should remove the resource from their indices. Most use cases do not require clients and search engines to purge the resource, and a "404 Not Found" may be used instead.
		/// </summary>
		GONE_410 = 410,
		
		/// <summary>
		/// 411 Length Required
		/// The request did not specify the length of its content, which is required by the requested resource.[2]
		/// </summary>
		LENGTH_REQUIRED_411 = 411,
		
		/// <summary>
		/// 412 Precondition Failed
		/// The server does not meet one of the preconditions that the requester put on the request.[2]
		/// </summary>
		PRECONDITION_FAILED_412 = 412,
		
		/// <summary>
		/// 413 Request Entity Too Large
		/// The request is larger than the server is willing or able to process.[2]
		/// </summary>
		REQUEST_ENTITY_TOO_LARGE_413 = 413,
		
		/// <summary>
		/// 414 Request-URI Too Long
		/// The URI provided was too long for the server to process.[2]
		/// </summary>
		REQUEST_URI_TOO_LONG_414 = 414,
		
		/// <summary>
		/// 415 Unsupported Media Type
		/// The request entity has a media type which the server or resource does not support.[2] For example, the client uploads an image as image/svg+xml, but the server requires that images use a different format.
		/// </summary>
		UNSUPPORTED_MEDIA_TYPE_415 = 415,
		
		/// <summary>
		/// 416 Requested Range Not Satisfiable
		/// The client has asked for a portion of the file, but the server cannot supply that portion.[2] For example, if the client asked for a part of the file that lies beyond the end of the file.
		/// </summary>
		REQUEST_RANGE_NOT_SATISFIABLE_416 = 416,
		
		/// <summary>
		/// 417 Expectation Failed
		/// The server cannot meet the requirements of the Expect request-header field.[2]
		/// </summary>
		EXPECTATION_FAILED_417 = 417,
		
		/// <summary>
		/// 418 I'm a teapot (RFC 2324)
		/// This code was defined in 1998 as one of the traditional IETF April Fools' jokes, in RFC 2324, Hyper Text Coffee Pot Control Protocol, and is not expected to be implemented by actual HTTP servers. However, known implementations do exist.[13] An Nginx HTTP server uses this code to simulate goto-like behaviour in its configuration.[14]
		/// </summary>
		I_AM_A_TEAPOT_418 = 418,

		/// <summary>
		/// 420 Enhance Your Calm
		/// Returned by the Twitter Search and Trends API when the client is being rate limited.[15] Likely a reference to this number's association with marijuana. Other services may wish to implement the 429 Too Many Requests response code instead.
		/// </summary>
		ENHACE_YOUR_CALM_420 = 420,
		
		/// <summary>
		/// 422 Unprocessable Entity (WebDAV) (RFC 4918)
		/// The request was well-formed but was unable to be followed due to semantic errors.[7]
		/// </summary>
		UNPROCESSABLE_ENTITY_WEBDAV_422 = 422,
		
		/// <summary>
		/// 423 Locked (WebDAV) (RFC 4918)
		/// The resource that is being accessed is locked.[7]
		/// </summary>
		LOCKED_WEBDAV_423 = 423,
		
		/// <summary>
		/// 424 Failed Dependency (WebDAV) (RFC 4918)
		/// The request failed due to failure of a previous request (e.g. a PROPPATCH).[7]
		/// </summary>
		FAILED_DEPENDENCY_WEBDAV_424 = 424,
		
		/// <summary>
		/// 425 Unordered Collection (RFC 3648)
		/// Defined in drafts of "WebDAV Advanced Collections Protocol",[16] but not present in "Web Distributed Authoring and Versioning (WebDAV) Ordered Collections Protocol".[17]
		/// </summary>
		UNORDERED_COLLECTION_425 = 425,
		
		/// <summary>
		/// 426 Upgrade Required (RFC 2817)
		/// The client should switch to a different protocol such as TLS/1.0.[18]
		/// </summary>
		UPGRADE_REQUIRED_426 = 426,
		
		/// <summary>
		/// 428 Precondition Required
		/// The origin server requires the request to be conditional. Intended to prevent "the 'lost update' problem, where a client GETs a resource's state, modifies it, and PUTs it back to the server, when meanwhile a third party has modified the state on the server, leading to a conflict."[19] Specified in an Internet-Draft which is approved for publication as RFC.
		/// </summary>
		PRECONDITION_REQUIRED_428 = 428,
		
		/// <summary>
		/// 429 Too Many Requests
		/// The user has sent too many requests in a given amount of time. Intended for use with rate limiting schemes. Specified in an Internet-Draft which is approved for publication as RFC.[19]
		/// </summary>
		TOO_MANY_REQUESTS_429 = 429,
		
		/// <summary>
		/// 431 Request Header Fields Too Large
		/// The server is unwilling to process the request because either an individual header field, or all the header fields collectively, are too large. Specified in an Internet-Draft which is approved for publication as RFC.[19]
		/// </summary>
		REQUEST_HEADER_FIELDS_TOO_LARGE_431 = 431,
		
		/// <summary>
		/// 444 No Response
		/// An nginx HTTP server extension. The server returns no information to the client and closes the connection (useful as a deterrent for malware).
		/// </summary>
		NO_RESPONSE_444 = 444,
		
		/// <summary>
		/// 449 Retry With
		/// A Microsoft extension. The request should be retried after performing the appropriate action.[20]
		/// </summary>
		RETRY_WITH_449 = 449,

		/// <summary>
		/// 450 Blocked by Windows Parental Controls
		/// A Microsoft extension. This error is given when Windows Parental Controls are turned on and are blocking access to the given webpage.[21]
		/// </summary>
		BLOCKED_BY_WINDOWS_PARENTAL_CONTROLS_450 = 450,
		
		/// <summary>
		/// 499 Client Closed Request
		/// An Nginx HTTP server extension. This code is introduced to log the case when the connection is closed by client while HTTP server is processing its request, making server unable to send the HTTP header back.[22]
		/// </summary>
		CLIENT_CLOSED_REQUEST_499 = 499,

		/// <summary>
		/// 500 Internal Server Error
		/// A generic error message, given when no more specific message is suitable.[2]
		/// </summary>
		INTERNAL_SERVER_ERROR_500 = 500,
		
		/// <summary>
		/// 501 Not Implemented
		/// The server either does not recognise the request method, or it lacks the ability to fulfill the request.[2]
		/// </summary>
		NOT_IMPLEMENTED_501 = 501,
		
		/// <summary>
		/// 502 Bad Gateway
		/// The server was acting as a gateway or proxy and received an invalid response from the upstream server.[2]
		/// </summary>
		BAD_GATEWAY_502 = 502,
		
		/// <summary>
		/// 503 Service Unavailable
		/// The server is currently unavailable (because it is overloaded or down for maintenance).[2] Generally, this is a temporary state.
		/// </summary>
		SERVICE_UNAVAILABLE_503 = 503,
		
		/// <summary>
		/// 504 Gateway Timeout
		/// The server was acting as a gateway or proxy and did not receive a timely response from the upstream server.[2]
		/// </summary>
		GATEWAY_TIMEOUT_504 = 504,
		
		/// <summary>
		/// 505 HTTP Version Not Supported
		/// The server does not support the HTTP protocol version used in the request.[2]
		/// </summary>
		HTTP_VERSION_NOT_SUPPORTED_505 = 505,
		
		/// <summary>
		/// 506 Variant Also Negotiates (RFC 2295)
		/// Transparent content negotiation for the request results in a circular reference.[23]
		/// </summary>
		VARIANT_ALSO_NEGOTIATES_506 = 506,
		
		/// <summary>
		/// 507 Insufficient Storage (WebDAV) (RFC 4918)
		/// The server is unable to store the representation needed to complete the request.[7]
		/// </summary>
		INSUFFICIENT_STORAGE_WEBDAV_507 = 507,
		
		/// <summary>
		/// 508 Loop Detected (WebDAV) (RFC 5842)
		/// The server detected an infinite loop while processing the request (sent in lieu of 208).
		/// </summary>
		LOOP_DETECTED_WEBDAV_508 = 508,
		
		/// <summary>
		/// 509 Bandwidth Limit Exceeded (Apache bw/limited extension)
		/// This status code, while used by many servers, is not specified in any RFCs.
		/// </summary>
		BANDWIDTH_LIMIT_EXCEEDED_509 = 509,
		
		/// <summary>
		/// 510 Not Extended (RFC 2774)
		/// Further extensions to the request are required for the server to fulfill it.[24]
		/// </summary>
		NOT_EXTENDED_510 = 510,
		
		/// <summary>
		/// 511 Network Authentication Required
		/// The client needs to authenticate to gain network access. Intended for use by intercepting proxies used to control access to the network (e.g. "captive portals" used to require agreement to Terms of Service before granting full Internet access via a Wi-Fi hotspot). Specified in an Internet-Draft which is approved for publication as RFC.[19]
		/// </summary>
		NETWORK_AUTHENTICATION_REQUIRED_511 = 511,
		
		/// <summary>
		/// 598 Network read timeout error
		/// This status code is not specified in any RFCs, but is used by some[which?] HTTP proxies to signal a network read timeout behind the proxy to a client in front of the proxy.
		/// </summary>
		NETWORK_READ_TIMEOUT_ERROR_598 = 598,
		
		/// <summary>
		/// 599 Network connect timeout error
		/// This status code is not specified in any RFCs, but is used by some[which?] HTTP proxies to signal a network connect timeout behind the proxy to a client in front of the proxy.
		/// </summary>
		NETWORK_CONNECT_TIMEOUT_ERROR_599 = 599,
	}

	public class HttpCodeUtils
	{
		static public string GetStringFromId(HttpCode Code)
		{
			switch (Code)
			{
				// 1xx
				case HttpCode.CONTINUE_100: return "Continue";
				case HttpCode.SWITCHING_PROTOCOLS_101: return "Switching Protocols";
				case HttpCode.PROCESSING_WEBDAV_102: return "Processing";
				case HttpCode.CHECKPOINT_103: return "Checkpoint";
				case HttpCode.REQUEST_URI_TOO_LONG_122: return "Request-URI too long";

				// 2xx
				case HttpCode.OK_200: return "OK";
				case HttpCode.CREATED_201: return "Created";
				case HttpCode.ACCEPTED_202: return "Accepted";
				case HttpCode.NON_AUTHORITATIVE_INFORMATION_203: return "Non-Authoritative Information";
				case HttpCode.NO_CONTENT_204: return "No Content";
				case HttpCode.RESET_CONTENT_205: return "Reset Content";
				case HttpCode.PARTIAL_CONTENT_206: return "Partial Content";
				case HttpCode.MULTI_STATUS_WEBDAV_207: return "Multi-Status";
				case HttpCode.ALREADY_REPORTED_WEBDAV_208: return "Already Reported";
				case HttpCode.IM_USED_226: return "IM Used";

				// 3xx
				case HttpCode.MULTIPLE_CHOICES_300: return "Multiple Choices";
				case HttpCode.MOVED_PERMANENTLY_301: return "Moved Permanently";
				case HttpCode.FOUND_302: return "Found";
				case HttpCode.SEE_OTHER_303: return "See Other";
				case HttpCode.NOT_MODIFIED_304: return "Not Modified";
				case HttpCode.USE_PROXY_305: return "Use Proxy";
				case HttpCode.SWITCH_PROXY_306: return "Switch Proxy";
				case HttpCode.TEMPORARY_REDIRECT_307: return "Temporary Redirect";
				case HttpCode.RESUME_INCOMPLETE_308: return "Resume Incomplete";
				
				// 4xx
				case HttpCode.BAD_REQUEST_400: return "Bad Request";
				case HttpCode.UNAUTHORIZED_401: return "Unauthorized";
				case HttpCode.PAYMENT_REQUIRED_402: return "Payment Required";
				case HttpCode.FORBIDDEN_403: return "Forbidden";
				case HttpCode.NOT_FOUND_404: return "Not Found";
				case HttpCode.METHOD_NOT_ALLOWED_405: return "Method Not Allowed";
				case HttpCode.NOT_ACCEPTABLE_406: return "Not Acceptable";
				case HttpCode.PROXY_AUTHENTICATION_REQUIRED_407: return "Proxy Authentication Required";
				case HttpCode.REQUEST_TIMEOUT_408: return "Request Timeout";
				case HttpCode.CONFLICT_409: return "Conflict";
				case HttpCode.GONE_410: return "Gone";
				case HttpCode.LENGTH_REQUIRED_411: return "Length Required";
				case HttpCode.PRECONDITION_FAILED_412: return "Precondition Failed";
				case HttpCode.REQUEST_ENTITY_TOO_LARGE_413: return "Request Entity Too Large";
				case HttpCode.REQUEST_URI_TOO_LONG_414: return "Request-URI Too Long";
				case HttpCode.UNSUPPORTED_MEDIA_TYPE_415: return "Unsupported Media Type";
				case HttpCode.REQUEST_RANGE_NOT_SATISFIABLE_416: return "Requested Range Not Satisfiable";
				case HttpCode.EXPECTATION_FAILED_417: return "Expectation Failed";
				case HttpCode.I_AM_A_TEAPOT_418: return "I'm a teapot";
				case HttpCode.ENHACE_YOUR_CALM_420: return "Enhance Your Calm";
				case HttpCode.UNPROCESSABLE_ENTITY_WEBDAV_422: return "Unprocessable Entity";
				case HttpCode.LOCKED_WEBDAV_423: return "Locked";
				case HttpCode.FAILED_DEPENDENCY_WEBDAV_424: return "Failed Dependency";
				case HttpCode.UNORDERED_COLLECTION_425: return "Unordered Collection";
				case HttpCode.UPGRADE_REQUIRED_426: return "Upgrade Required";
				case HttpCode.PRECONDITION_REQUIRED_428: return "Precondition Required";
				case HttpCode.TOO_MANY_REQUESTS_429: return "Too Many Requests";
				case HttpCode.REQUEST_HEADER_FIELDS_TOO_LARGE_431: return "Request Header Fields Too Large";
				case HttpCode.NO_RESPONSE_444: return "No Response";
				case HttpCode.RETRY_WITH_449: return "Retry With";
				case HttpCode.BLOCKED_BY_WINDOWS_PARENTAL_CONTROLS_450: return "Blocked by Windows Parental Controls";
				case HttpCode.CLIENT_CLOSED_REQUEST_499: return "Client Closed Request";

				// 5xx
				case HttpCode.INTERNAL_SERVER_ERROR_500: return "Internal Server Error";
				case HttpCode.NOT_IMPLEMENTED_501: return "Not Implemented";
				case HttpCode.BAD_GATEWAY_502: return "Bad Gateway";
				case HttpCode.SERVICE_UNAVAILABLE_503: return "Service Unavailable";
				case HttpCode.GATEWAY_TIMEOUT_504: return "Gateway Timeout";
				case HttpCode.HTTP_VERSION_NOT_SUPPORTED_505: return "HTTP Version Not Supported";
				case HttpCode.VARIANT_ALSO_NEGOTIATES_506: return "Variant Also Negotiates";
				case HttpCode.INSUFFICIENT_STORAGE_WEBDAV_507: return "Insufficient Storage";
				case HttpCode.LOOP_DETECTED_WEBDAV_508: return "Loop Detected";
				case HttpCode.BANDWIDTH_LIMIT_EXCEEDED_509: return "Bandwidth Limit Exceeded";
				case HttpCode.NOT_EXTENDED_510: return "Not Extended";
				case HttpCode.NETWORK_AUTHENTICATION_REQUIRED_511: return "Network Authentication Required";
				case HttpCode.NETWORK_READ_TIMEOUT_ERROR_598: return "Network read timeout error";
				case HttpCode.NETWORK_CONNECT_TIMEOUT_ERROR_599: return "Network connect timeout error";

				// ???
				default: return "Unknwon";
			}
		}
	}
}
