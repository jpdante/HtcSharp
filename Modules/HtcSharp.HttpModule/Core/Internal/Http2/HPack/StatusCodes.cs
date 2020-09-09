// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Globalization;
using System.Text;

namespace HtcSharp.HttpModule.Core.Internal.Http2.HPack {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\Internal\Http2\HPack\StatusCodes.cs
    // Start-At-Remote-Line 8
    // Ignore-Local-Line-Range 14-158
    // SourceTools-End
    internal static class StatusCodes {
        private static readonly byte[] _bytesStatus100 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status100Continue);
        private static readonly byte[] _bytesStatus101 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status101SwitchingProtocols);
        private static readonly byte[] _bytesStatus102 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status102Processing);

        private static readonly byte[] _bytesStatus200 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status200OK);
        private static readonly byte[] _bytesStatus201 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status201Created);
        private static readonly byte[] _bytesStatus202 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status202Accepted);
        private static readonly byte[] _bytesStatus203 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status203NonAuthoritative);
        private static readonly byte[] _bytesStatus204 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status204NoContent);
        private static readonly byte[] _bytesStatus205 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status205ResetContent);
        private static readonly byte[] _bytesStatus206 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status206PartialContent);
        private static readonly byte[] _bytesStatus207 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status207MultiStatus);
        private static readonly byte[] _bytesStatus208 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status208AlreadyReported);
        private static readonly byte[] _bytesStatus226 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status226IMUsed);

        private static readonly byte[] _bytesStatus300 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status300MultipleChoices);
        private static readonly byte[] _bytesStatus301 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status301MovedPermanently);
        private static readonly byte[] _bytesStatus302 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status302Found);
        private static readonly byte[] _bytesStatus303 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status303SeeOther);
        private static readonly byte[] _bytesStatus304 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status304NotModified);
        private static readonly byte[] _bytesStatus305 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status305UseProxy);
        private static readonly byte[] _bytesStatus306 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status306SwitchProxy);
        private static readonly byte[] _bytesStatus307 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status307TemporaryRedirect);
        private static readonly byte[] _bytesStatus308 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status308PermanentRedirect);

        private static readonly byte[] _bytesStatus400 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status400BadRequest);
        private static readonly byte[] _bytesStatus401 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status401Unauthorized);
        private static readonly byte[] _bytesStatus402 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status402PaymentRequired);
        private static readonly byte[] _bytesStatus403 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status403Forbidden);
        private static readonly byte[] _bytesStatus404 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status404NotFound);
        private static readonly byte[] _bytesStatus405 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status405MethodNotAllowed);
        private static readonly byte[] _bytesStatus406 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status406NotAcceptable);
        private static readonly byte[] _bytesStatus407 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status407ProxyAuthenticationRequired);
        private static readonly byte[] _bytesStatus408 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status408RequestTimeout);
        private static readonly byte[] _bytesStatus409 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status409Conflict);
        private static readonly byte[] _bytesStatus410 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status410Gone);
        private static readonly byte[] _bytesStatus411 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status411LengthRequired);
        private static readonly byte[] _bytesStatus412 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status412PreconditionFailed);
        private static readonly byte[] _bytesStatus413 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status413PayloadTooLarge);
        private static readonly byte[] _bytesStatus414 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status414UriTooLong);
        private static readonly byte[] _bytesStatus415 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status415UnsupportedMediaType);
        private static readonly byte[] _bytesStatus416 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status416RangeNotSatisfiable);
        private static readonly byte[] _bytesStatus417 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status417ExpectationFailed);
        private static readonly byte[] _bytesStatus418 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status418ImATeapot);
        private static readonly byte[] _bytesStatus419 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status419AuthenticationTimeout);
        private static readonly byte[] _bytesStatus421 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status421MisdirectedRequest);
        private static readonly byte[] _bytesStatus422 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status422UnprocessableEntity);
        private static readonly byte[] _bytesStatus423 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status423Locked);
        private static readonly byte[] _bytesStatus424 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status424FailedDependency);
        private static readonly byte[] _bytesStatus426 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status426UpgradeRequired);
        private static readonly byte[] _bytesStatus428 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status428PreconditionRequired);
        private static readonly byte[] _bytesStatus429 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status429TooManyRequests);
        private static readonly byte[] _bytesStatus431 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status431RequestHeaderFieldsTooLarge);
        private static readonly byte[] _bytesStatus451 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status451UnavailableForLegalReasons);

        private static readonly byte[] _bytesStatus500 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status500InternalServerError);
        private static readonly byte[] _bytesStatus501 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status501NotImplemented);
        private static readonly byte[] _bytesStatus502 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status502BadGateway);
        private static readonly byte[] _bytesStatus503 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status503ServiceUnavailable);
        private static readonly byte[] _bytesStatus504 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status504GatewayTimeout);
        private static readonly byte[] _bytesStatus505 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status505HttpVersionNotsupported);
        private static readonly byte[] _bytesStatus506 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status506VariantAlsoNegotiates);
        private static readonly byte[] _bytesStatus507 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status507InsufficientStorage);
        private static readonly byte[] _bytesStatus508 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status508LoopDetected);
        private static readonly byte[] _bytesStatus510 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status510NotExtended);
        private static readonly byte[] _bytesStatus511 = CreateStatusBytes(HttpModule.Http.Abstractions.StatusCodes.Status511NetworkAuthenticationRequired);

        private static byte[] CreateStatusBytes(int statusCode) {
            return Encoding.ASCII.GetBytes(statusCode.ToString(CultureInfo.InvariantCulture));
        }

        public static byte[] ToStatusBytes(int statusCode) {
            return statusCode switch
            {
                HttpModule.Http.Abstractions.StatusCodes.Status100Continue => _bytesStatus100,
                HttpModule.Http.Abstractions.StatusCodes.Status101SwitchingProtocols => _bytesStatus101,
                HttpModule.Http.Abstractions.StatusCodes.Status102Processing => _bytesStatus102,

                HttpModule.Http.Abstractions.StatusCodes.Status200OK => _bytesStatus200,
                HttpModule.Http.Abstractions.StatusCodes.Status201Created => _bytesStatus201,
                HttpModule.Http.Abstractions.StatusCodes.Status202Accepted => _bytesStatus202,
                HttpModule.Http.Abstractions.StatusCodes.Status203NonAuthoritative => _bytesStatus203,
                HttpModule.Http.Abstractions.StatusCodes.Status204NoContent => _bytesStatus204,
                HttpModule.Http.Abstractions.StatusCodes.Status205ResetContent => _bytesStatus205,
                HttpModule.Http.Abstractions.StatusCodes.Status206PartialContent => _bytesStatus206,
                HttpModule.Http.Abstractions.StatusCodes.Status207MultiStatus => _bytesStatus207,
                HttpModule.Http.Abstractions.StatusCodes.Status208AlreadyReported => _bytesStatus208,
                HttpModule.Http.Abstractions.StatusCodes.Status226IMUsed => _bytesStatus226,

                HttpModule.Http.Abstractions.StatusCodes.Status300MultipleChoices => _bytesStatus300,
                HttpModule.Http.Abstractions.StatusCodes.Status301MovedPermanently => _bytesStatus301,
                HttpModule.Http.Abstractions.StatusCodes.Status302Found => _bytesStatus302,
                HttpModule.Http.Abstractions.StatusCodes.Status303SeeOther => _bytesStatus303,
                HttpModule.Http.Abstractions.StatusCodes.Status304NotModified => _bytesStatus304,
                HttpModule.Http.Abstractions.StatusCodes.Status305UseProxy => _bytesStatus305,
                HttpModule.Http.Abstractions.StatusCodes.Status306SwitchProxy => _bytesStatus306,
                HttpModule.Http.Abstractions.StatusCodes.Status307TemporaryRedirect => _bytesStatus307,
                HttpModule.Http.Abstractions.StatusCodes.Status308PermanentRedirect => _bytesStatus308,

                HttpModule.Http.Abstractions.StatusCodes.Status400BadRequest => _bytesStatus400,
                HttpModule.Http.Abstractions.StatusCodes.Status401Unauthorized => _bytesStatus401,
                HttpModule.Http.Abstractions.StatusCodes.Status402PaymentRequired => _bytesStatus402,
                HttpModule.Http.Abstractions.StatusCodes.Status403Forbidden => _bytesStatus403,
                HttpModule.Http.Abstractions.StatusCodes.Status404NotFound => _bytesStatus404,
                HttpModule.Http.Abstractions.StatusCodes.Status405MethodNotAllowed => _bytesStatus405,
                HttpModule.Http.Abstractions.StatusCodes.Status406NotAcceptable => _bytesStatus406,
                HttpModule.Http.Abstractions.StatusCodes.Status407ProxyAuthenticationRequired => _bytesStatus407,
                HttpModule.Http.Abstractions.StatusCodes.Status408RequestTimeout => _bytesStatus408,
                HttpModule.Http.Abstractions.StatusCodes.Status409Conflict => _bytesStatus409,
                HttpModule.Http.Abstractions.StatusCodes.Status410Gone => _bytesStatus410,
                HttpModule.Http.Abstractions.StatusCodes.Status411LengthRequired => _bytesStatus411,
                HttpModule.Http.Abstractions.StatusCodes.Status412PreconditionFailed => _bytesStatus412,
                HttpModule.Http.Abstractions.StatusCodes.Status413PayloadTooLarge => _bytesStatus413,
                HttpModule.Http.Abstractions.StatusCodes.Status414UriTooLong => _bytesStatus414,
                HttpModule.Http.Abstractions.StatusCodes.Status415UnsupportedMediaType => _bytesStatus415,
                HttpModule.Http.Abstractions.StatusCodes.Status416RangeNotSatisfiable => _bytesStatus416,
                HttpModule.Http.Abstractions.StatusCodes.Status417ExpectationFailed => _bytesStatus417,
                HttpModule.Http.Abstractions.StatusCodes.Status418ImATeapot => _bytesStatus418,
                HttpModule.Http.Abstractions.StatusCodes.Status419AuthenticationTimeout => _bytesStatus419,
                HttpModule.Http.Abstractions.StatusCodes.Status421MisdirectedRequest => _bytesStatus421,
                HttpModule.Http.Abstractions.StatusCodes.Status422UnprocessableEntity => _bytesStatus422,
                HttpModule.Http.Abstractions.StatusCodes.Status423Locked => _bytesStatus423,
                HttpModule.Http.Abstractions.StatusCodes.Status424FailedDependency => _bytesStatus424,
                HttpModule.Http.Abstractions.StatusCodes.Status426UpgradeRequired => _bytesStatus426,
                HttpModule.Http.Abstractions.StatusCodes.Status428PreconditionRequired => _bytesStatus428,
                HttpModule.Http.Abstractions.StatusCodes.Status429TooManyRequests => _bytesStatus429,
                HttpModule.Http.Abstractions.StatusCodes.Status431RequestHeaderFieldsTooLarge => _bytesStatus431,
                HttpModule.Http.Abstractions.StatusCodes.Status451UnavailableForLegalReasons => _bytesStatus451,

                HttpModule.Http.Abstractions.StatusCodes.Status500InternalServerError => _bytesStatus500,
                HttpModule.Http.Abstractions.StatusCodes.Status501NotImplemented => _bytesStatus501,
                HttpModule.Http.Abstractions.StatusCodes.Status502BadGateway => _bytesStatus502,
                HttpModule.Http.Abstractions.StatusCodes.Status503ServiceUnavailable => _bytesStatus503,
                HttpModule.Http.Abstractions.StatusCodes.Status504GatewayTimeout => _bytesStatus504,
                HttpModule.Http.Abstractions.StatusCodes.Status505HttpVersionNotsupported => _bytesStatus505,
                HttpModule.Http.Abstractions.StatusCodes.Status506VariantAlsoNegotiates => _bytesStatus506,
                HttpModule.Http.Abstractions.StatusCodes.Status507InsufficientStorage => _bytesStatus507,
                HttpModule.Http.Abstractions.StatusCodes.Status508LoopDetected => _bytesStatus508,
                HttpModule.Http.Abstractions.StatusCodes.Status510NotExtended => _bytesStatus510,
                HttpModule.Http.Abstractions.StatusCodes.Status511NetworkAuthenticationRequired => _bytesStatus511,

                _ => CreateStatusBytes(statusCode)
            };
        }
    }
}