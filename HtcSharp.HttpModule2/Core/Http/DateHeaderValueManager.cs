using System;
using System.Text;
using System.Threading;
using HtcSharp.HttpModule2.Core.Infrastructure;

namespace HtcSharp.HttpModule2.Core.Http {
    internal class DateHeaderValueManager : IHeartbeatHandler {
        private static readonly byte[] _datePreambleBytes = Encoding.ASCII.GetBytes("\r\nDate: ");

        private DateHeaderValues _dateValues;

        public DateHeaderValues GetDateHeaderValues() => _dateValues;

        public void OnHeartbeat(DateTimeOffset now) {
            SetDateValues(now);
        }

        private void SetDateValues(DateTimeOffset value) {
            var dateValue = HeaderUtilities.FormatDate(value);
            var dateBytes = new byte[_datePreambleBytes.Length + dateValue.Length];
            Buffer.BlockCopy(_datePreambleBytes, 0, dateBytes, 0, _datePreambleBytes.Length);
            Encoding.ASCII.GetBytes(dateValue, 0, dateValue.Length, dateBytes, _datePreambleBytes.Length);

            var dateValues = new DateHeaderValues {
                Bytes = dateBytes,
                String = dateValue
            };
            Volatile.Write(ref _dateValues, dateValues);
        }

        public class DateHeaderValues {
            public byte[] Bytes;
            public string String;
        }
    }
}
