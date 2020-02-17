// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Globalization;
using System.Text;

namespace HtcSharp.HttpModule.Http.Protocols.Http2.HPack {
    internal static class StatusCodes {
        private static readonly byte[] _bytesStatus100 = CreateStatusBytes(Abstractions.StatusCodes.Status100Continue);
        private static readonly byte[] _bytesStatus101 = CreateStatusBytes(Abstractions.StatusCodes.Status101SwitchingProtocols);
        private static readonly byte[] _bytesStatus102 = CreateStatusBytes(Abstractions.StatusCodes.Status102Processing);

        private static readonly byte[] _bytesStatus200 = CreateStatusBytes(Abstractions.StatusCodes.Status200OK);
        private static readonly byte[] _bytesStatus201 = CreateStatusBytes(Abstractions.StatusCodes.Status201Created);
        private static readonly byte[] _bytesStatus202 = CreateStatusBytes(Abstractions.StatusCodes.Status202Accepted);
        private static readonly byte[] _bytesStatus203 = CreateStatusBytes(Abstractions.StatusCodes.Status203NonAuthoritative);
        private static readonly byte[] _bytesStatus204 = CreateStatusBytes(Abstractions.StatusCodes.Status204NoContent);
        private static readonly byte[] _bytesStatus205 = CreateStatusBytes(Abstractions.StatusCodes.Status205ResetContent);
        private static readonly byte[] _bytesStatus206 = CreateStatusBytes(Abstractions.StatusCodes.Status206PartialContent);
        private static readonly byte[] _bytesStatus207 = CreateStatusBytes(Abstractions.StatusCodes.Status207MultiStatus);
        private static readonly byte[] _bytesStatus208 = CreateStatusBytes(Abstractions.StatusCodes.Status208AlreadyReported);
        private static readonly byte[] _bytesStatus226 = CreateStatusBytes(Abstractions.StatusCodes.Status226IMUsed);

        private static readonly byte[] _bytesStatus300 = CreateStatusBytes(Abstractions.StatusCodes.Status300MultipleChoices);
        private static readonly byte[] _bytesStatus301 = CreateStatusBytes(Abstractions.StatusCodes.Status301MovedPermanently);
        private static readonly byte[] _bytesStatus302 = CreateStatusBytes(Abstractions.StatusCodes.Status302Found);
        private static readonly byte[] _bytesStatus303 = CreateStatusBytes(Abstractions.StatusCodes.Status303SeeOther);
        private static readonly byte[] _bytesStatus304 = CreateStatusBytes(Abstractions.StatusCodes.Status304NotModified);
        private static readonly byte[] _bytesStatus305 = CreateStatusBytes(Abstractions.StatusCodes.Status305UseProxy);
        private static readonly byte[] _bytesStatus306 = CreateStatusBytes(Abstractions.StatusCodes.Status306SwitchProxy);
        private static readonly byte[] _bytesStatus307 = CreateStatusBytes(Abstractions.StatusCodes.Status307TemporaryRedirect);
        private static readonly byte[] _bytesStatus308 = CreateStatusBytes(Abstractions.StatusCodes.Status308PermanentRedirect);

        private static readonly byte[] _bytesStatus400 = CreateStatusBytes(Abstractions.StatusCodes.Status400BadRequest);
        private static readonly byte[] _bytesStatus401 = CreateStatusBytes(Abstractions.StatusCodes.Status401Unauthorized);
        private static readonly byte[] _bytesStatus402 = CreateStatusBytes(Abstractions.StatusCodes.Status402PaymentRequired);
        private static readonly byte[] _bytesStatus403 = CreateStatusBytes(Abstractions.StatusCodes.Status403Forbidden);
        private static readonly byte[] _bytesStatus404 = CreateStatusBytes(Abstractions.StatusCodes.Status404NotFound);
        private static readonly byte[] _bytesStatus405 = CreateStatusBytes(Abstractions.StatusCodes.Status405MethodNotAllowed);
        private static readonly byte[] _bytesStatus406 = CreateStatusBytes(Abstractions.StatusCodes.Status406NotAcceptable);
        private static readonly byte[] _bytesStatus407 = CreateStatusBytes(Abstractions.StatusCodes.Status407ProxyAuthenticationRequired);
        private static readonly byte[] _bytesStatus408 = CreateStatusBytes(Abstractions.StatusCodes.Status408RequestTimeout);
        private static readonly byte[] _bytesStatus409 = CreateStatusBytes(Abstractions.StatusCodes.Status409Conflict);
        private static readonly byte[] _bytesStatus410 = CreateStatusBytes(Abstractions.StatusCodes.Status410Gone);
        private static readonly byte[] _bytesStatus411 = CreateStatusBytes(Abstractions.StatusCodes.Status411LengthRequired);
        private static readonly byte[] _bytesStatus412 = CreateStatusBytes(Abstractions.StatusCodes.Status412PreconditionFailed);
        private static readonly byte[] _bytesStatus413 = CreateStatusBytes(Abstractions.StatusCodes.Status413PayloadTooLarge);
        private static readonly byte[] _bytesStatus414 = CreateStatusBytes(Abstractions.StatusCodes.Status414UriTooLong);
        private static readonly byte[] _bytesStatus415 = CreateStatusBytes(Abstractions.StatusCodes.Status415UnsupportedMediaType);
        private static readonly byte[] _bytesStatus416 = CreateStatusBytes(Abstractions.StatusCodes.Status416RangeNotSatisfiable);
        private static readonly byte[] _bytesStatus417 = CreateStatusBytes(Abstractions.StatusCodes.Status417ExpectationFailed);
        private static readonly byte[] _bytesStatus418 = CreateStatusBytes(Abstractions.StatusCodes.Status418ImATeapot);
        private static readonly byte[] _bytesStatus419 = CreateStatusBytes(Abstractions.StatusCodes.Status419AuthenticationTimeout);
        private static readonly byte[] _bytesStatus421 = CreateStatusBytes(Abstractions.StatusCodes.Status421MisdirectedRequest);
        private static readonly byte[] _bytesStatus422 = CreateStatusBytes(Abstractions.StatusCodes.Status422UnprocessableEntity);
        private static readonly byte[] _bytesStatus423 = CreateStatusBytes(Abstractions.StatusCodes.Status423Locked);
        private static readonly byte[] _bytesStatus424 = CreateStatusBytes(Abstractions.StatusCodes.Status424FailedDependency);
        private static readonly byte[] _bytesStatus426 = CreateStatusBytes(Abstractions.StatusCodes.Status426UpgradeRequired);
        private static readonly byte[] _bytesStatus428 = CreateStatusBytes(Abstractions.StatusCodes.Status428PreconditionRequired);
        private static readonly byte[] _bytesStatus429 = CreateStatusBytes(Abstractions.StatusCodes.Status429TooManyRequests);
        private static readonly byte[] _bytesStatus431 = CreateStatusBytes(Abstractions.StatusCodes.Status431RequestHeaderFieldsTooLarge);
        private static readonly byte[] _bytesStatus451 = CreateStatusBytes(Abstractions.StatusCodes.Status451UnavailableForLegalReasons);

        private static readonly byte[] _bytesStatus500 = CreateStatusBytes(Abstractions.StatusCodes.Status500InternalServerError);
        private static readonly byte[] _bytesStatus501 = CreateStatusBytes(Abstractions.StatusCodes.Status501NotImplemented);
        private static readonly byte[] _bytesStatus502 = CreateStatusBytes(Abstractions.StatusCodes.Status502BadGateway);
        private static readonly byte[] _bytesStatus503 = CreateStatusBytes(Abstractions.StatusCodes.Status503ServiceUnavailable);
        private static readonly byte[] _bytesStatus504 = CreateStatusBytes(Abstractions.StatusCodes.Status504GatewayTimeout);
        private static readonly byte[] _bytesStatus505 = CreateStatusBytes(Abstractions.StatusCodes.Status505HttpVersionNotsupported);
        private static readonly byte[] _bytesStatus506 = CreateStatusBytes(Abstractions.StatusCodes.Status506VariantAlsoNegotiates);
        private static readonly byte[] _bytesStatus507 = CreateStatusBytes(Abstractions.StatusCodes.Status507InsufficientStorage);
        private static readonly byte[] _bytesStatus508 = CreateStatusBytes(Abstractions.StatusCodes.Status508LoopDetected);
        private static readonly byte[] _bytesStatus510 = CreateStatusBytes(Abstractions.StatusCodes.Status510NotExtended);
        private static readonly byte[] _bytesStatus511 = CreateStatusBytes(Abstractions.StatusCodes.Status511NetworkAuthenticationRequired);

        private static byte[] CreateStatusBytes(int statusCode) {
            return Encoding.ASCII.GetBytes(statusCode.ToString(CultureInfo.InvariantCulture));
        }

        public static byte[] ToStatusBytes(int statusCode) {
            return statusCode switch
            {
                Abstractions.StatusCodes.Status100Continue => _bytesStatus100,
                Abstractions.StatusCodes.Status101SwitchingProtocols => _bytesStatus101,
                Abstractions.StatusCodes.Status102Processing => _bytesStatus102,

                Abstractions.StatusCodes.Status200OK => _bytesStatus200,
                Abstractions.StatusCodes.Status201Created => _bytesStatus201,
                Abstractions.StatusCodes.Status202Accepted => _bytesStatus202,
                Abstractions.StatusCodes.Status203NonAuthoritative => _bytesStatus203,
                Abstractions.StatusCodes.Status204NoContent => _bytesStatus204,
                Abstractions.StatusCodes.Status205ResetContent => _bytesStatus205,
                Abstractions.StatusCodes.Status206PartialContent => _bytesStatus206,
                Abstractions.StatusCodes.Status207MultiStatus => _bytesStatus207,
                Abstractions.StatusCodes.Status208AlreadyReported => _bytesStatus208,
                Abstractions.StatusCodes.Status226IMUsed => _bytesStatus226,

                Abstractions.StatusCodes.Status300MultipleChoices => _bytesStatus300,
                Abstractions.StatusCodes.Status301MovedPermanently => _bytesStatus301,
                Abstractions.StatusCodes.Status302Found => _bytesStatus302,
                Abstractions.StatusCodes.Status303SeeOther => _bytesStatus303,
                Abstractions.StatusCodes.Status304NotModified => _bytesStatus304,
                Abstractions.StatusCodes.Status305UseProxy => _bytesStatus305,
                Abstractions.StatusCodes.Status306SwitchProxy => _bytesStatus306,
                Abstractions.StatusCodes.Status307TemporaryRedirect => _bytesStatus307,
                Abstractions.StatusCodes.Status308PermanentRedirect => _bytesStatus308,

                Abstractions.StatusCodes.Status400BadRequest => _bytesStatus400,
                Abstractions.StatusCodes.Status401Unauthorized => _bytesStatus401,
                Abstractions.StatusCodes.Status402PaymentRequired => _bytesStatus402,
                Abstractions.StatusCodes.Status403Forbidden => _bytesStatus403,
                Abstractions.StatusCodes.Status404NotFound => _bytesStatus404,
                Abstractions.StatusCodes.Status405MethodNotAllowed => _bytesStatus405,
                Abstractions.StatusCodes.Status406NotAcceptable => _bytesStatus406,
                Abstractions.StatusCodes.Status407ProxyAuthenticationRequired => _bytesStatus407,
                Abstractions.StatusCodes.Status408RequestTimeout => _bytesStatus408,
                Abstractions.StatusCodes.Status409Conflict => _bytesStatus409,
                Abstractions.StatusCodes.Status410Gone => _bytesStatus410,
                Abstractions.StatusCodes.Status411LengthRequired => _bytesStatus411,
                Abstractions.StatusCodes.Status412PreconditionFailed => _bytesStatus412,
                Abstractions.StatusCodes.Status413PayloadTooLarge => _bytesStatus413,
                Abstractions.StatusCodes.Status414UriTooLong => _bytesStatus414,
                Abstractions.StatusCodes.Status415UnsupportedMediaType => _bytesStatus415,
                Abstractions.StatusCodes.Status416RangeNotSatisfiable => _bytesStatus416,
                Abstractions.StatusCodes.Status417ExpectationFailed => _bytesStatus417,
                Abstractions.StatusCodes.Status418ImATeapot => _bytesStatus418,
                Abstractions.StatusCodes.Status419AuthenticationTimeout => _bytesStatus419,
                Abstractions.StatusCodes.Status421MisdirectedRequest => _bytesStatus421,
                Abstractions.StatusCodes.Status422UnprocessableEntity => _bytesStatus422,
                Abstractions.StatusCodes.Status423Locked => _bytesStatus423,
                Abstractions.StatusCodes.Status424FailedDependency => _bytesStatus424,
                Abstractions.StatusCodes.Status426UpgradeRequired => _bytesStatus426,
                Abstractions.StatusCodes.Status428PreconditionRequired => _bytesStatus428,
                Abstractions.StatusCodes.Status429TooManyRequests => _bytesStatus429,
                Abstractions.StatusCodes.Status431RequestHeaderFieldsTooLarge => _bytesStatus431,
                Abstractions.StatusCodes.Status451UnavailableForLegalReasons => _bytesStatus451,

                Abstractions.StatusCodes.Status500InternalServerError => _bytesStatus500,
                Abstractions.StatusCodes.Status501NotImplemented => _bytesStatus501,
                Abstractions.StatusCodes.Status502BadGateway => _bytesStatus502,
                Abstractions.StatusCodes.Status503ServiceUnavailable => _bytesStatus503,
                Abstractions.StatusCodes.Status504GatewayTimeout => _bytesStatus504,
                Abstractions.StatusCodes.Status505HttpVersionNotsupported => _bytesStatus505,
                Abstractions.StatusCodes.Status506VariantAlsoNegotiates => _bytesStatus506,
                Abstractions.StatusCodes.Status507InsufficientStorage => _bytesStatus507,
                Abstractions.StatusCodes.Status508LoopDetected => _bytesStatus508,
                Abstractions.StatusCodes.Status510NotExtended => _bytesStatus510,
                Abstractions.StatusCodes.Status511NetworkAuthenticationRequired => _bytesStatus511,

                _ => CreateStatusBytes(statusCode)
            };
        }
    }
}
