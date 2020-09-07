// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using HtcSharp.HttpModule.Http.Features;

namespace HtcSharp.HttpModule.Http {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Http\Http\src\FormFileCollection.cs
    // Start-At-Remote-Line 9
    // SourceTools-End
    /// <summary>
    /// Default implementation of <see cref="IFormFileCollection"/>.
    /// </summary>
    public class FormFileCollection : List<IFormFile>, IFormFileCollection {
        /// <inheritdoc />
        public IFormFile this[string name] => GetFile(name);

        /// <inheritdoc />
        public IFormFile GetFile(string name) {
            foreach (var file in this) {
                if (string.Equals(name, file.Name, StringComparison.OrdinalIgnoreCase)) {
                    return file;
                }
            }

            return null;
        }

        /// <inheritdoc />
        public IReadOnlyList<IFormFile> GetFiles(string name) {
            var files = new List<IFormFile>();

            foreach (var file in this) {
                if (string.Equals(name, file.Name, StringComparison.OrdinalIgnoreCase)) {
                    files.Add(file);
                }
            }

            return files;
        }
    }
}