// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.ObjectPool;

namespace HtcSharp.HttpModule2.Http.Features
{
    /// <summary>
    /// Default implementation of <see cref="Microsoft.AspNetCore.Http.Features.IResponseCookiesFeature"/>.
    /// </summary>
    public class ResponseCookiesFeature : Microsoft.AspNetCore.Http.Features.IResponseCookiesFeature
    {
        // Lambda hoisted to static readonly field to improve inlining https://github.com/dotnet/roslyn/issues/13624
        private readonly static Func<Microsoft.AspNetCore.Http.Features.IFeatureCollection, Microsoft.AspNetCore.Http.Features.IHttpResponseFeature> _nullResponseFeature = f => null;

        private FeatureReferences<Microsoft.AspNetCore.Http.Features.IHttpResponseFeature> _features;
        private Microsoft.AspNetCore.Http.IResponseCookies _cookiesCollection;

        /// <summary>
        /// Initializes a new <see cref="ResponseCookiesFeature"/> instance.
        /// </summary>
        /// <param name="features">
        /// <see cref="Microsoft.AspNetCore.Http.Features.IFeatureCollection"/> containing all defined features, including this
        /// <see cref="Microsoft.AspNetCore.Http.Features.IResponseCookiesFeature"/> and the <see cref="Microsoft.AspNetCore.Http.Features.IHttpResponseFeature"/>.
        /// </param>
        public ResponseCookiesFeature(Microsoft.AspNetCore.Http.Features.IFeatureCollection features)
            : this(features, builderPool: null)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="ResponseCookiesFeature"/> instance.
        /// </summary>
        /// <param name="features">
        /// <see cref="Microsoft.AspNetCore.Http.Features.IFeatureCollection"/> containing all defined features, including this
        /// <see cref="Microsoft.AspNetCore.Http.Features.IResponseCookiesFeature"/> and the <see cref="Microsoft.AspNetCore.Http.Features.IHttpResponseFeature"/>.
        /// </param>
        /// <param name="builderPool">The <see cref="ObjectPool{T}"/>, if available.</param>
        public ResponseCookiesFeature(Microsoft.AspNetCore.Http.Features.IFeatureCollection features, ObjectPool<StringBuilder> builderPool)
        {
            if (features == null)
            {
                throw new ArgumentNullException(nameof(features));
            }

            _features.Initalize(features);
        }

        private Microsoft.AspNetCore.Http.Features.IHttpResponseFeature HttpResponseFeature => _features.Fetch(ref _features.Cache, _nullResponseFeature);

        /// <inheritdoc />
        public Microsoft.AspNetCore.Http.IResponseCookies Cookies
        {
            get
            {
                if (_cookiesCollection == null)
                {
                    var headers = HttpResponseFeature.Headers;
                    _cookiesCollection = new ResponseCookies(headers, null);
                }

                return _cookiesCollection;
            }
        }
    }
}
